namespace Messenger.Models.DB;

public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string EncryptedPassword { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public byte[] ProfilePhoto { get; set; } = File.ReadAllBytes($@"../../../../Messenger.Views/Resources/Images/UnknownUser.png");

    public User()
    {
    }

    public User(User user)
    {
        this.Id = user.Id;
        this.Nickname = new(user.Nickname);
        this.EncryptedPassword = new(user.EncryptedPassword);
        this.IpAddress = new(user.IpAddress);
        this.Port = user.Port;
        Array.Copy(user.ProfilePhoto, this.ProfilePhoto, user.ProfilePhoto.Length);
    }

    public static User Parse(User? user)
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
            IpAddress = user.IpAddress,
            Port = user.Port,
            ProfilePhoto = user.ProfilePhoto,
        };
    }

    public bool IsSimilar(User another)
    {
        if (this.Nickname.Equals(another.Nickname))
            return true;
        return false;
    }
}
