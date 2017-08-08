using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Calendar = System.Windows.Controls.Calendar;

namespace ControlTestDatePicker4
{
    [TemplatePart(Name = ElementRoot, Type = typeof(Grid))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    public class MyDatePicker : DatePicker
    {
        #region Constants

        private const string ElementRoot = "PART_Root";
        private const string ElementTextBox = "PART_TextBox";
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";

        #endregion Constants

        #region Data

        private Calendar _calendar;
        private string _defaultText;
        private ButtonBase _dropDownButton;
        private Popup _popUp;
        private bool _disablePopupReopen;
        private DatePickerTextBox _textBox;
        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;
        private DateTime? _originalSelectedDate;

        #endregion Data

        #region Public Events
        /// <summary>
        /// 声明路由事件
        /// </summary>
        public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent("SelectedDateChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(MyDatePicker));

        /// <summary>
        /// CLR事件包装器
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        /// <summary>
        /// 日历关闭时发生
        /// </summary>
        public event RoutedEventHandler CalendarClosed;

        /// <summary>
        /// 日历打开时发生
        /// </summary>
        public event RoutedEventHandler CalendarOpened;

        /// <summary>
        /// 日期文本错误解析时发生
        /// </summary>
        public event EventHandler<DatePickerDateValidationErrorEventArgs> DateValidationError;

        #endregion Public Events

        #region Public properties

        #region BlackoutDates

        /// <summary>
        ///     返回不能选择的日期
        /// Gets the days that are not selectable.
        /// </summary>
        public CalendarBlackoutDatesCollection BlackoutDates
        {
            get { return _calendar.BlackoutDates; }
        }

        #endregion BlackoutDates

        #region CalendarStyle

        /// <summary>
        ///     日期控件样式
        /// Gets or sets the style that is used when rendering the calendar.
        /// </summary>
        public Style CalendarStyle
        {
            get { return (Style)GetValue(CalendarStyleProperty); }
            set { SetValue(CalendarStyleProperty, value); }
        }

        /// <summary>
        ///     日期控件样式的依赖属性
        /// Identifies the CalendarStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarStyleProperty =
            DependencyProperty.Register(
                "CalendarStyle",
                typeof(Style),
                typeof(MyDatePicker));

        #endregion CalendarStyle

        #region DisplayDate

        /// <summary>
        ///     显示的日期
        /// Gets or sets the date to display.
        /// </summary>
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        ///     显示日期的依赖属性
        /// Identifies the DisplayDate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
                "DisplayDate",
                typeof(DateTime),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayDate));

        /// <summary>
        /// 显示时间被强制改变时的委托关联函数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDate in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDate = (DateTime)value;
            return dp._calendar.DisplayDate;
        }

        #endregion DisplayDate

        #region DisplayDateEnd

