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

namespace MyControlTestColor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void colorUserControl_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = e.NewValue.ToString();
            StackPanel.Children.Add(textBlock);
        }
    }
}
