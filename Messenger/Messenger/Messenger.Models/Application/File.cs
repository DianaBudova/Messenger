namespace Messenger.Models.Application;

public class File
{
    public string Path { get; set; }
    public byte[] Content { get; set; }
    public long SizeBytes { get; set; }
}
