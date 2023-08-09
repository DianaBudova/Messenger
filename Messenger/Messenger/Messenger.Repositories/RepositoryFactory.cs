using Messenger.Repositories.Interfaces;
using Messenger.DAL;

namespace Messenger.Repositories;

public static class RepositoryFactory
{
    public static IUserRepository GetUserRepository() =>
        new UserRepository(new DataContext());

    public static IServerRepository GetServerRepository() =>
        new ServerRepository(new DataContext());

    public static IChatRepository GetChatRepository() =>
        new ChatRepository(new DataContext());
}
