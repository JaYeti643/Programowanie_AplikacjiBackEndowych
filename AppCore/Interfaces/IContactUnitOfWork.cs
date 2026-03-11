using System;
using System.Threading.Tasks;

namespace AppCore.Interfaces;

public interface IContactUnitOfWork : IAsyncDisposable
{
    IPersonRepositoryAsync Persons { get; }
    ICompanyRepositoryAsync Companies { get; }
    IOrganizationRepositoryAsync Organizations { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
