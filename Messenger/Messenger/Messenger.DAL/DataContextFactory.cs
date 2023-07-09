using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Messenger.DAL;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args) => new DataContext(
        new DbContextOptionsBuilder<DataContext>()
        .UseSqlServer(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString)
        .Options);
}
