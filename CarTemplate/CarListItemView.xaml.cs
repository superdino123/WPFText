using System;
using System.Collections.Generic;
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
    /// CarListItemView.xaml 的交互逻辑
    /// </summary>
    public partial class CarListItemView : UserControl
    {
        public CarListItemView()
        {
            InitializeComponent();
        }


        private Car car;
        public Car Car
        {
            get { return car; }
            set
            {
                car = value;
                this.TextBlockName.Text = car.Name;
                this.TextBlockYear.Text = car.Year;
                string uriStr = string.Format(@"E:\4VS\workSpace\Information\CarTemplate\car.png");
                this.ImageLogo.Source = new BitmapImage(new Uri(uriStr,UriKind.Absolute));
            }
        }
    }

}
