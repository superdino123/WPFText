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

namespace ListBinding
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<Country> countryList = new List<Country>();
            City city = new City {Name = "成都"};
            Province province = new Province
            {
                Name = "四川",
                CityList = new List<City> {city}
            };
            Country country = new Country
            {
                Name = "中国",
                ProvinceList = new List<Province>() { province }
            };
            countryList.Add(country);
            this.TextBox1.SetBinding(TextBox.TextProperty, new Binding("/Name") { Source = countryList });//Tim
            this.TextBox2.SetBinding(TextBox.TextProperty, new Binding("/ProvinceList.[0].Name") { Source = countryList});//3
            this.TextBox3.SetBinding(TextBox.TextProperty, new Binding("/ProvinceList.[0].CityList.[0].Name") { Source = countryList});//m
        }
    }

    class City
    {
        public string Name { get; set; }
    }

    class Province
    {
        public string Name { get; set; }

        public List<City> CityList { get; set; }
    }

    class Country
    {
        public string Name { get; set; }
        public List<Province> ProvinceList { get; set; }
    }
}
