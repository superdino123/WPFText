using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyControlTestLastDateTimePicker.Converter
{
    public class HourAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            TimeSpan timeSpan = dateTime.TimeOfDay;
            double AllHour = timeSpan.TotalHours;
            return AllHour * 360 / 12;
        }

        //单向绑定不调用
        //双向绑定，double类型小数部分处理复杂，不利于实现
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
