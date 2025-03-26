using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepositiry
{
    public ProdutoRepository(AppDbContext context) : base(context)
    { }

    public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
    {
        var produtos = await GetAllAsync();
        var produtosordenados = produtos.OrderBy(p => p.ProdutoId).AsQueryable();
        var resultado = await produtosordenados.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);
        return resultado;
    }

    public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
    {
        var produtos = await GetAllAsync();

        if (produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
        {
            if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(P => P.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);

            else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco).OrderBy(p => p.Preco);

            else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco).OrderBy(p => p.Preco);
        }
        var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroParams.PageNumber,
                                                          produtosFiltroParams.PageSize);

        return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
    {
        var produtos = await GetAllAsync();
        var produtosCategoria = produtos.Where(C => C.CategoriaId == id);
        return produtosCategoria;
    }
}
