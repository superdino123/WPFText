using System;
using System.Windows;
using System.Windows.Controls;

namespace RouteEvent
{
    public class TimeButton:Button
    {
        //声明和注册路由事件
        //参数：
        //1.事件的名称
        //2.路由事件的策略：Bubble(冒泡式：向上级容器逐级路由)，Tunnel(隧道式：由根节点向发出事件的控件路由)，Direct(直达式：直接将事件消息送达事件处理器)
        //3.事件处理器(会被执行的函数)的类型
        //4.路由事件的拥有者类型
        public static readonly RoutedEvent ReportTimEvent = EventManager.RegisterRoutedEvent(
            "ReportTime", 
            RoutingStrategy.Bubble, 
            typeof(EventHandler<ReportTimeEventArgs>), 
            typeof(TimeButton));
        //CLR事件包装器
        public event RoutedEventHandler ReportTime
        {
            add { this.AddHandler(ReportTimEvent,value);}
            remove { this.RemoveHandler(ReportTimEvent,value);}
        }
        //激发了路由事件，借用Click事件的激发方法
        protected override void OnClick()
        {
            base.OnClick();//保证原有功能正常使用，Click事件能被激发

            ReportTimeEventArgs args = new ReportTimeEventArgs(ReportTimEvent,this);
            args.ClickTime = DateTime.Now;
            this.RaiseEvent(args);
        }
    }
}