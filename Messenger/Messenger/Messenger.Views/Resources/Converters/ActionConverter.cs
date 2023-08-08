using Messenger.Models.Application;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Messenger.Views.Resources.Converters;

public class ActionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null && value is MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.File:
                    return "Save";
                case MessageType.Audio:
                    return "Play";
                default:
                    return string.Empty;
            }
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
