namespace Messenger.Common;

public static class RegisteredPortExtensions
{
    public static readonly int MinPort = 1024;
    public static readonly int MaxPort = 49151;

    public static bool IsPortRegistered(int port) =>
        port >= MinPort && port <= MaxPort;
}
