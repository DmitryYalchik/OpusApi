using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpusApi;
using OpusApi.Controllers;
using OpusApi.DbModels;
using OpusApi.Tests.Environments;

namespace OpusApi.Tests.Controllers;

[TestClass]
[TestSubject(typeof(ConnectionController))]
public class ConnectionControllerTest
{
    private InMemoryDbContext _dbContext = null!;
    private ConnectionEnvironment _environment = null!;
    private ConnectionController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        // InMemoryDbContext использует общее in-memory хранилище ("database"),
        // поэтому очищаем его перед каждым тестом для изоляции.
        _dbContext = new InMemoryDbContext();
        _dbContext.Database.EnsureDeleted();
        _environment = new ConnectionEnvironment(_dbContext);
        _controller = new ConnectionController(_dbContext);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [TestMethod]
    public void GetAll_WhenEmpty_ReturnsNoContent()
    {
        var result = _controller.GetAll();

        Assert.IsInstanceOfType<NoContentResult>(result.Result);
    }

    [TestMethod]
    public async Task GetAll_WhenHasConnections_ReturnsOkWithList()
    {
        await _environment.AddConnectionAsync("Оператор Иванов");

        var result = _controller.GetAll();

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var payload = Assert.IsInstanceOfType<List<ConnectionEntity>>(ok.Value);
        Assert.AreEqual(1, payload.Count);
        Assert.AreEqual("Оператор Иванов", payload[0].UserName);
    }
}
