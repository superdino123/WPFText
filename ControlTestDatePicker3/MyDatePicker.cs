using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Calendar = System.Windows.Controls.Calendar;

namespace ControlTestDatePicker3
{
    [TemplatePart(Name = ElementRoot, Type = typeof(Grid))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    public class MyDatePicker : Control
    {
        /// <summary>
        ///     Static constructor
        /// </summary>
        static MyDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));
            EventManager.RegisterClassHandler(typeof(DatePicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(false));
            IsEnabledProperty.OverrideMetadata(typeof(DatePicker), new UIPropertyMetadata(OnIsEnabledChanged));

            ControlsTraceLogger.AddControl(TelemetryControls.DatePicker);
        }

        /// <summary>
        ///     Initializes a new instance of the DatePicker class.
        /// </summary>
        public MyDatePicker()
        {
            InitializeCalendar();
            _defaultText = string.Empty;

            this.SetCurrentValueInternal(FirstDayOfWeekProperty, DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);
            this.SetCurrentValueInternal(DisplayDateProperty, DateTime.Today);
        }

        #region Constants常量

        /// <summary>
        ///     根控件name
        /// </summary>
        private const string ElementRoot = "PART_Root";

        /// <summary>
        ///     输入控件name
        /// </summary>
        private const string ElementTextBox = "PART_TextBox";

        /// <summary>
        ///     按钮name
        /// </summary>
        private const string ElementButton = "PART_Button";

        /// <summary>
        ///     弹出窗口name
        /// </summary>
        private const string ElementPopup = "PART_Popup";

        #endregion Constants

        #region Data数据

        /// <summary>
        ///     日历控件，动态添加进去
        /// </summary>
        private Calendar _calendar;

        /// <summary>
        ///     默认输入框文字
        /// </summary>
        private string _defaultText;

        /// <summary>
        ///     点击按钮
        /// </summary>
        private ButtonBase _dropDownButton;

        /// <summary>
        ///     弹出窗口
        /// </summary>
        private Popup _popUp;

        /// <summary>
        ///     是否弹出弹出窗口
        /// </summary>
        private bool _disablePopupReopen;

        /// <summary>
        ///     输入框
        /// </summary>
        private DatePickerTextBox _textBox;

        /// <summary>
        ///     该处理程序是否暂停
        /// </summary>
        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        /// <summary>
        ///     当前选择的时间
        /// </summary>
        private DateTime? _originalSelectedDate;

        #endregion Data

        #region Public Events
        /// <summary>
        /// 创建路由事件变量(选择数据改变事件委托)
        /// </summary>
        public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent("SelectedDateChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(DatePicker));

        /// <summary>
        ///     日历关闭时发生
        /// Occurs when the drop-down Calendar is closed.
        /// </summary>
        public event RoutedEventHandler CalendarClosed;

        /// <summary>
        ///     日历打开时发生
        /// Occurs when the drop-down Calendar is opened.
        /// </summary>
        public event RoutedEventHandler CalendarOpened;

        /// <summary>
        ///     文本错误解析时发生
        /// Occurs when text entered into the DatePicker cannot be parsed or the Date is not valid to be selected.
        /// </summary>
        public event EventHandler<DatePickerDateValidationErrorEventArgs> DateValidationError;

        /// <summary>
        ///     当日期被改变时发生(路由事件包装器，包装成事件)
        /// Occurs when a date is selected.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        #endregion Public Events

        #region Public properties

        #region BlackoutDates

        /// <summary>
        ///     Gets the days that are not selectable.
        /// </summary>
        public CalendarBlackoutDatesCollection BlackoutDates
        {
            get { return _calendar.BlackoutDates; }
        }

        #endregion BlackoutDates

        #region CalendarStyle

        /// <summary>
        ///     返回不能选择的日期
        /// Gets or sets the style that is used when rendering the calendar.
        /// </summary>
        public Style CalendarStyle
        {
            get { return (Style) GetValue(CalendarStyleProperty); }
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
                typeof(DatePicker));

        #endregion CalendarStyle

        #region DisplayDate

        /// <summary>
        ///     显示的日期
        /// Gets or sets the date to display.
        /// </summary>
        public DateTime DisplayDate
        {
            get { return (DateTime) GetValue(DisplayDateProperty); }
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
                typeof(DatePicker),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayDate));

