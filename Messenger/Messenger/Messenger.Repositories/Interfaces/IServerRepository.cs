using Messenger.Models.DB;

namespace Messenger.Repositories.Interfaces;

public interface IServerRepository
{
    public Server? Add(Server server);
    public Server? Update(Server server);
    public bool Remove(Server server);
    public Server? GetById(int id);
    public Server? GetByNameServer(string nameServer);
    public List<Server>? GetAll();
}
