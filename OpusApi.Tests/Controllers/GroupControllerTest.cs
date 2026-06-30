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
using OpusApi.Tests.Fakes;

namespace OpusApi.Tests.Controllers;

[TestClass]
[TestSubject(typeof(GroupController))]
public class GroupControllerTest : SqliteTestBase
{
    private GroupEnvironment _environment = null!;
    private FakeEntityNotifier _notifier = null!;
    private GroupController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _environment = new GroupEnvironment(DbContext);
        _notifier = new FakeEntityNotifier();
        _controller = new GroupController(new GroupRepository(DbContext), _notifier);
    }

    // ----- GetAll -----

    [TestMethod]
    public async Task GetAll_WhenEmpty_ReturnsNoContent()
    {
        var result = await _controller.GetAll();

        Assert.IsInstanceOfType<NoContentResult>(result.Result);
    }

    [TestMethod]
    public async Task GetAll_WhenHasItems_ReturnsOkWithAllGroups()
    {
        await _environment.AddGroupAsync("Группа №1");
        await _environment.AddGroupAsync("Группа №2");

        var result = await _controller.GetAll();

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var payload = Assert.IsInstanceOfType<IEnumerable<GroupResponse>>(ok.Value);
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
        var seeded = await _environment.AddGroupAsync("Группа №1");

        var result = await _controller.GetById(seeded.Id);

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var response = Assert.IsInstanceOfType<GroupResponse>(ok.Value);
        Assert.AreEqual(seeded.Id, response.Id);
        Assert.AreEqual("Группа №1", response.Name);
    }

    [TestMethod]
    public async Task GetById_LoadsLinkedSearchersViaInclude()
    {
        var group = await _environment.AddGroupWithSearcherAsync(searcherLastName: "Иванов");

        // Читаем через свежий контекст, чтобы Include реально доставал состав из БД,
        // а не из change-tracker'а.
        await using var verify = CreateContext();
        var controller = new GroupController(new GroupRepository(verify), _notifier);

        var result = await controller.GetById(group.Id);

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var response = Assert.IsInstanceOfType<GroupResponse>(ok.Value);
        Assert.AreEqual(1, response.Searchers.Count());
        Assert.AreEqual("Иванов", response.Searchers.Single().LastName);
    }

    // ----- Create -----

    [TestMethod]
    public async Task Create_WhenValid_Returns201AndPersistsToDatabase()
    {
        var result = await _controller.Create(_environment.Request());

        var created = Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsInstanceOfType<GroupResponse>(created.Value);

        await using var verify = CreateContext();
        var stored = await verify.Groups.FindAsync(response.Id);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Группа №1", stored.Name);

        var notification = _notifier.Sent.Single();
        Assert.AreEqual("Group", notification.Entity);
        Assert.AreEqual("created", notification.Action);
        Assert.AreEqual(response.Id, notification.Id);
    }

    [TestMethod]
    public async Task Create_DoesNotCreateSearchers()
    {
        // Запрос группы в принципе не содержит поисковиков — проверяем, что БД их и не получила.
        await _controller.Create(_environment.Request());

        await using var verify = CreateContext();
        Assert.AreEqual(0, await verify.Searchers.CountAsync());
    }

    [TestMethod]
    public async Task Create_WhenInvalid_Returns400AndDoesNotPersist()
    {
        var result = await _controller.Create(_environment.Request(name: ""));

        Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result);

        await using var verify = CreateContext();
        Assert.AreEqual(0, await verify.Groups.CountAsync());
        Assert.AreEqual(0, _notifier.Sent.Count); // невалидный запрос — уведомления нет
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
        var seeded = await _environment.AddGroupAsync("Группа №1");

        var result = await _controller.Update(seeded.Id, _environment.Request(name: "Группа №2"));

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Groups.FindAsync(seeded.Id);
        Assert.AreEqual("Группа №2", stored!.Name);

        var notification = _notifier.Sent.Single();
        Assert.AreEqual("Group", notification.Entity);
        Assert.AreEqual("updated", notification.Action);
        Assert.AreEqual(seeded.Id, notification.Id);
    }

    [TestMethod]
    public async Task Update_WhenInvalid_Returns400AndDoesNotChange()
    {
        var seeded = await _environment.AddGroupAsync("Группа №1");

        var result = await _controller.Update(seeded.Id, _environment.Request(name: ""));

        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await using var verify = CreateContext();
        var stored = await verify.Groups.FindAsync(seeded.Id);
        Assert.AreEqual("Группа №1", stored!.Name);
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
        var seeded = await _environment.AddGroupAsync("Группа №1");

        var result = await _controller.Delete(seeded.Id);

        Assert.IsInstanceOfType<NoContentResult>(result);

        await using var verify = CreateContext();
        Assert.IsNull(await verify.Groups.FindAsync(seeded.Id));

        var notification = _notifier.Sent.Single();
        Assert.AreEqual("Group", notification.Entity);
        Assert.AreEqual("deleted", notification.Action);
        Assert.AreEqual(seeded.Id, notification.Id);
    }
}
