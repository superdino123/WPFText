using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MyControlTestDatePicker
{
    /// <summary>
    /// 1.继承Control
    /// 2.定义自定义依赖属性
    /// 3.标记模板部件
    /// 4.添加默认模板themes/generic.xaml
    /// 5.静态构造函数中为自定义控件定义默认样式的依赖项属性
    /// 6.设置依赖属性中回调函数配套的私有方法
    /// 7.定义相关的私有字段
    /// 8.后台添加绑定与事件(继承OnApplyTemplate函数)
    /// </summary>
    [TemplatePart(Name = "PART_Root",Type = typeof(Grid))]
    [TemplatePart(Name = "PART_TextBox",Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = "PART_Button",Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup",Type = typeof(Popup))]
    public class MyDatePicker:Control
    {
        #region 私有变量

        private Popup _popUp;//声明Popup控件
        private DateTime? _originalSelectedDate;//记录当前选中的日期
        private Calendar _calendar;//声明日期控件
        private ButtonBase _dropDownButton;//右侧按钮
        private bool _disablePopupReopen;//关闭窗口是否重新打开
        private DatePickerTextBox _textBox;//输入框

        #endregion

        #region 依赖属性

        #region 声明依赖属性

        public static readonly DependencyProperty CalendarStyleProperty;
        public static readonly DependencyProperty DisplayDateProperty;
        public static readonly DependencyProperty DisplayDateEndProperty;
        public static readonly DependencyProperty DisplayDateStartProperty;
        public static readonly DependencyProperty FirstDayOfWeekProperty;
        public static readonly DependencyProperty IsDropDownOpenProperty;
        public static readonly DependencyProperty IsTodayHighlightedProperty;
        public static readonly DependencyProperty SelectedDateProperty;
        public static readonly DependencyProperty SelectedDateFormatProperty;
        public static readonly DependencyProperty TextProperty;

        #endregion

        #region 依赖属性包装器

        public Style CalendarStyle
        {
            get { return (Style) GetValue(CalendarStyleProperty); }
            set { SetValue(CalendarStyleProperty, value); }
        }

        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        public DateTime DisplayDateEnd
        {
            get { return (DateTime)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        public DateTime DisplayDateStart
        {
            get { return (DateTime)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public DatePickerFormat SelectedDateFormat
        {
            get { return (DatePickerFormat)GetValue(SelectedDateFormatProperty); }
            set { SetValue(SelectedDateFormatProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #endregion

        #region 静态构造函数

        static MyDatePicker()
        {
            #region 注册静态函数

            CalendarStyleProperty = DependencyProperty.Register("CalendarStyle", typeof(Style), typeof(MyDatePicker));
            DisplayDateProperty = DependencyProperty.Register("DisplayDate",typeof(DateTime),typeof(MyDatePicker));
            DisplayDateEndProperty = DependencyProperty.Register("DisplayDateEnd", typeof(DateTime),typeof(MyDatePicker));
            DisplayDateStartProperty = DependencyProperty.Register("DisplayDateStart", typeof(DateTime),typeof(MyDatePicker));
            FirstDayOfWeekProperty = DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek),typeof(MyDatePicker));
            IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool),typeof(MyDatePicker),
                new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnIsDropDownOpenChanged));
            IsTodayHighlightedProperty = DependencyProperty.Register("IsTodayHighlighted", typeof(bool),typeof(MyDatePicker));
            SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime),typeof(MyDatePicker));
            SelectedDateFormatProperty = DependencyProperty.Register("SelectedDateFormat", typeof(DatePickerFormat),typeof(MyDatePicker));
            TextProperty = DependencyProperty.Register("Text", typeof(string),typeof(MyDatePicker));

            #endregion

            #region 为自定义控件定义默认样式的依赖项属性

            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyDatePicker), new FrameworkPropertyMetadata(typeof(MyDatePicker)));

            #endregion
        }

        #endregion

        #region 公有方法

        public override void OnApplyTemplate()
        {
            if (_popUp != null)//如果popup已经被赋值
            {
                //------------------路由事件--------------------------要移除的函数
                _popUp.RemoveHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));      
            }

            base.OnApplyTemplate();

            _popUp = GetTemplateChild("PART_Popup") as Popup;//获取前台的popup组件

            if (_popUp != null)
            {   //向 鼠标左键点击 路由事件(委托) 添加 执行函数 改变_disablePopupReopen
                _popUp.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));
                //当Popup的IsOpen属性为true时触发
                _popUp.Opened += PopUp_Opened;
                _popUp.Child = _calendar;
            }

            _dropDownButton = GetTemplateChild("PART_Button") as Button;
            if (_dropDownButton != null)
            {
                _dropDownButton.Click += DropDownButton_Click;
                /*_dropDownButton.AddHandler(MouseLeaveEvent, new MouseEventHandler(DropDownButton_MouseLeave), true);

                // If the user does not provide a Content value in template, we provide a helper text that can be used in Accessibility
                // this text is not shown on the UI, just used for Accessibility purposes
                if (_dropDownButton.Content == null)
                {
                    _dropDownButton.Content = System.SR.Get(SRID.DatePicker_DropDownButtonName);
                }*/
            }
        }

        #endregion

        #region 私有方法

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyDatePicker dp = d as MyDatePicker;
            Debug.Assert(dp != null);//报错提示框

            bool newValue = (bool) e.NewValue;
            if (dp._popUp != null && dp._popUp.IsOpen != newValue)
            {
                dp._popUp.IsOpen = newValue;//更改变量值
                if (newValue)//如果新状态是打开状态
                {
                    dp._originalSelectedDate = dp.SelectedDate;//将依赖属性SelectedDate的值赋给当前选择日期字段

                    //多线程设置
                    dp.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, (Action) delegate()
                    {
                        dp._calendar.Focus();
                    });
                }
            }
        }

        /// <summary>
        /// 鼠标左键点击按钮时我要做什么
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup popup = sender as Popup;
            if (popup != null && !popup.StaysOpen)
            {
                if (_dropDownButton != null)
                {
                    if (_dropDownButton.InputHitTest(e.GetPosition(_dropDownButton)) != null)
                    {
                        _disablePopupReopen = true;//将打开变量改为true
                    }
                }
            }
        }

        private void PopUp_Opened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }

            if (this._calendar != null)
            {
                this._calendar.DisplayMode = CalendarMode.Month;
                this._calendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    
                }
            }
        }

        private void SetSelectedDate()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    string s = _textBox.Text;

                    if (SelectedDate != null)
                    {
                    }
                }
            }
        }

        #endregion
    }
}