using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICustomerServices
{
    public IEnumerable<Customer> GetCustomers();
    public Task<IEnumerable<Customer>> GetCustomerAsync();

}