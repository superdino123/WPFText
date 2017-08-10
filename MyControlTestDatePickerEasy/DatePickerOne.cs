using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyControlTestDatePickerEasy
{
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementHourPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinutePicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementAmPmSwitcher, Type = typeof(Selector))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(DatePickerTextBox))]
    public class DatePickerOne : Control
    {
        #region 静态构造函数

        static DatePickerOne()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePickerOne), new FrameworkPropertyMetadata(typeof(DatePickerOne)));
        }

        #endregion

        #region 私有变量

        private static IList<int> CreateValueList(int interval)
        {
            return Enumerable.Repeat(interval, 60 / interval)
                .Select((value, index) => value * index)
                .ToList();
        }

        #endregion

        #region 公共方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild(ElementPopup) as Popup;
            _button = GetTemplateChild(ElementButton) as Button;
            _hourInput = GetTemplateChild(ElementHourPicker) as Selector;
            _minuteInput = GetTemplateChild(ElementMinutePicker) as Selector;
            _secondInput = GetTemplateChild(ElementSecondPicker) as Selector;
            _hourHand = GetTemplateChild(ElementHourHand) as FrameworkElement;
            _ampmSwitcher = GetTemplateChild(ElementAmPmSwitcher) as Selector;
            _minuteHand = GetTemplateChild(ElementMinuteHand) as FrameworkElement;
            _secondHand = GetTemplateChild(ElementSecondHand) as FrameworkElement;
            _textBox = GetTemplateChild(ElementTextBox) as DatePickerTextBox;
        }

        #endregion

        #region 依赖属性

        #region SourceHours

        public IEnumerable<int> SourceHours
        {
            get { return (IEnumerable<int>) GetValue(SourceHoursProperty); }
            set { SetValue(SourceHoursProperty, value); }
        }

        public static readonly DependencyProperty SourceHoursProperty =
            DependencyProperty.Register("SourceHours", typeof(IEnumerable<int>), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region SourceMinutes

        public IEnumerable<int> SourceMinutes
        {
            get { return (IEnumerable<int>) GetValue(SourceMinutesProperty); }
            set { SetValue(SourceMinutesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceMinutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceMinutesProperty =
            DependencyProperty.Register("SourceMinutes", typeof(IEnumerable<int>), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region SourceSeconds

        public IEnumerable<int> SourceSeconds
        {
            get { return (IEnumerable<int>) GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceSeconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceSecondsProperty =
            DependencyProperty.Register("SourceSeconds", typeof(IEnumerable<int>), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region IsDropDownOpen

        /*public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }*/

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty = DatePicker.IsDropDownOpenProperty.AddOwner(typeof(DatePickerOne), new PropertyMetadata(default(bool)));

        #endregion

        #region IsClockVisible

        public bool IsClockVisible
        {
            get { return (bool) GetValue(IsClockVisibleProperty); }
            set { SetValue(IsClockVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsClockVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClockVisibleProperty =
            DependencyProperty.Register("IsClockVisible", typeof(bool), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region Is24Hour

        public int? Is24Hour
        {
            get { return (int?) GetValue(Is24HourProperty); }
            set { SetValue(Is24HourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Is24Hour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Is24HourProperty =
            DependencyProperty.Register("Is24Hour", typeof(int?), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region TimeFormat

        public string TimeFormat
        {
            get { return (string) GetValue(TimeFormatProperty); }
            set { SetValue(TimeFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeFormatProperty =
            DependencyProperty.Register("TimeFormat", typeof(string), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region HandVisibility

        public TimePartVisibility HandVisibility
        {
            get { return (TimePartVisibility) GetValue(HandVisibilityProperty); }
            set { SetValue(HandVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HandVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandVisibilityProperty =
            DependencyProperty.Register("HandVisibility", typeof(TimePartVisibility), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region Culture

        public CultureInfo Culture
        {
            get { return (CultureInfo) GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Culture.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region PickerVisibility

        public TimePartVisibility PickerVisibility
        {
            get { return (TimePartVisibility) GetValue(PickerVisibilityProperty); }
            set { SetValue(PickerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PickerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PickerVisibilityProperty =
            DependencyProperty.Register("PickerVisibility", typeof(TimePartVisibility), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region SelectedTime

        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?) GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(TimeSpan?), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #region Text

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DatePickerOne), new PropertyMetadata(0));

        #endregion

        #endregion

        #region 组件名

        private const string ElementAmPmSwitcher = "PART_AmPmSwitcher";
        private const string ElementButton = "PART_Button";
        private const string ElementHourHand = "PART_HourHand";
        private const string ElementHourPicker = "PART_HourPicker";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementMinutePicker = "PART_MinutePicker";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSecondHand = "PART_SecondHand";
        private const string ElementSecondPicker = "PART_SecondPicker";
        private const string ElementTextBox = "PART_TextBox";

        #endregion

        #region 特殊的依赖属性

        private static readonly DependencyPropertyKey IsDatePickerVisiblePropertyKey = DependencyProperty.RegisterReadOnly(
            "IsDatePickerVisible", typeof(bool), typeof(DatePickerOne), new PropertyMetadata(true));

        public static readonly DependencyProperty IsDatePickerVisibleProperty = IsDatePickerVisiblePropertyKey.DependencyProperty;

        #endregion

        #region 私有变量

        private static readonly TimeSpan MinTimeOfDay = TimeSpan.Zero;
        private static readonly TimeSpan MaxTimeOfDay = TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
        private Selector _ampmSwitcher;
        private Button _button;
        private bool _deactivateRangeBaseEvent;
        private bool _deactivateTextChangedEvent;
        private bool _textInputChanged;
        private UIElement _hourHand;
        private Selector _hourInput;
        private UIElement _minuteHand;
        private Selector _minuteInput;
        private Popup _popup;
        private UIElement _secondHand;
        private Selector _secondInput;
        protected DatePickerTextBox _textBox;

        #endregion

        #region 公共变量

        public static readonly IEnumerable<int> IntervalOf5 = CreateValueList(5);
        public static readonly IEnumerable<int> IntervalOf10 = CreateValueList(10);
        public static readonly IEnumerable<int> IntervalOf15 = CreateValueList(15);

        #endregion
    }
}