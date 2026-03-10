using AppCore.Interfaces;
using AppCore.Models;
using Infrastructure.Memory;

namespace UnitTest;

public class UnitTest1
{
    private IGenericRepositoryAsync<Person> _repo;

    public UnitTest1()
    {
        _repo = new MemoryGenericRepository<Person>();
    }

    [Fact]
    public async Task FindByIdAsyncTest()
    {
        // Arrange
        var person = new Person { FirstName = "Jan", LastName = "Kowalski" };
        await _repo.AddAsync(person);

        // Act
        var result = await _repo.FindByIdAsync(person.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
        Assert.Equal("Jan", result.FirstName);
        Assert.Equal("Kowalski", result.LastName);
    }

    [Fact]
    public async Task FindByIdAsyncTest_NotFound()
    {
        // Act
        var result = await _repo.FindByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindAllAsyncTest()
    {
        // Arrange
        var person1 = new Person { FirstName = "Anna", LastName = "Nowak" };
        var person2 = new Person { FirstName = "Piotr", LastName = "Lewandowski" };
        var person3 = new Person { FirstName = "Maria", LastName = "Zalewska" };

        await _repo.AddAsync(person1);
        await _repo.AddAsync(person2);
        await _repo.AddAsync(person3);

        // Act
        var result = await _repo.FindAllAsync();
        var resultList = result.ToList();

        // Assert
        Assert.NotEmpty(resultList);
        Assert.Equal(3, resultList.Count);
        Assert.Contains(person1, resultList);
        Assert.Contains(person2, resultList);
        Assert.Contains(person3, resultList);
    }

    [Fact]
    public async Task FindAllAsyncTest_Empty()
    {
        // Act
        var result = await _repo.FindAllAsync();
        var resultList = result.ToList();

        // Assert
        Assert.Empty(resultList);
    }

    [Fact]
    public async Task FindPagedAsyncTest()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            var person = new Person { FirstName = $"Person{i}", LastName = "Test" };
            await _repo.AddAsync(person);
        }

        // Act
        var result = await _repo.FindPagedAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNext);
    }

    [Fact]
    public async Task FindPagedAsyncTest_SecondPage()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            var person = new Person { FirstName = $"Person{i}", LastName = "Test" };
            await _repo.AddAsync(person);
        }

        // Act
        var result = await _repo.FindPagedAsync(2, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNext);
    }

    [Fact]
    public async Task FindPagedAsyncTest_LastPage()
    {
        // Arrange
        for (int i = 0; i < 25; i++)
        {
            var person = new Person { FirstName = $"Person{i}", LastName = "Test" };
            await _repo.AddAsync(person);
        }

        // Act
        var result = await _repo.FindPagedAsync(3, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.False(result.HasNext);
    }

    [Fact]
    public async Task AddAsyncTest()
    {
        // Arrange
        var person = new Person { FirstName = "Tomasz", LastName = "Wisniewski" };

        // Act
        var result = await _repo.AddAsync(person);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Tomasz", result.FirstName);
        var foundPerson = await _repo.FindByIdAsync(result.Id);
        Assert.NotNull(foundPerson);
        Assert.Equal(result.Id, foundPerson.Id);
    }

    [Fact]
    public async Task AddAsyncTest_WithExistingId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var person = new Person { Id = id, FirstName = "Barbara", LastName = "Krol" };

        // Act
        var result = await _repo.AddAsync(person);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        var foundPerson = await _repo.FindByIdAsync(id);
        Assert.NotNull(foundPerson);
        Assert.Equal(id, foundPerson.Id);
    }

    [Fact]
    public async Task UpdateAsyncTest()
    {
        // Arrange
        var person = new Person { FirstName = "Krzysztof", LastName = "Nowicki" };
        await _repo.AddAsync(person);
        person.FirstName = "Krzysztof Updated";

        // Act
        var result = await _repo.UpdateAsync(person);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Krzysztof Updated", result.FirstName);
        var foundPerson = await _repo.FindByIdAsync(person.Id);
        Assert.NotNull(foundPerson);
        Assert.Equal("Krzysztof Updated", foundPerson.FirstName);
    }

    [Fact]
    public async Task UpdateAsyncTest_NotFound()
    {
        // Arrange
        var person = new Person { Id = Guid.NewGuid(), FirstName = "NonExistent", LastName = "Person" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repo.UpdateAsync(person));
    }

    [Fact]
    public async Task RemoveByIdAsyncTest()
    {
        // Arrange
        var person = new Person { FirstName = "Dawid", LastName = "Kucharski" };
        await _repo.AddAsync(person);
        var id = person.Id;

        // Act
        await _repo.RemoveByIdAsync(id);

        // Assert
        var result = await _repo.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveByIdAsyncTest_NotFound()
    {
        // Act & Assert - Should not throw exception
        await _repo.RemoveByIdAsync(Guid.NewGuid());
    }

    [Fact]
    public async Task RemoveByIdAsyncTest_MultipleRecords()
    {
        // Arrange
        var person1 = new Person { FirstName = "Marek", LastName = "Szpak" };
        var person2 = new Person { FirstName = "Ewa", LastName = "Nawrocka" };
        var person3 = new Person { FirstName = "Stanislaw", LastName = "Duda" };

        await _repo.AddAsync(person1);
        await _repo.AddAsync(person2);
        await _repo.AddAsync(person3);

        // Act
        await _repo.RemoveByIdAsync(person2.Id);

        // Assert
        Assert.NotNull(await _repo.FindByIdAsync(person1.Id));
        Assert.Null(await _repo.FindByIdAsync(person2.Id));
        Assert.NotNull(await _repo.FindByIdAsync(person3.Id));
    }
}