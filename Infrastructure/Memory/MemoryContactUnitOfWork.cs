using System.Threading.Tasks;
using AppCore.Interfaces;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork(
    IPersonRepositoryAsync persons,
    ICompanyRepositoryAsync companies,
    IOrganizationRepositoryAsync organizations
    ) : IContactUnitOfWork
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public IPersonRepositoryAsync Persons => persons;
    public ICompanyRepositoryAsync Companies => companies;
    public IOrganizationRepositoryAsync Organizations => organizations;
    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync()
    {
       return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}
    

