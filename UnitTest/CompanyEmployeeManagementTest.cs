using AppCore.Models;
using Xunit;

namespace UnitTest;

public class CompanyEmployeeManagementTest
{
    private Company _company;

    public CompanyEmployeeManagementTest()
    {
        _company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Tech Corp",
            NIP = "1234567890"
        };
    }

    #region AddEmployee Tests

    [Fact]
    public void AddEmployee_ValidPerson_AddsSuccessfully()
    {
        // Arrange
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

        // Act
        _company.AddEmployee(person);

        // Assert
        Assert.Contains(person, _company.Employees);
        Assert.Equal(_company, person.Employer);
    }

    [Fact]
    public void AddEmployee_NullPerson_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _company.AddEmployee(null));
    }

    [Fact]
    public void AddEmployee_DuplicatePerson_DoesNotAddTwice()
    {
        // Arrange
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        _company.AddEmployee(person);

        // Act
        _company.AddEmployee(person);

        // Assert
        Assert.Single(_company.Employees);
    }

    #endregion

    #region AddEmployees Tests

    [Fact]
    public void AddEmployees_ValidPersons_AddsAllSuccessfully()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        var persons = new List<Person> { person1, person2 };

        // Act
        _company.AddEmployees(persons);

        // Assert
        Assert.Equal(2, _company.Employees.Count);
        Assert.Contains(person1, _company.Employees);
        Assert.Contains(person2, _company.Employees);
        Assert.Equal(_company, person1.Employer);
        Assert.Equal(_company, person2.Employer);
    }

    [Fact]
    public void AddEmployees_NullList_DoesNotThrow()
    {
        // Act
        _company.AddEmployees(null);

        // Assert
        Assert.Empty(_company.Employees);
    }

    #endregion

    #region RemoveEmployee Tests

    [Fact]
    public void RemoveEmployee_ValidPerson_RemovesSuccessfully()
    {
        // Arrange
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        _company.AddEmployee(person);

        // Act
        var result = _company.RemoveEmployee(person);

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(person, _company.Employees);
        Assert.Null(person.Employer);
    }

    [Fact]
    public void RemoveEmployee_NullPerson_ReturnsFalse()
    {
        // Act
        var result = _company.RemoveEmployee(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveEmployee_PersonNotInList_ReturnsFalse()
    {
        // Arrange
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

        // Act
        var result = _company.RemoveEmployee(person);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetEmployees Filtering Tests

    [Fact]
    public void GetEmployees_NoFilters_ReturnsAll()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.GetEmployees();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetEmployees_FilterByLastName_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.GetEmployees(lastName: "Doe");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void GetEmployees_FilterByPosition_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.GetEmployees(position: "Devel");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void GetEmployees_FilterByDateRange_ReturnsMatching()
    {
        // Arrange
        var today = DateTime.Now;
        var yesterday = today.AddDays(-1);
        var tomorrow = today.AddDays(1);
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer", CreatedAt = today };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager", CreatedAt = today.AddDays(-5) };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.GetEmployees(hiredFrom: yesterday, hiredTo: tomorrow);

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    #endregion

    #region GetEmployees Sorting Tests

    [Fact]
    public void GetEmployees_SortByLastNameAscending_ReturnsSorted()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Zoe", Position = "Developer", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Albert", Position = "Manager", CreatedAt = DateTime.Now };
        var person3 = new Person { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Miller", Position = "Analyst", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2, person3 });

        // Act
        var result = _company.GetEmployees(sortBy: EmployeeSortBy.LastName, descending: false);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Albert", result[0].LastName);
        Assert.Equal("Miller", result[1].LastName);
        Assert.Equal("Zoe", result[2].LastName);
    }

    [Fact]
    public void GetEmployees_SortByLastNameDescending_ReturnsSorted()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Zoe", Position = "Developer", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Albert", Position = "Manager", CreatedAt = DateTime.Now };
        var person3 = new Person { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Miller", Position = "Analyst", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2, person3 });

        // Act
        var result = _company.GetEmployees(sortBy: EmployeeSortBy.LastName, descending: true);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Zoe", result[0].LastName);
        Assert.Equal("Miller", result[1].LastName);
        Assert.Equal("Albert", result[2].LastName);
    }

    [Fact]
    public void GetEmployees_SortByPosition_ReturnsSorted()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Zebra Manager", CreatedAt = DateTime.Now };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Analyst", CreatedAt = DateTime.Now };
        var person3 = new Person { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Miller", Position = "Developer", CreatedAt = DateTime.Now };
        _company.AddEmployees(new List<Person> { person1, person2, person3 });

        // Act
        var result = _company.GetEmployees(sortBy: EmployeeSortBy.Position, descending: false);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Analyst", result[0].Position);
        Assert.Equal("Developer", result[1].Position);
        Assert.Equal("Zebra Manager", result[2].Position);
    }

    [Fact]
    public void GetEmployees_SortByHireDate_ReturnsSorted()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 6, 1);
        var date3 = new DateTime(2024, 3, 1);
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer", CreatedAt = date1 };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager", CreatedAt = date2 };
        var person3 = new Person { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Miller", Position = "Analyst", CreatedAt = date3 };
        _company.AddEmployees(new List<Person> { person1, person2, person3 });

        // Act
        var result = _company.GetEmployees(sortBy: EmployeeSortBy.HireDate, descending: false);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(date1, result[0].CreatedAt);
        Assert.Equal(date3, result[1].CreatedAt);
        Assert.Equal(date2, result[2].CreatedAt);
    }

    #endregion

    #region SearchEmployees Tests

    [Fact]
    public void SearchEmployees_ByFirstName_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager" };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.SearchEmployees(firstName: "John");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void SearchEmployees_ByLastName_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager" };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.SearchEmployees(lastName: "Smith");

        // Assert
        Assert.Single(result);
        Assert.Contains(person2, result);
    }

    [Fact]
    public void SearchEmployees_ByPosition_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Position = "Manager" };
        _company.AddEmployees(new List<Person> { person1, person2 });

        // Act
        var result = _company.SearchEmployees(position: "Devel");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void SearchEmployees_WithCompanyNip_MatchingNip_ReturnsResults()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        _company.AddEmployee(person1);

        // Act
        var result = _company.SearchEmployees(firstName: "John", companyNip: "1234567890");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void SearchEmployees_WithCompanyNip_MismatchingNip_ReturnsEmpty()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        _company.AddEmployee(person1);

        // Act
        var result = _company.SearchEmployees(firstName: "John", companyNip: "9999999999");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchEmployees_MultipleCriteria_ReturnsMatchingAll()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith", Position = "Manager" };
        var person3 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Position = "Developer" };
        _company.AddEmployees(new List<Person> { person1, person2, person3 });

        // Act
        var result = _company.SearchEmployees(firstName: "John", lastName: "Doe", position: "Devel");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    [Fact]
    public void SearchEmployees_CaseInsensitive_ReturnsMatching()
    {
        // Arrange
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Position = "Developer" };
        _company.AddEmployee(person1);

        // Act
        var result = _company.SearchEmployees(firstName: "john");

        // Assert
        Assert.Single(result);
        Assert.Contains(person1, result);
    }

    #endregion
}

