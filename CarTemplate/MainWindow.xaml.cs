using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarTemplate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitialCarList();
        }

        private void InitialCarList()
        {
            List<Car> carList = new List<Car>()
            {
                new Car() { Automaker = "Lamborghini",Name = "Diablo",Year = "1990",TopSpeed = "340"},
                new Car() { Automaker = "Lamborghini",Name = "Murcielago",Year = "2001",TopSpeed = "353"},
                new Car() { Automaker = "Lamborghini",Name = "Gallardo",Year = "2003",TopSpeed = "325"},
                new Car() { Automaker = "Lamborghini",Name = "Reventon",Year = "2008",TopSpeed = "356"}
            };

            this.listBoxCars.ItemsSource = carList;
        }
        
    }

    public class Car
    {
        public string Automaker { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string TopSpeed { get; set; }
    }

    /// <summary>
    /// 厂商名称转换为Logo图片路径
    /// </summary>AutomakerToLogoPathConverter
    public class AutomakerToLogoPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string uriStr = string.Format(@"E:\4VS\workSpace\Information\CarTemplate\car.png");
            return new BitmapImage(new Uri(uriStr, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 汽车名称转换为照片路径
    /// </summary>
    public class NameToPhotoPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string uriStr = string.Format(@"E:\4VS\workSpace\Information\CarTemplate\car.png");
            return new BitmapImage(new Uri(uriStr, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
