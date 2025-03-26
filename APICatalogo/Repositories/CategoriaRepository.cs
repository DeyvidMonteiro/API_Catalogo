using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    { }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriaParams)
    {
        var categorias = await GetAllAsync();
        var categoriasOrdenada = categorias.OrderBy(p => p.CategoriaId).AsQueryable();

        //var resultado = PagedList<Categoria>.ToPagedList(categoriasOrdenada,
        //    categoriaParams.PageNumber, categoriaParams.PageSize);

        var resultado = await categoriasOrdenada.ToPagedListAsync(categoriaParams.PageNumber, categoriaParams.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriaParams)
    {
        var categorias = (await GetAllAsync()).AsQueryable();

        if (!string.IsNullOrEmpty(categoriaParams.Nome))
            categorias = categorias.Where(c => c.Nome.Contains(categoriaParams.Nome));

        //var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(),
        //categoriaParams.PageNumber, categoriaParams.PageSize);

        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriaParams.PageNumber, categoriaParams.PageSize);

        return categoriasFiltradas;
    }
}
