using System;
using System.Globalization;
using System.Windows.Data;

namespace Messenger.Views.Resources.Converters;

public class OnlineOfflineConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null)
            return "Online";
        else
            return "Offline";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
