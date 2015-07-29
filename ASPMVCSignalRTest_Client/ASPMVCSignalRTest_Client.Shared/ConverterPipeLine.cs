using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ASPMVCSignalRTest_Client
{
    public class ConverterPipeLine : IValueConverter
    {
        public ConverterPipeLine()
        {
            this.ValueConverters = new List<IValueConverter>();
        }
        public List<IValueConverter> ValueConverters
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            foreach (var lConverter in this.ValueConverters)
                value = lConverter.Convert(value, targetType, parameter, language);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            foreach (var lConverter in this.ValueConverters.Reverse<IValueConverter>())
                value = lConverter.Convert(value, targetType, parameter, language);

            return value;
        }
    }
}
