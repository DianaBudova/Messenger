using Messenger.Models.DB;
using System.Collections.Generic;

namespace Messenger.Repositories.Interfaces;

public interface IEndpointRepository
{
    public Endpoint? Add(Endpoint endpoint);
    public Endpoint? Update(Endpoint endpoint);
    public bool Remove(Endpoint endpoint);
    public bool Exists(Endpoint endpoint);
    public Endpoint? GetById(int id);
    public List<Endpoint>? GetAll();
}
