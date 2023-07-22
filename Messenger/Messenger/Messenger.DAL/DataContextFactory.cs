using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Messenger.DAL;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        return new DataContext(
        new DbContextOptionsBuilder<DataContext>()
        .UseSqlServer(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString)
        .Options);

        //db.Server.Add(new()
        //{
        //    NameServer = "Bila Tserkva Server #1",
        //    IpAddress = "127.0.0.1",
        //    Port = 8888,
        //});
    }
}
