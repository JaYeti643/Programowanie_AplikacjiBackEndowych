using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCore.Models;

public class Company : Contact
{
    public string Name { get; set; }
    public string NIP { get; set; }
    public string REGON { get; set; }
    public string KRS { get; set; }
    public string Industry { get; set; }
    public int EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string Website { get; set; }
    public List<Person> Employees { get; set; }
    public Person PrimaryContact { get; set; }
    public Person Employer { get; set; }

    public Company()
    {
        Employees = new List<Person>();
    }

    public void AddEmployee(Person person)
    {
        if (person == null) throw new ArgumentNullException(nameof(person));
        if (Employees.Contains(person)) return;
        Employees.Add(person);
        person.Employer = this;
    }

    public void AddEmployees(IEnumerable<Person> persons)
    {
        if (persons == null) return;
        foreach (var p in persons)
        {
            AddEmployee(p);
        }
    }

    public bool RemoveEmployee(Person person)
    {
        if (person == null) return false;
        var removed = Employees.Remove(person);
        if (removed && person.Employer == this)
        {
            person.Employer = null;
        }
        return removed;
    }
    public List<Person> GetEmployees(string lastName = null, string position = null, DateTime? hiredFrom = null, DateTime? hiredTo = null, EmployeeSortBy? sortBy = null, bool descending = false)
    {
        IEnumerable<Person> query = Employees ?? new List<Person>();

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(p => !string.IsNullOrWhiteSpace(p.LastName) && p.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            query = query.Where(p => !string.IsNullOrWhiteSpace(p.Position) && p.Position.Contains(position, StringComparison.OrdinalIgnoreCase));
        }

        if (hiredFrom.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= hiredFrom.Value);
        }

        if (hiredTo.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= hiredTo.Value);
        }
        
        if (sortBy.HasValue)
        {
            switch (sortBy.Value)
            {
                case EmployeeSortBy.LastName:
                    query = descending
                        ? query.OrderByDescending(p => p.LastName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                        : query.OrderBy(p => p.LastName ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                    break;
                case EmployeeSortBy.Position:
                    query = descending
                        ? query.OrderByDescending(p => p.Position ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                        : query.OrderBy(p => p.Position ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                    break;
                case EmployeeSortBy.HireDate:
                    query = descending
                        ? query.OrderByDescending(p => p.CreatedAt)
                        : query.OrderBy(p => p.CreatedAt);
                    break;
                default:
                    break;
            }
        }

        return query.ToList();
    }

    public override string GetDisplayName()
    {
        return Name;
    }
    
    public List<Person> SearchEmployees(string firstName = null, string lastName = null, string position = null, string companyNip = null)
    {
        if (!string.IsNullOrWhiteSpace(companyNip) && !string.Equals(this.NIP, companyNip, StringComparison.Ordinal))
        {
            return new List<Person>();
        }

        IEnumerable<Person> query = Employees ?? new List<Person>();
        
        if (!string.IsNullOrWhiteSpace(firstName))
        {
            query = query.Where(p => !string.IsNullOrWhiteSpace(p.FirstName) && p.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(p => !string.IsNullOrWhiteSpace(p.LastName) && p.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrWhiteSpace(position))
        {
            query = query.Where(p => !string.IsNullOrWhiteSpace(p.Position) && p.Position.Contains(position, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
}

public enum EmployeeSortBy
{
    LastName,
    Position,
    HireDate
}
