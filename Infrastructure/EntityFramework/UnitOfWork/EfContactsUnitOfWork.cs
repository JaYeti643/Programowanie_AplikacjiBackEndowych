using System;
using System.Threading.Tasks;
using AppCore.Interfaces;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class EfContactsUnitOfWork(
    IPersonRepositoryAsync personRepository,
    ICompanyRepositoryAsync companyRepository,
    IOrganizationRepositoryAsync organizationRepository,
    // pozostałe repozytoria
    ContactsDbContext context
): IContactUnitOfWork
{
    private IPersonRepositoryAsync _persons = personRepository;
    private ICompanyRepositoryAsync _companies = companyRepository;
    private IOrganizationRepositoryAsync _organizations = organizationRepository;

    public ValueTask DisposeAsync()
    {
        return context.DisposeAsync();
    }
	

    // public ICompanyRepositoryAsync Companies { get; }
    // public IOrganizationRepositoryAsync Organizations { get; }

    // właściwości pozostałych repozytoriów
    IPersonRepositoryAsync IContactUnitOfWork.Persons => _persons;
    ICompanyRepositoryAsync IContactUnitOfWork.Companies => _companies;
    IOrganizationRepositoryAsync IContactUnitOfWork.Organizations => _organizations;

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    Task IContactUnitOfWork.BeginTransactionAsync()
    {
        return BeginTransactionAsync();
    }

    Task IContactUnitOfWork.CommitTransactionAsync()
    {
        return CommitTransactionAsync();
    }

    Task IContactUnitOfWork.RollbackTransactionAsync()
    {
        return RollbackTransactionAsync();
    }

    Task<int> IContactUnitOfWork.SaveChangesAsync()
    {
        return SaveChangesAsync();
    }

    public Task BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }
	
    public Task CommitTransactionAsync()
    {
        return context.Database.CommitTransactionAsync();
    }
	
    public Task RollbackTransactionAsync()
    {
        return context.Database.RollbackTransactionAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        return DisposeAsync();
    }
}