using Messenger.Repositories.Interfaces;
using Messenger.DAL;
using System.Collections.Generic;
using System.Linq;
using Messenger.Models.DB;

namespace Messenger.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly DataContext context;

    public UserRepository(DataContext context) =>
        this.context = context;

    public User? Add(User user)
    {
        var result = this.context.User.Add(user).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public User? Update(User user)
    {
        var result = this.context.User.Update(user).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        { return null; }
    }

    public bool Remove(User user)
    {
        this.context.User.Remove(user);
        try
        { return this.context.SaveChanges() > 0; }
        catch
        { return false; }
    }

    public bool Exists(User user)
    {
        try
        {
            return this.context.User
                //.Where(prop => prop.Id == user.Id)
                .Where(prop => prop.Nickname.Equals(user.Nickname))
                .Where(prop => prop.EncryptedPassword.Equals(user.EncryptedPassword))
                //.Where(prop => prop.IpAddress.Equals(user.IpAddress))
                //.Where(prop => prop.Port.Equals(user.Port))
                .Any();
        }
        catch
        { return false; }
    }

    public bool ExistsNickname(string nickname)
    {
        try
        {
            return this.context.User
                .Where(prop => prop.Nickname.Equals(nickname))
                .Any();
        }
        catch
        { return false; }
    }

    public User? GetById(int id)
    {
        try
        {
            return this.context.User
                .Where(prop => prop.Id == id)
                .First();
        }
        catch
        { return null; }
    }

    public User? GetByNickname(string nickname)
    {
        try
        { return this.context.User
                .Where(prop => prop.Nickname.Equals(nickname))
                .First(); }
        catch
        { return null; }
    }

    public List<User>? GetAll()
    {
        try
        { return this.context.User.ToList(); }
        catch
        { return null; }
    }
}