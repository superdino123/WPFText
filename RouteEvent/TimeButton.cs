using System;
using System.Windows;
using System.Windows.Controls;

namespace RouteEvent
{
    public class TimeButton:Button
    {
        //������ע��·���¼�
        //������
        //1.�¼�������
        //2.·���¼��Ĳ��ԣ�Bubble(ð��ʽ�����ϼ�������·��)��Tunnel(���ʽ���ɸ��ڵ��򷢳��¼��Ŀؼ�·��)��Direct(ֱ��ʽ��ֱ�ӽ��¼���Ϣ�ʹ��¼�������)
        //3.�¼�������(�ᱻִ�еĺ���)������
        //4.·���¼���ӵ��������
        public static readonly RoutedEvent ReportTimEvent = EventManager.RegisterRoutedEvent(
            "ReportTime", 
            RoutingStrategy.Bubble, 
            typeof(EventHandler<ReportTimeEventArgs>), 
            typeof(TimeButton));
        //CLR�¼���װ��
        public event RoutedEventHandler ReportTime
        {
            add { this.AddHandler(ReportTimEvent,value);}
            remove { this.RemoveHandler(ReportTimEvent,value);}
        }
        //������·���¼�������Click�¼��ļ�������
        protected override void OnClick()
        {
            base.OnClick();//��֤ԭ�й�������ʹ�ã�Click�¼��ܱ�����

            ReportTimeEventArgs args = new ReportTimeEventArgs(ReportTimEvent,this);
            args.ClickTime = DateTime.Now;
            this.RaiseEvent(args);
        }
    }
}