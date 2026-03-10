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
        // Arrange
        var expected = new Person()
        {
            FirstName = "Adam"
        };
        // Act
        await _repo.AddAsync(expected);
        // Assert
        var actual = await _repo.FindByIdAsync(expected.Id);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Id, actual?.Id);
    }
}