using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUIObj.Converters
{
    public class VisibilityToScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return ScrollBarVisibility.Hidden;
            Visibility v = (Visibility)value;
            if (v == Visibility.Visible)
                return ScrollBarVisibility.Visible;
            return ScrollBarVisibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;

            ScrollBarVisibility v = (ScrollBarVisibility)value;
            if (v == ScrollBarVisibility.Visible)
                return Visibility.Visible;
            return Visibility.Hidden;
        }
    }
}
