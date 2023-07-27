using Messenger.Repositories.Interfaces;
using Messenger.DAL;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Repositories;

public static class RepositoryFactory
{
    public static IUserRepository GetUserRepository()
    {
        //var options = new ServiceCollection()
        //    .AddDbContext<DataContext>(options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString))
        //    .BuildServiceProvider()
        //    .GetRequiredService<DbContextOptions>();
        return new UserRepository(new DataContext(/*options*/));
    }

    public static IServerRepository GetServerRepository()
    {
        //var options = new ServiceCollection()
        //    .AddDbContext<DataContext>(options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString))
        //    .BuildServiceProvider()
        //    .GetRequiredService<DbContextOptions>();
        return new ServerRepository(new DataContext(/*options*/));
    }

    public static IChatRepository GetChatRepository()
    {
        return new ChatRepository(new DataContext(/*options*/));
    }
}
