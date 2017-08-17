using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyControlTestTimePicker
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
    public class TimePicker : Control
    {
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

        static TimePicker() {
            SelectedTimeProperty = null;
           
        }

        public TimePicker() {

        }



        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(TimeSpan?), typeof(TimePicker), new PropertyMetadata(null
                ));



        public IEnumerable<int> SourceHours
        {
            get { return (IEnumerable<int>)GetValue(SourceHoursProperty); }
            set { SetValue(SourceHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceHours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceHoursProperty =
            DependencyProperty.Register("SourceHours", typeof(IEnumerable<int>), typeof(TimePicker), new PropertyMetadata(Enumerable.Range(0,24)));



        public IEnumerable<int> SourceMinutes
        {
            get { return (IEnumerable<int>)GetValue(SourceMinutesProperty); }
            set { SetValue(SourceMinutesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceMinutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceMinutesProperty =
            DependencyProperty.Register("SourceMinutes", typeof(IEnumerable<int>), typeof(TimePicker), new PropertyMetadata(Enumerable.Range(0,60)));



        public IEnumerable<int> SourceSeconds
        {
            get { return (IEnumerable<int>)GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceSeconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceSecondsProperty =
            DependencyProperty.Register("SourceSeconds", typeof(IEnumerable<int>), typeof(TimePicker), new PropertyMetadata(Enumerable.Range(0,60)));


    }
}