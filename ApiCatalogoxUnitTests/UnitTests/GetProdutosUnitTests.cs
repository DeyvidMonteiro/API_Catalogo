using APICatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests;

public class GetProdutosUnitTests : IClassFixture<ProdutosUnitTesteController>
{
    private readonly ProdutosController _controller;

    public GetProdutosUnitTests(ProdutosUnitTesteController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task GetProdutoById_Okresult()
    {
        var prodId = 2;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<OkObjectResult>()
                   .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetProdutoById_Return_NotFoun()
    {
        var prodId = 999;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<NotFoundObjectResult>()
                   .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProdutoById_Return_BadRequest()
    {
        var prodId = -1;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<BadRequestObjectResult>()
                   .Which.StatusCode.Should().Be(400);
    }
}
