using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryCustomerService : ICustomerServices
{
    public IEnumerable<Customer> GetCustomers()
    {
        return [
         new Customer()
         {
             Id = 1,
             FirstName = "Jan",
             LastName = "Kowalski",
             Email = "a@wsei.edu.pl",
             Phone = "123456789",
             AddressId = 11

         },
         new Customer()
         {
             Id = 2,
             FirstName = "Anna",
             LastName = "Nowak",
             Email = "b@wsei.edu.pl",
             Phone = "987654321",
             AddressId = 22

         }
        ];
    }

    public Task<IEnumerable<Customer>> GetCustomerAsync()
    {
        return Task.FromResult(GetCustomers());
    }
}