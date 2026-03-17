using AppCore.Interfaces;
using AppCore.Models;
using Infrastructure.Memory;

namespace UnitTest;

public class MemoryGenericRepositoryTest
{
    private IGenericRepositoryAsync<Person> _repo = new MemoryGenericRepository<Person>();

    [Fact]
    public async Task AddPersonTestAsync()
    {
        var expected = new Person()
        {
            FirstName = "Adam"
        };
        await _repo.AddAsync(expected);
        var actual = await _repo.FindByIdAsync(expected.Id);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Id, actual?.Id);
    }

    [Fact]
    public async Task FindByIdReturnsCorrectEntity()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        await _repo.AddAsync(person);

        var result = await _repo.FindByIdAsync(person.Id);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
    }

    [Fact]
    public async Task FindAllReturnsAllEntities()
    {
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        await _repo.AddAsync(person1);
        await _repo.AddAsync(person2);

        var result = await _repo.FindAllAsync();
        var resultList = result.ToList();

        Assert.Contains(person1, resultList);
        Assert.Contains(person2, resultList);
    }

    [Fact]
    public async Task FindPagedReturnsCorrectPage()
    {
        for (int i = 0; i < 10; i++)
        {
            await _repo.AddAsync(new Person { Id = Guid.NewGuid(), FirstName = $"Person{i}" });
        }

        var pagedResult = await _repo.FindPagedAsync(2, 3);

        Assert.Equal(3, pagedResult.Items.Count());
        Assert.Equal(2, pagedResult.Page);
        Assert.Equal(3, pagedResult.PageSize);
    }

    [Fact]
    public async Task UpdateEntityUpdatesCorrectly()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        await _repo.AddAsync(person);
        person.FirstName = "UpdatedName";

        var updatedPerson = await _repo.UpdateAsync(person);

        Assert.Equal("UpdatedName", updatedPerson.FirstName);
    }

    [Fact]
    public async Task RemoveByIdDeletesEntity()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        await _repo.AddAsync(person);

        await _repo.RemoveByIdAsync(person.Id);
        var result = await _repo.FindByIdAsync(person.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByIdAsyncReturnsNullForNonExistentId()
    {
        var result = await _repo.FindByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task FindAllAsyncReturnsEmptyWhenNoEntities()
    {
        var result = await _repo.FindAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindPagedAsyncReturnsEmptyForLargePage()
    {
        var pagedResult = await _repo.FindPagedAsync(100, 10);
        Assert.Empty(pagedResult.Items);
        Assert.Equal(0, pagedResult.TotalCount);
    }

    [Fact]
    public async Task FindPagedAsyncReturnsEmptyForPageSizeZero()
    {
        await _repo.AddAsync(new Person { Id = Guid.NewGuid(), FirstName = "Test" });
        var pagedResult = await _repo.FindPagedAsync(1, 0);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task UpdateAsyncThrowsForNonExistentEntity()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "NonExistent" };
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.UpdateAsync(person));
    }

    [Fact]
    public async Task RemoveByIdAsyncDoesNothingForNonExistentId()
    {
        var id = Guid.NewGuid();
        await _repo.RemoveByIdAsync(id);
        Assert.True(true);
    }

    [Fact]
    public async Task AddAsyncOverwritesExistingEntity()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "Original" };
        await _repo.AddAsync(person);
        person.FirstName = "Updated";
        await _repo.AddAsync(person);
        var result = await _repo.FindByIdAsync(person.Id);
        Assert.Equal("Updated", result.FirstName);
    }
}