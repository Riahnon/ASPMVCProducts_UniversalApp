using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ASPMVCSignalRTest_Client
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public BoolToVisibilityConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                var lValue = (bool)value;
                return lValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
