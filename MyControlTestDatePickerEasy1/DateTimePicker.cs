using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyControlTestDatePickerEasy1
{
    /// <summary>
    ///     静态构造函数 构造函数 组件名 非依赖属性 依赖属性 公有方法 私有方法 事件处理方法
    /// </summary>
    [TemplatePart(Name = ElementGrid, Type = typeof(Grid))]
    [TemplatePart(Name = ElementDatePickerTextBox, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    public class DateTimePicker : Control
    {
        #region 1静态构造函数

        static DateTimePicker()
        {
            #region 应用默认主题样式

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));

            #endregion
        }

        #endregion

        #region 2构造函数

        #endregion

        #region 6公共方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            #region 获取模板控件

            _grid = GetTemplateChild(ElementGrid) as Grid;
            _datePickerTextBox = GetTemplateChild(ElementDatePickerTextBox) as DatePickerTextBox;
            _button = GetTemplateChild(ElementButton) as Button;
            Popup = GetTemplateChild(ElementPopup) as Popup;

            #endregion

            /*AddCalendar();
            if (!string.IsNullOrEmpty(_calendar.SelectedDate.ToString()))
                MessageBox.Show(_calendar.SelectedDate.ToString());*/
            //_datePickerTextBox.SetBinding(TextBox.TextProperty, new Binding("SelectedDate") {ElementName = ElementCalendar});

            #region 添加事件

            _button.Click += Button_Click;

            #endregion
        }

        #endregion

        #region 7私有方法

        private void AddCalendar()
        {
            _calendar = new Calendar();
            Popup.Child = _calendar;
            _calendar.Name = ElementCalendar;
            Popup.PlacementTarget = _datePickerTextBox;
            Popup.IsOpen = false;
        }

        #endregion

        #region 8事件处理方法

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
            _datePickerTextBox.Text = _calendar.SelectedDate.ToString();
        }

        #endregion

        #region 3组件名

        private const string ElementGrid = "PART_Grid";
        private const string ElementDatePickerTextBox = "PART_DatePickerTextBox";
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";
        private const string ElementCalendar = "PART_Calendar";

        #endregion

        #region 4非依赖属性

        private Grid _grid;
        private DatePickerTextBox _datePickerTextBox;
        private Button _button;
        private Calendar _calendar;

        protected internal Popup Popup { get; private set; }

        #endregion

        #region 5依赖属性

        public IEnumerable<int> SourceHours
        {
            get { return (IEnumerable<int>) GetValue(SourceHoursProperty); }
            set { SetValue(SourceHoursProperty, value); }
        }

        public IEnumerable<int> SourceMinutes
        {
            get { return (IEnumerable<int>) GetValue(SourceMinutesProperty); }
            set { SetValue(SourceMinutesProperty, value); }
        }

        public IEnumerable<int> SourceSeconds
        {
            get { return (IEnumerable<int>) GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        #region 注册依赖属性

        // Using a DependencyProperty as the backing store for SourceSeconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceSecondsProperty =
            DependencyProperty.Register("SourceSeconds", typeof(IEnumerable<int>), typeof(DateTimePicker), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for SourceMinutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceMinutesProperty =
            DependencyProperty.Register("SourceMinutes", typeof(IEnumerable<int>), typeof(DateTimePicker), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for SourceHours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceHoursProperty =
            DependencyProperty.Register("SourceHours", typeof(IEnumerable<int>), typeof(DateTimePicker), new PropertyMetadata(0));

        #endregion

        #endregion
    }
}