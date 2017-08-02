using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MultiBinding
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetMultiBinding();
        }

        private void SetMultiBinding()
        {
            //准备基础Binding
            var b1 = new Binding("Text") {Source = TextBox1};
            var b2 = new Binding("Text") {Source = TextBox2};
            var b3 = new Binding("Text") {Source = TextBox3};
            var b4 = new Binding("Text") {Source = TextBox4};

            //准备MultiBinding
            var mb = new System.Windows.Data.MultiBinding {Mode = BindingMode.OneWay};
            mb.Bindings.Add(b1); //MultiBinding对Add子Binding的顺序是敏感的
            mb.Bindings.Add(b2);
            mb.Bindings.Add(b3);
            mb.Bindings.Add(b4);
            mb.Converter = new LogonMultiBindingConverter();

            //将Button与MultiBinding对象关联
            Button.SetBinding(IsEnabledProperty, mb);
        }
    }

    //注重基类的变化
    public class LogonMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Cast<string>().Any(text => string.IsNullOrEmpty(text))
                && values[0].ToString() == values[1].ToString()
                && values[2].ToString() == values[3].ToString())
                return true;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}