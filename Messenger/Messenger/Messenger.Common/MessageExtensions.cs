using Messenger.Models.Application;

namespace Messenger.Common;

public static class MessageExtensions
{
    public static bool IsValid(this Message message)
    {
        if (message.Sender is null
            || message.Recipient is null
            || message.Content?.Length <= 0)
            return false;
        return true;
    }
}
