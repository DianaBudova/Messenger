using Messenger.Models.DB;
using System.Diagnostics.CodeAnalysis;

namespace Messenger.Common.EqualityComparers;

public class ServerEqualityComparer : IEqualityComparer<Server>
{
    public bool Equals(Server? x, Server? y)
    {
        if (object.ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] Server obj)
    {
        return obj.GetHashCode();
    }
}
