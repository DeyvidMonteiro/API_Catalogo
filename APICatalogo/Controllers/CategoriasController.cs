using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[EnableRateLimiting("fixedwindow")]
[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IUnitOfWork uof,
        ILogger<CategoriasController> logger)
    {
        _logger = logger;
        _uof = uof;
    }

    //[Authorize]
    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        var categorias = await _uof.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            return NotFound("Não existem catogirias...");

        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);

    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategotiasFiltradas([FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
        var categoriasFiltradas = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);

        return ObterCategorias(categoriasFiltradas);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

    [DisableCors]
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        _logger.LogInformation("================== GEt api/categorias/====================");

        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"categoria com id = {id} não encontrada...");
            return NotFound($"A categoria com Id {id} não foi encontrado...");
        }

        //var categoriaDto = new CategoriaDTO()
        //{
        //    CategoriaId = categoria.CategoriaId,
        //    Nome = categoria.Nome,
        //    ImagemUrl = categoria.ImagemUrl
        //};

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados Inválidos");
        }

        //var categoria = new Categoria()
        //{
        //    CategoriaId = categoriaDto.CategoriaId,
        //    Nome = categoriaDto.Nome,
        //    ImagemUrl = categoriaDto.ImagemUrl
        //};

        var categoria = categoriaDto.ToCategoria();

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        //var novaCategoriaDto = new CategoriaDTO()
        //{
        //    CategoriaId = categoriaCriada.CategoriaId,
        //    Nome = categoriaCriada.Nome,
        //    ImagemUrl = categoriaCriada.ImagemUrl
        //};

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);

    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        //var categoria = new Categoria()
        //{
        //    CategoriaId = categoriaDto.CategoriaId,
        //    Nome = categoriaDto.Nome,
        //    ImagemUrl = categoriaDto.ImagemUrl
        //};

        var categoria = categoriaDto.ToCategoria();

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        //var categoriaAtualizadaDto = new CategoriaDTO()
        //{
        //    CategoriaId = categoriaAtualizada.CategoriaId,
        //    Nome = categoriaAtualizada.Nome,
        //    ImagemUrl = categoriaAtualizada.ImagemUrl
        //};

        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy ="AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria == null)
            return NotFound("não encontrado...");

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        //var categoriaExcluidaDto = new CategoriaDTO()
        //{
        //    CategoriaId = categoriaExcluida.CategoriaId,
        //    Nome = categoriaExcluida.Nome,
        //    ImagemUrl = categoriaExcluida.ImagemUrl
        //};

        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluida);

    }
}
