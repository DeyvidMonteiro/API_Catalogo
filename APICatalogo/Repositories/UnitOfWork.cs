using APICatalogo.Context;

namespace APICatalogo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepositiry? _produtoRepo;

    private ICategoriaRepository? _catgoriaRepo;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProdutoRepositiry ProdutoRepository
    {
        get { return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context); }
    }

    public ICategoriaRepository CategoriaRepository
    {
        get { return _catgoriaRepo = _catgoriaRepo ?? new CategoriaRepository(_context); }
    }

    public async Task CommitAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
