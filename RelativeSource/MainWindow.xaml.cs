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

namespace RelativeSource
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Data.RelativeSource rs = new System.Windows.Data.RelativeSource(RelativeSourceMode.FindAncestor);
            rs.AncestorLevel = 1;
            rs.AncestorType = typeof(Grid);
            rs.AncestorType = typeof(DockPanel);
            //rs.Mode = RelativeSourceMode.Self;

            Binding binding = new Binding("Name") {RelativeSource = rs};
            this.TextBox1.SetBinding(TextBox.TextProperty, binding);
        }
    }
}
