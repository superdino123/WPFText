using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyControlTestLastDateTimePicker.Converter
{
    public class MinuteAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            TimeSpan timeSpan = dateTime.TimeOfDay;
            double AllMinute = timeSpan.TotalMinutes;
            return AllMinute * 360 / 60;
        }

        //单向绑定不调用
        //双向绑定，double类型小数部分处理复杂，不利于实现
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
