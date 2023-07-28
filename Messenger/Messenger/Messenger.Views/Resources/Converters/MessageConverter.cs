using System.Globalization;
using System.Windows.Data;
using System;
using System.Text;
using Messenger.Models.Application;

namespace Messenger.Views.Converters;

public class MessageConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is MessageType messageType && values[1] is byte[] messageData)
        {
            switch (messageType)
            {
                case MessageType.Text:
                    return Encoding.UTF8.GetString(messageData);
                case MessageType.File:
                    return "File Message";
                case MessageType.Audio:
                    return "Audio Message";
                default:
                    return "Unknown Message";
            }
        }
        return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
