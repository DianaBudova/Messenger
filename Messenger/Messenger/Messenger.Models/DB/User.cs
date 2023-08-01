namespace Messenger.Models.DB;

public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string EncryptedPassword { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public int? Port { get; set; }
    public byte[] ProfilePhoto { get; set; } = File.ReadAllBytes($@"../../../../Messenger.Views/Resources/Images/UnknownUser.png");
    public Server? LastUsingServer { get; set; }
    public List<Chat>? SentChat { get; set; }
    public List<Chat>? ReceivedChat { get; set; }

    public override bool Equals(object? another)
    {
        if (another is null || another is not User)
            return false;
        User user = (User)another;
        return this.Id == user.Id
            && this.Nickname == user.Nickname
            && this.EncryptedPassword == user.EncryptedPassword
            && this.IpAddress == user.IpAddress
            && this.Port == user.Port
            && this.ProfilePhoto.SequenceEqual(user.ProfilePhoto);
    }
}
