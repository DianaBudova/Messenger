using Messenger.Repositories.Interfaces;
using Messenger.DAL;
using Messenger.Models.DB;
using System.Collections.Generic;

namespace Messenger.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext context;

    public UserRepository(DataContext context) =>
        this.context = context ?? throw new ArgumentNullException(nameof(context));

    public User? Add(User user)
    {
        var result = this.context.User.Add(user).Entity;
        try
        {
            this.context.SaveChanges();
            return result;
        }
        catch
        {
            return null;
        }
    }

    public User? Update(User user)
    {
        User? existingUser = this.context.User.FirstOrDefault(u => u.Nickname == user.Nickname);
        if (existingUser is null)
            return null;
        existingUser.Nickname = user.Nickname;
        existingUser.EncryptedPassword = user.EncryptedPassword;
        existingUser.IpAddress = user.IpAddress;
        existingUser.Port = user.Port;
        existingUser.ProfilePhoto = user.ProfilePhoto;
        existingUser.LastUsingServer = user.LastUsingServer;
        try
        {
            this.context.SaveChanges();
            return existingUser;
        }
        catch
        {
            return null;
        }
    }

    public bool Remove(User user)
    {
        this.context.User.Remove(user);
        this.context.ReseedIdentity<User>();
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
                .Where(prop => prop.Id == user.Id)
                .Where(prop => prop.Nickname.Equals(user.Nickname))
                .Where(prop => prop.EncryptedPassword.Equals(user.EncryptedPassword))
                .Where(prop => prop.IpAddress.Equals(user.IpAddress))
                .Where(prop => prop.Port.Equals(user.Port))
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

    public User? GetByPort(int port)
    {
        try
        {
            return this.context.User
                .Where(prop => prop.Port == port)
                .First();
        }
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