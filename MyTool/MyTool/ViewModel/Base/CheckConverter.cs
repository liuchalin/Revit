using System;
using System.Globalization;
using System.Windows.Data;

namespace MyTool.ViewModel
{
    class CheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            if (value.ToString() == parameter.ToString())
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }
            if ((bool)value)
            {
                return parameter;
            }
            return null;
        }
    }
}
