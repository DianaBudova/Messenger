namespace Messenger.Models.Application;

public struct MultimediaMessage
{
    public byte[]? Content { get; set; } = Array.Empty<byte>();
    public MultimediaMessageType Type { get; set; }

    public MultimediaMessage() { }
}
