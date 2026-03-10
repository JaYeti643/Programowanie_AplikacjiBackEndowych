using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICustomerServices
{
    public IEnumerable<Customer> GetCustomers();
    public Task<IEnumerable<Customer>> GetCustomerAsync();

}