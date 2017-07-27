using System.Collections.Generic;
using System.Linq;
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

namespace RouteEvent
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
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //第一个参数：绑定事件；第二个参数：处理程序
            this.GridRoot.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.ButtonClicked));
        }

        #region 第一个实例
        //第一个参数：将事件消息交给方法进行处理的节点，不一定是事件发出的节点；第二个参数：状态信息/事件数据/路由事件
        void ButtonClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("扫地机粉丝都放假哦怕的");
        }

        //声明并注册路由事件(触发函数执行的事件)
        //参数：
        //1.事件的名称
        //2.路由事件的策略：Bubble(冒泡式：向上级容器逐级路由)，Tunnel(隧道式：由根节点向发出事件的控件路由)，Direct(直达式：直接将事件消息送达事件处理器)
        //3.事件处理器(会被执行的函数)的类型
        //4.路由事件的拥有者类型
        /*public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ButtonBase));/*注册路由事件*/
        //为路由事件添加CLR事件包装器
        /*
        public event RoutedEventHandler Click
        {
            add { this.AddHandler(ClickEvent, value); }
            remove { this.RemoveHandler(ClickEvent, value); }
        }
        //激发路由事件的方法。此方法在用户单击鼠标时会被Windows系统调用
        protected void OnClick()
        {
            RoutedEventArgs newEvent = new RoutedEventArgs(ButtonBase.ClickEvent, this);
            this.RaiseEvent(newEvent);
        }
        */
        #endregion

        //private void ReportTimeHandler(object sender, ReportTimeEventArgs e)
        //{
        //    FrameworkElement element = sender as FrameworkElement;
        //    string timeStr = e.ClickTime.ToLongTimeString();
        //    string content = string.Format("{0}到达{1}", timeStr, element.Name);
        //    this.ListBox.Items.Add(content);
        //}
    }

    //用于承载时间消息的事件参数
}