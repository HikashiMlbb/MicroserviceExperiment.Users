namespace Application.Users;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository Users { get; }
    
    public Task Commit();
    public Task Rollback();
}