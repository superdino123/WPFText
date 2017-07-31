 
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace PropertyAttached
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Grid grid = new Grid() {ShowGridLines = true};

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            Button button = new Button() {Content = "OK"};
            Grid.SetColumn(button,1);
            Grid.SetRow(button,1);
            grid.Children.Add(button);
            this.Content = grid;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Human human = new Human();
            School.SetGrade(human,6);
            int grade = School.GetGrade(human);
            MessageBox.Show(grade.ToString());
        }
    }

    class School : DependencyObject
    {
        //自定义附加属性
        public static readonly DependencyProperty GradeProperty = 
            DependencyProperty.RegisterAttached("Grade",typeof(int),typeof(School),new UIPropertyMetadata(0));

        public static int GetGrade(DependencyObject obj)
        {
            return (int) obj.GetValue(GradeProperty);
        }

        public static void SetGrade(DependencyObject obj, int value)
        {
            obj.SetValue(GradeProperty,value);
        }
    }

    class Human : DependencyObject
    {

    }
}
