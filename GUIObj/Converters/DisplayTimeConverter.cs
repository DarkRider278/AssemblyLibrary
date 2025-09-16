using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUIObj.Converters
{
    public class DisplayTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return "";
                DateTime dt = (DateTime)value;
                return dt.ToString("T");
            }
            catch
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime();
        }
    }

    public class DisplayTimeConverterMulti : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return "";
            DateTime st, et;
            if (values[0] == DependencyProperty.UnsetValue)
                st = DateTime.MaxValue;
            else
                st = (DateTime)values[0];
            if (values[1] == DependencyProperty.UnsetValue)
                et = DateTime.MaxValue;
            else
                et = (DateTime)values[1];

            //StackPanel w = (StackPanel)values[2];

            double w;
            try
            {
                w=(double)values[2];
                if (double.IsNaN(w) || w == 0)
                {
                    w = 50;
                }
            }
            catch
            {
                w = 50;
            }

            if (w < 50)
                return st.ToString("T");
            return string.Format("{0:T} - {1:T}", st, et);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] splitValues = ((string)value).Split(' ');
            return splitValues;
        }
    }
}
