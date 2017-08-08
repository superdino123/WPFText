using System;
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

namespace PropertyObject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Student stu;
        public MainWindow()
        {
            InitializeComponent();
            stu = new Student();
            stu.SetBinding(Student.NameProperty, new Binding("Text") {Source = TextBox1});
            TextBox2.SetBinding(TextBox.TextProperty, new Binding("Name") {Source = stu});
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //没有属性包装器时
            //Student stu = new Student();
            //stu.SetValue(Student.NameProperty,this.TextBox1.Text);
            //TextBox2.Text = (string) stu.GetValue(Student.NameProperty);
            
            //有了属性包装器后
            Student stu = new Student();
            stu.Name = this.TextBox1.Text;
            this.TextBox2.Text = stu.Name;
        }
    }

    public class Student : DependencyObject
    {
        /// <summary>
        /// 声明依赖属性
        /// 参数(包装该属性的CLR属性，该属性的类型，该属性的宿主的类型)
        /// </summary>
        public static readonly DependencyProperty NameProperty;

        static Student()
        {
            NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Student));
        }

        /// <summary>
        /// CLR属性包装器
        /// 为啥要用属性包装器：因为直接用依赖属性需要用GetValue和属性转换，看起来不方便,而且wpf有些功能会直接调用 GetValue / SetValue
        /// 达到效果：
        /// 原来这么写 (string) stu.GetValue(Student.NameProperty);
        /// 现在这么些 stu.Name
        /// </summary>
        public string Name
        {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty,value);}
        }

        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding)
        {
            return BindingOperations.SetBinding(this,dp,binding);
        }
    }
}