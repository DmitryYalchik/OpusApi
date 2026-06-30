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
[TestSubject(typeof(IncidentController))]
public class IncidentControllerTest : SqliteTestBase
{
    private IncidentEnvironment _environment = null!;
    private IncidentController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _environment = new IncidentEnvironment(DbContext);
        _controller = new IncidentController(new IncidentRepository(DbContext));
    }

    // ----- GetAll -----

    [TestMethod]
    public async Task GetAll_WhenEmpty_ReturnsNoContent()
    {
        var result = await _controller.GetAll();

        Assert.IsInstanceOfType<NoContentResult>(result.Result);
    }

    [TestMethod]
    public async Task GetAll_WhenHasItems_ReturnsOkWithAllIncidents()
    {
        await _environment.AddIncidentAsync("Сообщение 1");
        await _environment.AddIncidentAsync("Сообщение 2");

        var result = await _controller.GetAll();

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var payload = Assert.IsInstanceOfType<IEnumerable<IncidentResponse>>(ok.Value);
        Assert.AreEqual(2, payload.Count());
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
        var seeded = await _environment.AddIncidentAsync("Привет");

        var result = await _controller.GetById(seeded.Id);

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var response = Assert.IsInstanceOfType<IncidentResponse>(ok.Value);
        Assert.AreEqual(seeded.Id, response.Id);
        Assert.AreEqual("Привет", response.Message);
    }

    // ----- Create -----

    [TestMethod]
    public async Task Create_WhenValid_Returns201AndPersistsToDatabase()
    {
        var result = await _controller.Create(_environment.Request());

        var created = Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsInstanceOfType<IncidentResponse>(created.Value);

        await using var verify = CreateContext();
        var stored = await verify.Incidents.FindAsync(response.Id);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Штаб", stored.From);
    }

    [TestMethod]
    public async Task Create_WhenInvalid_Returns400AndDoesNotPersist()
    {
        var request = _environment.Request();
        request.From = ""; // обязательное поле

        var result = await _controller.Create(request);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result);

        await using var verify = CreateContext();
        Assert.AreEqual(0, await verify.Incidents.CountAsync());
    }

    [TestMethod]
    public async Task Create_WhenMessageTooLong_Returns400()
    {
        var request = _environment.Request();
        request.Message = new string('a', 351); // максимум 350

        var result = await _controller.Create(request);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result);

        await using var verify = CreateContext();
        Assert.AreEqual(0, await verify.Incidents.CountAsync());
    }

    // ----- Update -----

    [TestMethod]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        var result = await _controller.Update(Guid.NewGuid(), _environment.Request());

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Update_WhenValid_Returns204AndPersistsChange()
    {
        var seeded = await _environment.AddIncidentAsync("Старое сообщение");

        var request = _environment.Request();
        request.Message = "Новое сообщение";

        var result = await _controller.Update(seeded.Id, request);

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Incidents.FindAsync(seeded.Id);
        Assert.AreEqual("Новое сообщение", stored!.Message);
    }

    [TestMethod]
    public async Task Update_WhenInvalid_Returns400AndDoesNotChange()
    {
        var seeded = await _environment.AddIncidentAsync("Старое сообщение");

        var request = _environment.Request();
        request.To = ""; // обязательное поле

        var result = await _controller.Update(seeded.Id, request);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Incidents.FindAsync(seeded.Id);
        Assert.AreEqual("Старое сообщение", stored!.Message);
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
        var seeded = await _environment.AddIncidentAsync();

        var result = await _controller.Delete(seeded.Id);

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        Assert.IsNull(await verify.Incidents.FindAsync(seeded.Id));
    }
}
