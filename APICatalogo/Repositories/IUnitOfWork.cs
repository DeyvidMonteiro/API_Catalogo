namespace APICatalogo.Repositories;

public interface IUnitOfWork
{
    IProdutoRepositiry ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    Task CommitAsync();
}