        /// <summary>
        ///     显示时间的最大值
        /// Gets or sets the last date to be displayed.
        /// </summary>
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        ///     显示时间最大值的依赖属性
        /// Identifies the DisplayDateEnd dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
                "DisplayDateEnd",
                typeof(DateTime?),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateEndChanged, CoerceDisplayDateEnd));

        /// <summary>
        ///     显示时间最大值被改变后的委托关联函数
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDateEnd.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            dp.CoerceValue(DisplayDateProperty);
        }

        /// <summary>
        /// 显示时间最大值被强制改变时的委托关联函数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDateEnd in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDateEnd = (DateTime?)value;
            return dp._calendar.DisplayDateEnd;
        }

        #endregion DisplayDateEnd

        #region DisplayDateStart

        /// <summary>
        ///     显示时间的最小值
        /// Gets or sets the first date to be displayed.
        /// </summary>
        public DateTime? DisplayDateStart
        {
            get { return (DateTime?)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        /// <summary>
        ///     显示时间最小值的依赖属性
        /// Identifies the DisplayDateStart dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(
                "DisplayDateStart",
                typeof(DateTime?),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateStartChanged, CoerceDisplayDateStart));

        /// <summary>
        ///     显示时间最小值的改变后委托函数
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDateStart.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            dp.CoerceValue(DisplayDateEndProperty);
            dp.CoerceValue(DisplayDateProperty);
        }

        /// <summary>
        /// 显示时间最小值的强制改变委托函数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDateStart in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDateStart = (DateTime?)value;
            return dp._calendar.DisplayDateStart;
        }

        #endregion DisplayDateStart

        #region FirstDayOfWeek

        /// <summary>
        ///     一周第一天
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        ///     一周第一天的依赖属性
        /// 变动点 ：删掉了第五个参数 Calendar.IsValidFirstDayOfWeek，可能回调会出问题
        /// Identifies the FirstDayOfWeek dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
                "FirstDayOfWeek",
                typeof(DayOfWeek),
                typeof(MyDatePicker),
                null);
        //删掉了第五个参数 Calendar.IsValidFirstDayOfWeek，可能回调会出问题
        // <param name="validateValueCallback">对回调的引用，除了典型的类型验证之外，该引用还应执行依赖项对象值的任何自定义验证。</param>
        #endregion FirstDayOfWeek

        #region IsDropDownOpen

        /// <summary>
        ///     日期控件是否显示
        /// Gets or sets a value that indicates whether the drop-down Calendar is open or closed.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        ///     日期控件是否显示的依赖属性
        /// Identifies the IsDropDownOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                "IsDropDownOpen",
                typeof(bool),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        /// <summary>
        /// 日期框控件是否显示强制改变委托函数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            if (!dp.IsEnabled)
                return false;

            return baseValue;
        }

        /// <summary>
        ///     日期控件是否打开改变后委托函数
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its IsDropDownOpen.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            var newValue = (bool)e.NewValue;
            if (dp._popUp != null && dp._popUp.IsOpen != newValue)
            {
                dp._popUp.IsOpen = newValue;
                if (newValue)
                {
                    dp._originalSelectedDate = dp.SelectedDate;

                    // When the popup is opened set focus to the DisplayDate button.
                    // Do this asynchronously because the IsDropDownOpen could
                    // have been set even before the template for the DatePicker is
                    // applied. And this would mean that the visuals wouldn't be available yet.

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate
                    {
                        // setting the focus to the calendar will focus the correct date.
                        dp._calendar.Focus();
                    });
                }
            }
        }

        /// <summary>
        /// 控件是否能用属性改变后委托关联函数
        /// 变动点 ：注释最后一个方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            dp.CoerceValue(IsDropDownOpenProperty);

            //OnVisualStatePropertyChanged(d, e);
        }

        #endregion IsDropDownOpen

        #region IsTodayHighlighted

        /// <summary>
        ///     选中日期是否突出显示
        /// Gets or sets a value that indicates whether the current date will be highlighted.
        /// </summary>
        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        /// <summary>
        ///     选中日期是否突出显示的依赖属性
        /// Identifies the IsTodayHighlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(
                "IsTodayHighlighted",
                typeof(bool),
                typeof(MyDatePicker));

        #endregion IsTodayHighlighted

        #region SelectedDate

        /// <summary>
        ///     当前选中的日期
        /// Gets or sets the currently selected date.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        ///     当前选中日期的依赖属性
        /// Identifies the SelectedDate dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
                "SelectedDate",
                typeof(DateTime?),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged, CoerceSelectedDate));

        /// <summary>
        ///     选中日期改变后的委托关联函数
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its SelectedDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            var addedItems = new Collection<DateTime>();
            var removedItems = new Collection<DateTime>();
            DateTime? addedDate;
            DateTime? removedDate;

            dp.CoerceValue(DisplayDateStartProperty);
            dp.CoerceValue(DisplayDateEndProperty);
            dp.CoerceValue(DisplayDateProperty);

            addedDate = (DateTime?)e.NewValue;
            removedDate = (DateTime?)e.OldValue;

            if (dp.SelectedDate.HasValue)
            {
                var day = dp.SelectedDate.Value;
                dp.SetTextInternal(dp.DateTimeToString(day));

                // When DatePickerDisplayDateFlag is TRUE, the SelectedDate change is coming from the Calendar UI itself,
                // so, we shouldn't change the DisplayDate since it will automatically be changed by the Calendar
                if ((day.Month != dp.DisplayDate.Month || day.Year != dp.DisplayDate.Year) && !dp._calendar.DatePickerDisplayDateFlag)
                    dp.SetCurrentValueInternal(DisplayDateProperty, day);

                dp._calendar.DatePickerDisplayDateFlag = false;
            }
            else
            {
                dp.SetWaterMarkText();
            }

            if (addedDate.HasValue)
                addedItems.Add(addedDate.Value);

            if (removedDate.HasValue)
                removedItems.Add(removedDate.Value);

            dp.OnSelectedDateChanged(new CalendarSelectionChangedEventArgs(DatePicker.SelectedDateChangedEvent, removedItems, addedItems));

            var peer = UIElementAutomationPeer.FromElement(dp) as DatePickerAutomationPeer;
            // Raise the propetyChangeEvent for Value if Automation Peer exist
            if (peer != null)
            {
                var addedDateString = addedDate.HasValue ? dp.DateTimeToString(addedDate.Value) : "";
                var removedDateString = removedDate.HasValue ? dp.DateTimeToString(removedDate.Value) : "";
                peer.RaiseValuePropertyChangedEvent(removedDateString, addedDateString);
            }
        }

        /// <summary>
        /// 强制设置选择的日期
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceSelectedDate(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.SelectedDate in order to get _calendar to compute the coerced value
            dp._calendar.SelectedDate = (DateTime?)value;
            return dp._calendar.SelectedDate;
        }

        #endregion SelectedDate

        #region SelectedDateFormat

        /// <summary>
        ///     选中日期的格式
        /// Gets or sets the format that is used to display the selected date.
        /// </summary>
        public DatePickerFormat SelectedDateFormat
        {
            get { return (DatePickerFormat)GetValue(SelectedDateFormatProperty); }
            set { SetValue(SelectedDateFormatProperty, value); }
        }

        /// <summary>
        ///     选中日期格式的依赖属性
        /// Identifies the SelectedDateFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateFormatProperty =
            DependencyProperty.Register(
                "SelectedDateFormat",
                typeof(DatePickerFormat),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(DatePickerFormat.Long, OnSelectedDateFormatChanged),
                IsValidSelectedDateFormat);

        /// <summary>
        ///     选中日期格式变化时的处理程序SelectedDateFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its SelectedDateFormat.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            if (dp._textBox != null)
                if (string.IsNullOrEmpty(dp._textBox.Text))
                {
                    dp.SetWaterMarkText();
                }
                else
                {
                    DateTime? date = dp.ParseText(dp._textBox.Text);

                    if (date != null)
                        dp.SetTextInternal(dp.DateTimeToString((DateTime)date));
                }
        }

        #endregion SelectedDateFormat

        #region Text

        /// <summary>
        ///     显示的日期文本
        /// Gets or sets the text that is displayed by the DatePicker.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///     显示日期文本的依赖属性Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(MyDatePicker),
                new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        /// <summary>
        ///     显示日期文本变化时的处理程序TextProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its Text.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            if (!dp.IsHandlerSuspended(MyDatePicker.TextProperty))
            {
                var newValue = e.NewValue as string;

                if (newValue != null)
                {
                    if (dp._textBox != null)
                        dp._textBox.Text = newValue;
                    else
                        dp._defaultText = newValue;

                    dp.SetSelectedDate();
                }
                else
                {
                    dp.SetValueNoCallback(MyDatePicker.SelectedDateProperty, null);
                }
            }
        }


        /// <summary>
        ///     内部文本
        /// Sets the local Text property without breaking bindings
        /// </summary>
        /// <param name="value"></param>
        private void SetTextInternal(string value)
        {
            SetCurrentValueInternal(TextProperty, value);
        }

        #endregion Text

        #endregion Public Properties

        MyDatePicker()
        {
            

        }

        #region 私有方法

        private string DateTimeToString(DateTime d)
        {
            var dtfi = MyDateTimeHelper.GetDateFormat(MyDateTimeHelper.GetCulture(this));

            switch (SelectedDateFormat)
            {
                case DatePickerFormat.Short:
                    {
                        return string.Format(CultureInfo.CurrentCulture, d.ToString(dtfi.ShortDatePattern, dtfi));
                    }

                case DatePickerFormat.Long:
                    {
                        return string.Format(CultureInfo.CurrentCulture, d.ToString(dtfi.LongDatePattern, dtfi));
                    }
            }

            return null;
        }

        #endregion
    }
}
