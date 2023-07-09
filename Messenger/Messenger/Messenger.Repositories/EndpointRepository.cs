using Messenger.Repositories.Interfaces;
using Messenger.DAL;
using System.Collections.Generic;
using System.Linq;
using Messenger.Models.DB;

namespace Messenger.Repositories;

internal class EndpointRepository : IEndpointRepository
{
    private readonly DataContext context;

    public EndpointRepository(DataContext context) =>
        this.context = context;

    public Endpoint? Add(Endpoint endpoint)
    {
        var result = this.context.Endpoint.Add(endpoint).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public Endpoint? Update(Endpoint endpoint)
    {
        var result = this.context.Endpoint.Update(endpoint).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public bool Remove(Endpoint endpoint)
    {
        this.context.Endpoint.Remove(endpoint);
        try
        { return this.context.SaveChanges() > 0; }
        catch
        { return false; }
    }

    public bool Exists(Endpoint endpoint)
    {
        try
        {
            return this.context.Endpoint
                .Where(prop => prop.Id == endpoint.Id)
                .Where(prop => prop.IpAddress == endpoint.IpAddress)
                .Where(prop => prop.Port == endpoint.Port)
                .Any();
        }
        catch
        { return false; }
    }

    public Endpoint? GetById(int id)
    {
        try
        {
            return this.context.Endpoint
                .Where(prop => prop.Id == id)
                .First();
        }
        catch
        { return null; }
    }

    public List<Endpoint>? GetAll()
    {
        try
        { return this.context.Endpoint.ToList(); }
        catch
        { return null; }
    }
}
