using Messenger.DAL;
using Messenger.Models.DB;
using Messenger.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Messenger.Repositories;

internal class ServerRepository : IServerRepository
{
    private readonly DataContext context;

    public ServerRepository(DataContext context) =>
        this.context = context;

    public Server? Add(Server server)
    {
        var result = this.context.Server.Add(server).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public Server? Update(Server server)
    {
        var result = this.context.Server.Update(server).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public bool Remove(Server server)
    {
        this.context.Server.Remove(server);
        this.context.ReseedIdentity<Server>();
        try
        { return this.context.SaveChanges() > 0; }
        catch
        { return false; }
    }

    public Server? GetById(int id)
    {
        try
        {
            return this.context.Server
                .Where(prop => prop.Id == id)
                .First();
        }
        catch
        { return null; }
    }

    public Server? GetByNameServer(string nameServer)
    {
        try
        {
            return this.context.Server
                .Where(prop => prop.NameServer == nameServer)
                .First();
        }
        catch
        { return null; }
    }

    public List<Server>? GetAll()
    {
        try
        {
            return this.context.Server.ToList();
        }
        catch
        { return null; }
    }
}
