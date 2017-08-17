using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyControlTestLastDateTimePicker.Converter
{
    public class AmPmComboBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            int SelectedHour = dateTime.Hour;
            string a = SelectedHour > 11 ? "下午" : "上午";
            return SelectedHour > 11 ? "下午" : "上午";
        }

        //单向绑定，不需要双向绑定
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
