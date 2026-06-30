using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpusApi.Controllers;
using OpusApi.Dtos;
using OpusApi.Repositories;
using OpusApi.Tests.Environments;

namespace OpusApi.Tests.Controllers;

[TestClass]
[TestSubject(typeof(SearcherController))]
public class SearcherControllerTest : SqliteTestBase
{
    private SearcherEnvironment _environment = null!;
    private SearcherController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _environment = new SearcherEnvironment(DbContext);
        _controller = new SearcherController(new SearcherRepository(DbContext));
    }

    // ----- GetAll -----

    [TestMethod]
    public async Task GetAll_WhenEmpty_ReturnsNoContent()
    {
        var result = await _controller.GetAll();

        Assert.IsInstanceOfType<NoContentResult>(result.Result);
    }

    [TestMethod]
    public async Task GetAll_WhenHasItems_ReturnsOkWithAllSearchers()
    {
        await _environment.AddSearcherAsync("Иванов");
        await _environment.AddSearcherAsync("Петров");

        var result = await _controller.GetAll();

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var payload = Assert.IsInstanceOfType<IEnumerable<SearcherResponse>>(ok.Value);
        Assert.HasCount(2, payload);
    }

    // ----- GetById -----

    [TestMethod]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        var result = await _controller.GetById(Guid.NewGuid());

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_WhenFound_ReturnsOkWithResponse()
    {
        var seeded = await _environment.AddSearcherAsync("Иванов");

        var result = await _controller.GetById(seeded.Id);

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var response = Assert.IsInstanceOfType<SearcherResponse>(ok.Value);
        Assert.AreEqual(seeded.Id, response.Id);
        Assert.AreEqual("Иванов", response.LastName);
    }

    // ----- Create -----

    [TestMethod]
    public async Task Create_WhenValid_Returns201AndPersistsToDatabase()
    {
        var result = await _controller.Create(_environment.Request());

        var created = Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsInstanceOfType<SearcherResponse>(created.Value);

        await using var verify = CreateContext();
        var stored = await verify.Searchers.FindAsync(response.Id);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Иванов", stored.LastName);
    }

    [TestMethod]
    public async Task Create_WhenInvalid_Returns400AndDoesNotPersist()
    {
        var request = _environment.Request(lastName: "Ив"); // короче 3 символов

        var result = await _controller.Create(request);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result);

        await using var verify = CreateContext();
        Assert.AreEqual(0, await verify.Searchers.CountAsync());
    }

    // ----- Update -----

    [TestMethod]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        var result = await _controller.Update(Guid.NewGuid(), _environment.Request());

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Update_WhenValid_Returns204_PersistsChange_PreservesCreatedAt()
    {
        var seeded = await _environment.AddSearcherAsync("Иванов");

        // Берём дату создания уже после round-trip через БД, чтобы сравнение было точным.
        DateTime originalCreatedAt;
        await using (var pre = CreateContext())
        {
            originalCreatedAt = (await pre.Searchers.FindAsync(seeded.Id))!.CreatedAt;
        }

        var result = await _controller.Update(seeded.Id, _environment.Request(lastName: "Петров", firstName: "Иван"));

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Searchers.FindAsync(seeded.Id);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Петров", stored.LastName);
        Assert.AreEqual("Иван", stored.FirstName);
        Assert.AreEqual(originalCreatedAt, stored.CreatedAt); // дата создания не затёрта при обновлении
    }

    [TestMethod]
    public async Task Update_WhenInvalid_Returns400AndDoesNotChange()
    {
        var seeded = await _environment.AddSearcherAsync("Иванов");

        var request = _environment.Request(firstName: ""); // обязательное поле

        var result = await _controller.Update(seeded.Id, request);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Searchers.FindAsync(seeded.Id);
        Assert.AreEqual("Иванов", stored!.LastName); // данные в БД не изменились
    }

    // ----- Delete -----

    [TestMethod]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        var result = await _controller.Delete(Guid.NewGuid());

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Delete_WhenExists_Returns204AndRemovesFromDatabase()
    {
        var seeded = await _environment.AddSearcherAsync("Иванов");

        var result = await _controller.Delete(seeded.Id);

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        Assert.IsNull(await verify.Searchers.FindAsync(seeded.Id));
    }
}
