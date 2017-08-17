using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyControlTestLastDateTimePicker.Converter
{
    public class HourComboBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.Hour < 12 ? dateTime.Hour : dateTime.Hour - 12;
        }

        //单向绑定，不会调用
        //不用双向绑定的原因 ： 小时数无法为整个日期时间做源
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
