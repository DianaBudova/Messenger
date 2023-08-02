using Messenger.Repositories;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Messenger.Views.Resources.Converters;

public class UserConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int userId)
        {
            var user = RepositoryFactory.GetUserRepository().GetById(userId);
            return user?.GetType().GetProperty(parameter?.ToString())?.GetValue(user);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
