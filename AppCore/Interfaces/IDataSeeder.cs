using System.Threading.Tasks;

namespace AppCore.Interfaces;

public interface IDataSeeder
{
    public int Order { get; }
    Task SeedAsync();
}

