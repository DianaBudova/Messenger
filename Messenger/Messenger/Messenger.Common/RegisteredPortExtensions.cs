namespace Messenger.Common;

public static class RegisteredPortExtensions
{
    public static readonly int MinPort = 1024;
    public static readonly int MaxPort = 65535;

    public static bool IsPortAppropriate(int port) =>
        port >= MinPort && port <= MaxPort;
}
