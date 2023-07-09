namespace Messenger.Models.Application;

public class User
{
    public string Nickname { get; set; } = null!;
    public string EncryptedPassword { get; set; } = null!;
    public byte[]? ProfilePhoto { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TypeAccessException"></exception>
    public static User Parse(DB.User? user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        if (user.Nickname is not string)
            throw new TypeAccessException(nameof(user.Nickname));
        if (user.EncryptedPassword is not string)
            throw new TypeAccessException(nameof(user.EncryptedPassword));
        return new()
        {
            Nickname = user.Nickname,
            EncryptedPassword = user.EncryptedPassword,
            ProfilePhoto = user.ProfilePhoto,
        };
    }

    public bool IsSimilar(User another)
    {
        if (this.Nickname.Equals(another.Nickname))
            return true;
        if (this.EncryptedPassword.Equals(another.EncryptedPassword))
            return true;
        if (this.ProfilePhoto == another.ProfilePhoto)
            return true;
        return false;
    }
}
