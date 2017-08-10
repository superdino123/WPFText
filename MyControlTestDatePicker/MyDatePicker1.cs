using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyControlTestDatePicker
{
    public class MyDatePicker1:DatePicker
    {
        static MyDatePicker1()
        {
            #region 为自定义控件定义默认样式的依赖项属性

            //DefaultStyleKeyProperty.OverrideMetadata(typeof(MyDatePicker), new FrameworkPropertyMetadata(typeof(MyDatePicker1)));

            #endregion
        }
    }
}
