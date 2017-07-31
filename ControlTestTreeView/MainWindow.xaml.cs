using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlTestTreeView.Annotations;

namespace ControlTestTreeView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Product product1 = new Product("name1");
            Product product2 = new Product("name2");
            Product product3 = new Product("name3");
            Product product4 = new Product("name4");
            Product product5 = new Product("name5");
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            products.Add(product1);
            products.Add(product2);
            products.Add(product3);
            products.Add(product4);
            products.Add(product5);
            Category category1 = new Category("123", products);
            Category category2 = new Category("123", products);
            Category category3 = new Category("123", products);
            Category category4 = new Category("123", products);
            List<Category> a = new List<Category>();
            a.Add(category1);
            a.Add(category2);
            a.Add(category3);
            a.Add(category4);
            TreeView.ItemsSource = a;
        }
    }

    public class Product:INotifyPropertyChanged
    {
        public Product(string name)
        {
            this.name = name;
        }

        private string name;

        public string Name
        {
            get { return name;}
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
