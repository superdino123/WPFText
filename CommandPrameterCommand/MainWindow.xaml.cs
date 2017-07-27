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

namespace CommandPrameterCommand
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

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.NameTextBox.Text))
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string name = this.NameTextBox.Text;
            if (e.Parameter.ToString() == "Teacher")
            {
                //this.ListBoxNewItems.Items.Add(string.Format("New Teacher:{0},学而不厌，诲人不倦", name));
                ListBoxNewItems.Text += "12";
            }
            if (e.Parameter.ToString() == "Student")
            {
                //ListBoxNewItems.Items.Add(string.Format("New Student:{0},好好学习，天天向上", name));
                ListBoxNewItems.Text += "34";
            }
            NameTextBox.Text = null;
        }
    }
}
