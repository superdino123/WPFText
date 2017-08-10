using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MyControlTestDateTimePickerTwo
{
    internal class DateTimePicker : Control
    {
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";
        private const string ElementDatePickerTextBox = "PART_DatePickerTextBox";
        private const string ElementCalendar = "PART_Calendar";

        public static readonly DependencyProperty SelectedDateProperty = DatePicker.SelectedDateProperty.AddOwner(typeof(DateTimePicker));
        private Button _button;
        private Calendar _calendar;
        private DatePickerTextBox _datePickerTextBox;
        private Popup _popup;

        static DateTimePicker()
        {
            #region 应用默认主题样式

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));

            #endregion
        }

        public override void OnApplyTemplate()
        {
            _button = (Button) GetTemplateChild(ElementButton);
            _button.Click += ButtonClick;

            _datePickerTextBox = (DatePickerTextBox) GetTemplateChild(ElementDatePickerTextBox);
            _datePickerTextBox.MouseDown += DatePickerTextBoxMouseDown;
            _datePickerTextBox.KeyDown += DatePickerTextBoxKeyDown;

            _popup = (Popup) GetTemplateChild(ElementPopup);
            _popup.PlacementTarget = _datePickerTextBox;

            _calendar = (Calendar) GetTemplateChild(ElementCalendar);
            _calendar.DisplayDateChanged += CalendarDisplayDateChanged;
            _calendar.SelectedDatesChanged += CalendarSelectedDatesChanged;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = true;
        }


        private void CalendarSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _popup.IsOpen = false;
            _datePickerTextBox.Text = _calendar.SelectedDate.ToString();
            _datePickerTextBox.Focus();
            _datePickerTextBox.Select(0,4);
        }

        private void CalendarDisplayDateChanged(object sender, CalendarDateChangedEventArgs e) {}


        private void DatePickerTextBoxMouseDown(object sender, MouseButtonEventArgs e) {}

        private void DatePickerTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Up)
            {
                _datePickerTextBox.Focus();
                _datePickerTextBox.Select(6,7);
            }
        }
    }
}