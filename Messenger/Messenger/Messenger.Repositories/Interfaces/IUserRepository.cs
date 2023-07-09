using Messenger.Models.DB;
using System.Collections.Generic;

namespace Messenger.Repositories.Interfaces;

public interface IUserRepository
{
    public User? Add(User user);
    public User? Update(User user);
    public bool Remove(User user);
    public bool Exists(User user);
    public bool ExistsNickname(string nickname);
    public User? GetById(int id);
    public User? GetByNickname(string nickname);
    public List<User>? GetAll();
}
