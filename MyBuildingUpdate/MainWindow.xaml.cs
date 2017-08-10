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

namespace MyBuildingUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*
                *目标对象A.SetBinding(目标对象A属性A，Binding实例)；
                Binding实例：Binding Binding实例 = new Binding(){Source = 源控件B，Path = 源控件B属性B，Mode = 绑定方式

                */
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BindingExpression binding = TxtFontSize.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }
    }
}
