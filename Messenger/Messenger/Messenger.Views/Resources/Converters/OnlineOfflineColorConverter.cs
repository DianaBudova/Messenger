﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Messenger.Views.Resources.Converters;

public class OnlineOfflineColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null)
        {
            return new SolidColorBrush(Colors.Green);
        }
        else
        {
            return new SolidColorBrush(Colors.Red);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
