using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ASPMVCProducts_UniversalApp
{
    public class IsNullConverter : IValueConverter
    {
        public IsNullConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
