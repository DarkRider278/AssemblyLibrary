using System;
using System.Globalization;
using System.Windows.Data;

namespace GUIObj.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            double v = 0;
            try
            {
                v = Double.Parse(value.ToString(), CultureInfo.CurrentCulture.NumberFormat);
            }
            catch
            {
                // ignored
            }

            double p = 1.0;
            try
            {
                if (parameter != null) p = Double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
            }
            catch
            {
                // ignored
            }

            if (p < 1)
                p = 1;
            if (v < 1)
                v = 1;
            return v / p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            return value;
        }
    }
}
