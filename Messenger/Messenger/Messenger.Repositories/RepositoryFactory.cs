using Messenger.Repositories.Interfaces;
using Messenger.DAL;

namespace Messenger.Repositories;

public static class RepositoryFactory
{
    public static readonly IUserRepository SharedUserRepository;
    public static readonly IServerRepository SharedServerRepository;
    public static readonly IChatRepository SharedChatRepository;

    static RepositoryFactory()
    {
        RepositoryFactory.SharedUserRepository = RepositoryFactory.GetUserRepository();
        RepositoryFactory.SharedServerRepository = RepositoryFactory.GetServerRepository();
        RepositoryFactory.SharedChatRepository = RepositoryFactory.GetChatRepository();
    }

    public static IUserRepository GetUserRepository() =>
        new UserRepository(new DataContext());

    public static IServerRepository GetServerRepository() =>
        new ServerRepository(new DataContext());

    public static IChatRepository GetChatRepository() =>
        new ChatRepository(new DataContext());
}
