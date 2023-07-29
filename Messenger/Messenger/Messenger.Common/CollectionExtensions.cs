using Messenger.Models.DB;

namespace Messenger.Common;

public static class CollectionExtensions
{
    public static void Remove<T>(this ICollection<User> sequence, User itemToRemove)
    {
        sequence.Remove(sequence.Where(item => item.IsSimilar(itemToRemove)).First());
    }
}
