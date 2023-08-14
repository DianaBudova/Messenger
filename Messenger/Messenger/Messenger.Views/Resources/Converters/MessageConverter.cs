using System.Globalization;
using System.Windows.Data;
using System;
using System.Text;
using Messenger.Models.Application;
using System.IO;
using Newtonsoft.Json.Linq;
using Messenger.Common;

namespace Messenger.Views.Resources.Converters;

public class MessageConverter : IMultiValueConverter
{
    private const uint minCharsForAudioMessage = 10;
    private const uint maxCharsForAudioMessage = 30;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is MessageType messageType && values[1] is byte[] messageData)
        {
            switch (messageType)
            {
                case MessageType.Text:
                    string textMessage = Encoding.UTF8.GetString(messageData);
                    return textMessage[1..^1];
                case MessageType.File:
                    string fileAsJson = Encoding.UTF8.GetString(messageData);
                    JObject jObject = JObject.Parse(fileAsJson);
                    string? filePath = jObject["Path"]?.ToString();
                    if (filePath is null)
                        return "File name is unknown";
                    string fileName = Path.GetFileName(filePath);
                    return fileName;
                case MessageType.Audio:
                    return AudioHelper.BuildString(MessageConverter.minCharsForAudioMessage, MessageConverter.maxCharsForAudioMessage);
                    //return "⠙⣅⠯⣄⠸⢵⡽⣴⠃⠣⣔⡞⢋⡆⢪⣹⡀";
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
