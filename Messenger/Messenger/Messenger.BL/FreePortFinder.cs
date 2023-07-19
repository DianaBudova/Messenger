namespace Messenger.BL;

public static class FreePortFinder
{
    private const int minPort = 1024;
    private const int maxPort = 49151;

    public static int FindFreePort(int from = minPort)
    {
        if (from < minPort || from > maxPort)
            return 0;
        return Random.Shared.Next(minPort, maxPort);
    }
}
