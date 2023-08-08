using Messenger.Repositories.Interfaces;
using Messenger.DAL;

namespace Messenger.Repositories;

public static class RepositoryFactory
{
    public static IUserRepository GetUserRepository()
    {
        return new UserRepository(new DataContext());
    }

    public static IServerRepository GetServerRepository()
    {
        return new ServerRepository(new DataContext());
    }

    public static IChatRepository GetChatRepository()
    {
        return new ChatRepository(new DataContext());
    }
}