        /// <summary>
        /// 强制指定显示时间
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDate in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDate = (DateTime) value;
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
            get { return (DateTime?) GetValue(DisplayDateEndProperty); }
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
                typeof(DatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateEndChanged, CoerceDisplayDateEnd));

        /// <summary>
        ///     显示时间最大值改变时的处理程序
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
        /// 强制指定显示时间最大值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDateEnd in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDateEnd = (DateTime?) value;
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
            get { return (DateTime?) GetValue(DisplayDateStartProperty); }
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
        ///     显示时间最小值的依赖属性发生改变时的处理程序
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
        /// 强制显示时间最小值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            var dp = d as MyDatePicker;

            // We set _calendar.DisplayDateStart in order to get _calendar to compute the coerced value
            dp._calendar.DisplayDateStart = (DateTime?) value;
            return dp._calendar.DisplayDateStart;
        }

        #endregion DisplayDateStart

        #region FirstDayOfWeek

        /// <summary>
        ///     哪一天是一周的第一天
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek) GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        ///     一周第一天的依赖属性
        /// Identifies the FirstDayOfWeek dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
                "FirstDayOfWeek",
                typeof(DayOfWeek),
                typeof(MyDatePicker),
                null,
                Calendar.IsValidFirstDayOfWeek);

        #endregion FirstDayOfWeek

        #region IsDropDownOpen

        /// <summary>
        ///     日期控件是否显示
        /// Gets or sets a value that indicates whether the drop-down Calendar is open or closed.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool) GetValue(IsDropDownOpenProperty); }
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
        /// 强制日期框控件是否显示
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            var dp = d as DatePicker;
            Debug.Assert(dp != null);

            if (!dp.IsEnabled)
                return false;

            return baseValue;
        }

        /// <summary>
        ///     日期控件是否打开的状态变化时的处理程序
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its IsDropDownOpen.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            var newValue = (bool) e.NewValue;
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

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) delegate
                    {
                        // setting the focus to the calendar will focus the correct date.
                        dp._calendar.Focus();
                    });
                }
            }
        }

        /// <summary>
        /// 控件是否能用属性改变时的处理程序
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MyDatePicker;
            Debug.Assert(dp != null);

            dp.CoerceValue(IsDropDownOpenProperty);

            OnVisualStatePropertyChanged(d, e);
        }

        #endregion IsDropDownOpen

        #region IsTodayHighlighted

        /// <summary>
        ///     选中日期是否突出显示
        /// Gets or sets a value that indicates whether the current date will be highlighted.
        /// </summary>
        public bool IsTodayHighlighted
        {
            get { return (bool) GetValue(IsTodayHighlightedProperty); }
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
                typeof(DatePicker));

        #endregion IsTodayHighlighted

        #region SelectedDate

        /// <summary>
        ///     当前选中的日期
        /// Gets or sets the currently selected date.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?) GetValue(SelectedDateProperty); }
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
        ///     选中日期改变时的处理程序
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

            addedDate = (DateTime?) e.NewValue;
            removedDate = (DateTime?) e.OldValue;

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
            dp._calendar.SelectedDate = (DateTime?) value;
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
            get { return (DatePickerFormat) GetValue(SelectedDateFormatProperty); }
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
                        dp.SetTextInternal(dp.DateTimeToString((DateTime) date));
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
            get { return (string) GetValue(TextProperty); }
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

        #region Protected properties

        #endregion Protected Properties

        #region Internal Properties

        /// <summary>
        /// 日期控件
        /// </summary>
        internal Calendar Calendar
        {
            get { return _calendar; }
        }
        /// <summary>
        /// 输入框控件
        /// </summary>
        internal TextBox TextBox
        {
            get { return _textBox; }
        }

        #endregion Internal Properties

        #region Private Properties

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        ///     当应用新的模板时，创建日期输入框控件的可视化树
        /// Builds the visual tree for the DatePicker control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_popUp != null)
            {
                _popUp.RemoveHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));
                _popUp.Opened -= PopUp_Opened;
                _popUp.Closed -= PopUp_Closed;
                _popUp.Child = null;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
                _dropDownButton.RemoveHandler(MouseLeaveEvent, new MouseEventHandler(DropDownButton_MouseLeave));
            }

            if (_textBox != null)
            {
                _textBox.RemoveHandler(KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
                _textBox.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBox_TextChanged));
                _textBox.RemoveHandler(LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
            }

            base.OnApplyTemplate();

            _popUp = GetTemplateChild(ElementPopup) as Popup;

            if (_popUp != null)
            {
                _popUp.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));
                _popUp.Opened += PopUp_Opened;
                _popUp.Closed += PopUp_Closed;
                _popUp.Child = _calendar;

                if (IsDropDownOpen)
                    _popUp.IsOpen = true;
            }

            _dropDownButton = GetTemplateChild(ElementButton) as Button;
            if (_dropDownButton != null)
            {
                _dropDownButton.Click += DropDownButton_Click;
                _dropDownButton.AddHandler(MouseLeaveEvent, new MouseEventHandler(DropDownButton_MouseLeave), true);

                // If the user does not provide a Content value in template, we provide a helper text that can be used in Accessibility
                // this text is not shown on the UI, just used for Accessibility purposes
                if (_dropDownButton.Content == null)
                    _dropDownButton.Content = System.Windows.SR.Get(SRID.DatePicker_DropDownButtonName);
            }

            _textBox = GetTemplateChild(ElementTextBox) as DatePickerTextBox;

            if (SelectedDate == null)
                SetWaterMarkText();

            if (_textBox != null)
            {
                _textBox.AddHandler(KeyDownEvent, new KeyEventHandler(TextBox_KeyDown), true);
                _textBox.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBox_TextChanged), true);
                _textBox.AddHandler(LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus), true);

                if (SelectedDate == null)
                {
                    if (!string.IsNullOrEmpty(_defaultText))
                    {
                        _textBox.Text = _defaultText;
                        SetSelectedDate();
                    }
                }
                else
                {
                    _textBox.Text = DateTimeToString((DateTime) SelectedDate);
                }
            }
        }

        /// <summary>
        ///     Provides a text representation of the selected date.
        /// </summary>
        /// <returns>A text representation of the selected date, or an empty string if SelectedDate is a null reference.</returns>
        public override string ToString()
        {
            if (SelectedDate != null)
                return SelectedDate.Value.ToString(DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)));
            return string.Empty;
        }

        #endregion Public Methods

        #region Protected Methods

        internal override void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            else
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);

            base.ChangeVisualState(useTransitions);
        }

        /// <summary>
        ///     Creates the automation peer for this DatePicker Control.
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DatePickerAutomationPeer(this);
        }

        protected virtual void OnCalendarClosed(RoutedEventArgs e)
        {
            var handler = CalendarClosed;
            if (null != handler)
                handler(this, e);
        }

        protected virtual void OnCalendarOpened(RoutedEventArgs e)
        {
            var handler = CalendarOpened;
            if (null != handler)
                handler(this, e);
        }

        protected virtual void OnSelectedDateChanged(SelectionChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        ///     Raises the DateValidationError event.
        /// </summary>
        /// <param name="e">A DatePickerDateValidationErrorEventArgs that contains the event data.</param>
        protected virtual void OnDateValidationError(DatePickerDateValidationErrorEventArgs e)
        {
            var handler = DateValidationError;
            if (handler != null)
                handler(this, e);
        }

        protected internal override bool HasEffectiveKeyboardFocus
        {
            get
            {
                if (_textBox != null)
                    return _textBox.HasEffectiveKeyboardFocus;
                return base.HasEffectiveKeyboardFocus;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Datepicker gets focus move it to the TextBox
            var picker = (DatePicker) sender;
            if (!e.Handled && picker._textBox != null)
                if (e.OriginalSource == picker)
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (e.OriginalSource == picker._textBox)
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
        }

        private void SetValueNoCallback(DependencyProperty property, object value)
        {
            SetIsHandlerSuspended(property, true);
            try
            {
                SetCurrentValue(property, value);
            }
            finally
            {
                SetIsHandlerSuspended(property, false);
            }
        }

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (_isHandlerSuspended == null)
                    _isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);

                _isHandlerSuspended[property] = true;
            }
            else
            {
                if (_isHandlerSuspended != null)
                    _isHandlerSuspended.Remove(property);
            }
        }

        private void PopUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var popup = sender as Popup;
            if (popup != null && !popup.StaysOpen)
                if (_dropDownButton != null)
                    if (_dropDownButton.InputHitTest(e.GetPosition(_dropDownButton)) != null)
                        _disablePopupReopen = true;
        }

        private void PopUp_Opened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
                this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.TrueBox);

            if (_calendar != null)
            {
                _calendar.DisplayMode = CalendarMode.Month;
                _calendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            OnCalendarOpened(new RoutedEventArgs());
        }

        private void PopUp_Closed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
                this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.FalseBox);

            if (_calendar.IsKeyboardFocusWithin)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            OnCalendarClosed(new RoutedEventArgs());
        }

        private void Calendar_DayButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
        }

        private void CalendarDayOrMonthButton_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            var c = sender as Calendar;
            var args = (KeyEventArgs) e;

            Debug.Assert(c != null);
            Debug.Assert(args != null);

            if (args.Key == Key.Escape || (args.Key == Key.Enter || args.Key == Key.Space) && c.DisplayMode == CalendarMode.Month)
            {
                this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
                if (args.Key == Key.Escape)
                    SetCurrentValueInternal(SelectedDateProperty, _originalSelectedDate);
            }
        }

        private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (e.AddedDate != DisplayDate)
                SetCurrentValueInternal(DisplayDateProperty, (DateTime) e.AddedDate);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(e.AddedItems.Count < 2);

            if (e.AddedItems.Count > 0 && SelectedDate.HasValue && DateTime.Compare((DateTime) e.AddedItems[0], SelectedDate.Value) != 0)
            {
                this.SetCurrentValueInternal(SelectedDateProperty, (DateTime?) e.AddedItems[0]);
            }
            else
            {
                if (e.AddedItems.Count == 0)
                {
                    this.SetCurrentValueInternal(SelectedDateProperty, (DateTime?) null);
                    return;
                }

                if (!SelectedDate.HasValue)
                    if (e.AddedItems.Count > 0)
                        this.SetCurrentValueInternal(SelectedDateProperty, (DateTime?) e.AddedItems[0]);
            }
        }

        private string DateTimeToString(DateTime d)
        {
            var dtfi = DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this));

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

        private static DateTime DiscardDayTime(DateTime d)
        {
            var year = d.Year;
            var month = d.Month;
            var newD = new DateTime(year, month, 1, 0, 0, 0);
            return newD;
        }

        private static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
                return null;
            var discarded = (DateTime) d;
            var year = discarded.Year;
            var month = discarded.Month;
            var day = discarded.Day;
            var newD = new DateTime(year, month, day, 0, 0, 0);
            return newD;
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePopUp();
        }

        private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
        {
            _disablePopupReopen = false;
        }

        private void TogglePopUp()
        {
            if (IsDropDownOpen)
            {
                this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    SetSelectedDate();
                    this.SetCurrentValueInternal(IsDropDownOpenProperty, BooleanBoxes.TrueBox);
                }
            }
        }

        private void InitializeCalendar()
        {
            _calendar = new Calendar();
            _calendar.DayButtonMouseUp += Calendar_DayButtonMouseUp;
            _calendar.DisplayDateChanged += Calendar_DisplayDateChanged;
            _calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
            _calendar.DayOrMonthPreviewKeyDown += CalendarDayOrMonthButton_PreviewKeyDown;
            _calendar.HorizontalAlignment = HorizontalAlignment.Left;
            _calendar.VerticalAlignment = VerticalAlignment.Top;

            _calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            _calendar.SetBinding(ForegroundProperty, GetDatePickerBinding(ForegroundProperty));
            _calendar.SetBinding(StyleProperty, GetDatePickerBinding(DatePicker.CalendarStyleProperty));
            _calendar.SetBinding(Calendar.IsTodayHighlightedProperty, GetDatePickerBinding(DatePicker.IsTodayHighlightedProperty));
            _calendar.SetBinding(Calendar.FirstDayOfWeekProperty, GetDatePickerBinding(DatePicker.FirstDayOfWeekProperty));
            _calendar.SetBinding(FlowDirectionProperty, GetDatePickerBinding(FlowDirectionProperty));

            RenderOptions.SetClearTypeHint(_calendar, ClearTypeHint.Enabled);
        }

        private BindingBase GetDatePickerBinding(DependencyProperty property)
        {
            var binding = new Binding(property.Name);
            binding.Source = this;
            return binding;
        }

        private static bool IsValidSelectedDateFormat(object value)
        {
            var format = (DatePickerFormat) value;

            return format == DatePickerFormat.Long
                   || format == DatePickerFormat.Short;
        }

        // iT SHOULD RETURN NULL IF THE STRING IS NOT VALID, RETURN THE DATETIME VALUE IF IT IS VALID

        /// <summary>
        ///     Input text is parsed in the correct format and changed into a DateTime object.
        ///     If the text can not be parsed TextParseError Event is thrown.
        /// </summary>
        private DateTime? ParseText(string text)
        {
            DateTime newSelectedDate;

            // TryParse is not used in order to be able to pass the exception to the TextParseError event
            try
            {
                newSelectedDate = DateTime.Parse(text, DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)));

                if (Calendar.IsValidDateSelection(_calendar, newSelectedDate))
                    return newSelectedDate;
                var dateValidationError = new DatePickerDateValidationErrorEventArgs(new ArgumentOutOfRangeException("text", System.Windows.SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue)), text);
                OnDateValidationError(dateValidationError);

                if (dateValidationError.ThrowException)
                    throw dateValidationError.Exception;
            }
            catch (FormatException ex)
            {
                var textParseError = new DatePickerDateValidationErrorEventArgs(ex, text);
                OnDateValidationError(textParseError);

                if (textParseError.ThrowException && textParseError.Exception != null)
                    throw textParseError.Exception;
            }

            return null;
        }

        private bool ProcessDatePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System:
                {
                    switch (e.SystemKey)
                    {
                        case Key.Down:
                        {
                            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                            {
                                TogglePopUp();
                                return true;
                            }

                            break;
                        }
                    }

                    break;
                }

                case Key.Enter:
                {
                    SetSelectedDate();
                    return true;
                }
            }

            return false;
        }

        private void SetSelectedDate()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    var s = _textBox.Text;

                    if (SelectedDate != null)
                    {
                        // If the string value of the SelectedDate and the TextBox string value are equal,
                        // we do not parse the string again
                        // if we do an extra parse, we lose data in M/d/yy format
                        // ex: SelectedDate = DateTime(1008,12,19) but when "12/19/08" is parsed it is interpreted as DateTime(2008,12,19)
                        var selectedDate = DateTimeToString(SelectedDate.Value);

                        if (string.Compare(selectedDate, s, StringComparison.Ordinal) == 0)
                            return;
                    }

                    var d = SetTextBoxValue(s);
                    if (!SelectedDate.Equals(d))
                    {
                        this.SetCurrentValueInternal(SelectedDateProperty, d);
                        this.SetCurrentValueInternal(DisplayDateProperty, d);
                    }
                }
                else
                {
                    if (SelectedDate.HasValue)
                        this.SetCurrentValueInternal(SelectedDateProperty, (DateTime?) null);
                }
            }
            else
            {
                var d = SetTextBoxValue(_defaultText);
                if (!SelectedDate.Equals(d))
                    this.SetCurrentValueInternal(SelectedDateProperty, d);
            }
        }

        /// <summary>
        ///     Set the Text property if it's not already set to the supplied value.  This avoids making the ValueSource Local.
        /// </summary>
        private void SafeSetText(string s)
        {
            if (string.Compare(Text, s, StringComparison.Ordinal) != 0)
                SetCurrentValueInternal(TextProperty, s);
        }

        private DateTime? SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SafeSetText(s);
                return SelectedDate;
            }
            var d = ParseText(s);

            if (d != null)
            {
                SafeSetText(DateTimeToString((DateTime) d));
                return d;
            }
            // If parse error:
            // TextBox should have the latest valid selecteddate value:
            if (SelectedDate != null)
            {
                var newtext = DateTimeToString((DateTime) SelectedDate);
                SafeSetText(newtext);
                return SelectedDate;
            }
            SetWaterMarkText();
            return null;
        }

        private void SetWaterMarkText()
        {
            if (_textBox != null)
            {
                var dtfi = DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this));
                SetTextInternal(string.Empty);
                _defaultText = string.Empty;

                switch (SelectedDateFormat)
                {
                    case DatePickerFormat.Long:
                    {
                        _textBox.Watermark = string.Format(CultureInfo.CurrentCulture, System.Windows.SR.Get(SRID.DatePicker_WatermarkText), dtfi.LongDatePattern);
                        break;
                    }

                    case DatePickerFormat.Short:
                    {
                        _textBox.Watermark = string.Format(CultureInfo.CurrentCulture, System.Windows.SR.Get(SRID.DatePicker_WatermarkText), dtfi.ShortDatePattern);
                        break;
                    }
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedDate();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessDatePickerKey(e) || e.Handled;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueNoCallback(DatePicker.TextProperty, _textBox.Text);
        }

        #endregion Private Methods
    }
}