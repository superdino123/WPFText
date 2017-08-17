using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyControlTestLastDateTimePicker.Converter
{
    public class DateTextBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            string DateStr = dateTime.ToShortDateString();
            string TimeStr =  dateTime.ToLongTimeString();
            return DateStr + " " + TimeStr;
        }

        //单向绑定不会调用
        //原因 ： 还没到这一步，一会就用到了
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
