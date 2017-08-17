using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using MyControlTestLastDateTimePicker.Converter;

namespace MyControlTestLastDateTimePicker.DateTimePickerCs
{
    //扩展功能
    //自定义属性DateTimePickerType标识 DateTime(显示日期时间) Date(显示时间)
    public class DateTimePicker : Control
    {
        private const string ElementDateTimeTextBox = "PART_DatePickerTextBox";
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";
        private const string ElementCalendar = "PART_Calendar";
        private const string ElementHourComboBox = "PART_HourComboBox";
        private const string ElementMinuteComboBox = "PART_MinuteComboBox";
        private const string ElementSecondComboBox = "PART_SecondComboBox";
        private const string ElementAmPmSwithcerComboBox = "PART_AmPmSwitcherComboBox";
        private ComboBox ampmComboBox;
        private Button button;
        private Calendar calendar;
        private DatePickerTextBox datePickerTextBox;
        private ComboBox hourComboBox;
        private ComboBox minuteComboBox;
        private Popup popup;
        private ComboBox secondComboBox;

        static DateTimePicker()
        {
            for (var i = 0; i <= 11; i++)
            {
                hourSource.Add(i);
            }
            for (var i = 0; i <= 59; i++)
            {
                clockSource.Add(i);
            }
            ampmSource.Add(1, "上午");
            ampmSource.Add(2, "下午");
        }

        public DateTimePicker()
        {
            SelectedDateTime = DateTime.Now;
            UpdatePartOfDateTime();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            datePickerTextBox = GetTemplateChild(ElementDateTimeTextBox) as DatePickerTextBox;
            button = GetTemplateChild(ElementButton) as Button;
            popup = GetTemplateChild(ElementPopup) as Popup;
            calendar = GetTemplateChild(ElementCalendar) as Calendar;
            hourComboBox = GetTemplateChild(ElementHourComboBox) as ComboBox;
            minuteComboBox = GetTemplateChild(ElementMinuteComboBox) as ComboBox;
            secondComboBox = GetTemplateChild(ElementSecondComboBox) as ComboBox;
            ampmComboBox = GetTemplateChild(ElementAmPmSwithcerComboBox) as ComboBox;

            datePickerTextBox.GotMouseCapture += DatePickerTextBoxGotMouseCapture;
        }

        private void DatePickerTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(
                (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab ||
                (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C) ||
                (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V) ||
                (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A)
                ))
            {
                e.Handled = true;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) && datePickerTextBox.SelectedText.Equals(datePickerTextBox.Text))
            {
                selectCell = 1;
                UpdateStartAndEnd(selectCell, ref start, ref length);
                datePickerTextBox.Select(start, length);
            }
        }

        private void DatePickerTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Space || e.Key == Key.Left)
            {
                //当shift + right/left时，屏蔽right/left键功能
                if (!(e.KeyboardDevice.Modifiers == ModifierKeys.Shift && (e.Key == Key.Right || e.Key == Key.Left)))
                {
                    UpdateSelectedDateTime();
                    if (NoTime)
                    {
                        selectCell = e.Key == Key.Left ? (selectCell < 2 ? 3 : selectCell - 1) : (selectCell < 3 ? selectCell + 1 : 1);
                    }

                    else
                    {
                        selectCell = e.Key == Key.Left ? (selectCell < 2 ? 6 : selectCell - 1) : (selectCell < 6 ? selectCell + 1 : 1);
                    }
                }
                else
                {
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.Key == Key.Right)
                    {
                        length = length < datePickerTextBox.Text.Length - start + 1 ? length + 1 : length;
                    }
                    else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.Key == Key.Left)
                    {
                        length = length > 1 ? length - 1 : length;
                    }
                    datePickerTextBox.Select(start, length);
                }
            }
            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                UpdateSelectedDateTime();
                AddAndReduceDateTime(e.Key == Key.Up ? 1 : -1);
                UpdatePartOfDateTime();
            }
            if (!(e.Key == Key.Delete || e.Key == Key.Back ||
                  e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                  e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                  (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C) ||
                  (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A) ||
                  (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && (e.Key == Key.Right || e.Key == Key.Left))
                ))
            {
                UpdateSelectedDateTime();
                UpdateStartAndEnd(selectCell, ref start, ref length);
                datePickerTextBox.Select(start, length);
            }
            else if (e.Key == Key.Delete)
            {
                //保证可以在点击之后恢复空值，但实时更新使得多次删除效果不能产生。
                //使用Delete快捷键清空 事件，绑定，同时变更datePickerTextBox状态为不可编辑
                datePickerTextBox.IsReadOnly = true;

                #region 删除datePickerTextBox的绑定，以保证持续空值不受干扰

                BindingOperations.ClearBinding(datePickerTextBox, TextBox.TextProperty);

                #endregion

                #region 删除原有功能数据源和事件

                button.Click -= ButtonClick;
                calendar.SelectedDatesChanged -= CalendarSelectedDatesChanged;
                if (!NoTime)
                {
                    hourComboBox.SelectionChanged -= HourComboBoxSelectionChanged;
                    minuteComboBox.SelectionChanged -= MinuteComboBoxSelectionChanged;
                    secondComboBox.SelectionChanged -= SecondComboBoxSelectionChanged;
                    ampmComboBox.SelectionChanged -= AmPmComboBoxSelectionChanged;
                }

                #endregion

                #region 删除新加功能事件

                datePickerTextBox.KeyUp -= DatePickerTextBoxKeyUp; //方向与空格键不支持KeyDown，事件优先级不够，会被挂起
                datePickerTextBox.PreviewKeyDown -= DatePickerTextBoxPreviewKeyDown; //因为要彻底取消空格和字母的输入，因此采用PreviewKeyDown对可以支持KeyDown的按键的输入操作进行拦截

                #endregion
            }
        }

        private void DatePickerTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            //初始没有鼠标焦点时，datePickerTextBox为只读控件，鼠标点击后，变为可编辑
            datePickerTextBox.IsReadOnly = false;

            //初始状态，如果没有值，并且有鼠标点击，则添加所有事件和绑定
            if (string.IsNullOrEmpty(datePickerTextBox.Text))
            {
                #region 因为前期不能有值，因此需要将绑定移到后台代码动态添加(因是否显示时间分情况添加不同绑定)

                if (NoTime)
                {
                    var dateTextBoxNoTimeConverter = new DateTextBoxNoTimeConverter();
                    var datePickerTextBoxTextBinding = new Binding("SelectedDateTime") {Source = this, Converter = dateTextBoxNoTimeConverter, Mode = BindingMode.OneWay};
                    datePickerTextBox.SetBinding(TextBox.TextProperty, datePickerTextBoxTextBinding);
                }
                else
                {
                    var dateTextBoxConverter = new DateTextBoxConverter();
                    var datePickerTextBoxTextBinding = new Binding("SelectedDateTime") {Source = this, Converter = dateTextBoxConverter, Mode = BindingMode.OneWay};
                    datePickerTextBox.SetBinding(TextBox.TextProperty, datePickerTextBoxTextBinding);
                }

                #endregion

                #region 原有功能添加数据源和事件

                button.Click += ButtonClick;
                calendar.SelectedDatesChanged += CalendarSelectedDatesChanged;
                if (!NoTime)
                {
                    hourComboBox.ItemsSource = hourSource;
                    hourComboBox.SelectionChanged += HourComboBoxSelectionChanged;
                    minuteComboBox.ItemsSource = clockSource;
                    minuteComboBox.SelectionChanged += MinuteComboBoxSelectionChanged;
                    secondComboBox.ItemsSource = clockSource;
                    secondComboBox.SelectionChanged += SecondComboBoxSelectionChanged;
                    ampmComboBox.ItemsSource = ampmSource.Values;
                    ampmComboBox.SelectionChanged += AmPmComboBoxSelectionChanged;
                }

                #endregion

                #region 新加功能添加事件

                //输入的数字必须在KeyUp事件才能处理，因此，将取消特定按键的输入放在PreviewKeyDown,将所有的处理逻辑放在KeyUp
                datePickerTextBox.KeyUp += DatePickerTextBoxKeyUp; //方向与空格键不支持KeyDown，事件优先级不够，会被挂起
                datePickerTextBox.PreviewKeyDown += DatePickerTextBoxPreviewKeyDown; //因为要彻底取消空格和字母的输入，因此采用PreviewKeyDown对可以支持KeyDown的按键的输入操作进行拦截

                #endregion

                //默认点击后选中的是月份
                selectCell = 2;
            }
            else
            {
                selectCell = GetMouseCell(datePickerTextBox.CaretIndex);
            }
            UpdateStartAndEnd(selectCell, ref start, ref length);
            datePickerTextBox.Select(start, length);
        }

        #region 是否显示时分秒

        public bool NoTime
        {
            get { return (bool) GetValue(NoTimeProperty); }
            set { SetValue(NoTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoTimeProperty =
            DependencyProperty.Register("NoTime", typeof (bool), typeof (DateTimePicker), new PropertyMetadata(false));

        #endregion

        #region 选择的日期

        public DateTime SelectedDateTime
        {
            get { return (DateTime) GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof (DateTime), typeof (DateTimePicker), new PropertyMetadata(null));

        private int year;
        private int mouth;
        private int day;
        private int hour;
        private int minute;
        private int second;

        #endregion

        #region 时间选择框的数据源

        public static List<int> hourSource = new List<int>();
        public static List<int> clockSource = new List<int>();
        public static Dictionary<int, string> ampmSource = new Dictionary<int, string>();

        #endregion

        #region 时间块标识位

        private int selectCell = 2;
        // 1 year
        // 2 mouth
        // 3 day
        // 4 hour
        // 5 minute
        // 6 second

        private int start;
        private int length;

        #endregion

        #region 原有功能事件处理程序

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void CalendarSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var Calendar = (Calendar) sender;
            var dateTime = (DateTime) Calendar.SelectedDate;
            SelectedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second);
        }

        private void HourComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var HourComboBox = (ComboBox) sender;
            UpdatePartOfDateTime();
            hour = hour < 12 ? (int) HourComboBox.SelectedItem : (int) HourComboBox.SelectedItem + 12;
            SelectedDateTime = new DateTime(year, mouth, day, hour, minute, second);
        }

        private void MinuteComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var MinuteComboBox = (ComboBox) sender;
            minute = (int) MinuteComboBox.SelectedItem;
            SelectedDateTime = new DateTime(year, mouth, day, hour, minute, second);
        }

        private void SecondComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SecondComboBox = (ComboBox) sender;
            second = (int) SecondComboBox.SelectedItem;
            SelectedDateTime = new DateTime(year, mouth, day, hour, minute, second);
        }

        private void AmPmComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var AmPmComboBox = (ComboBox) sender;
            UpdatePartOfDateTime();
            if ((AmPmComboBox.SelectedItem.Equals("上午") && hour > 11) || (AmPmComboBox.SelectedItem.Equals("下午") && hour < 12))
            {
                hour = hour > 11 ? hour - 12 : hour + 12;
                SelectedDateTime = new DateTime(year, mouth, day, hour, minute, second);
            }
        }

        #endregion

        #region 私有方法 

        //获取start与end的方法
        //增加与减少时间的方法
        //根据TextBox更改SelectedDateTime的值
        //更新日期时间各个块
        //获取当前鼠标所在块

        /// <summary>
        ///     获取start与end的方法
        /// </summary>
        /// <param name="SelectCell">当前选择块</param>
        /// <param name="start">选择开始位置</param>
        /// <param name="length">选择的字符长度</param>
        private void UpdateStartAndEnd(int SelectCell, ref int start, ref int length)
        {
            var dateTime = datePickerTextBox.Text;
            switch (SelectCell)
            {
                //2017/1/12 1:12:12
                case 1: //year
                    start = 0;
                    length = dateTime.IndexOf('/');
                    break;
                case 2: //mouth
                    start = dateTime.IndexOf('/') + 1;
                    length = dateTime.LastIndexOf('/') - dateTime.IndexOf('/') - 1;
                    break;
                case 3: //day
                    start = dateTime.LastIndexOf('/') + 1;
                    length = NoTime ? dateTime.Length - dateTime.LastIndexOf('/') - 1 : dateTime.IndexOf(' ') - dateTime.LastIndexOf('/') - 1;
                    break;
                case 4: //hour
                    start = dateTime.IndexOf(' ') + 1;
                    length = dateTime.IndexOf(':') - dateTime.IndexOf(' ') - 1;
                    break;
                case 5: //minute
                    start = dateTime.IndexOf(':') + 1;
                    length = dateTime.LastIndexOf(':') - dateTime.IndexOf(':') - 1;
                    break;
                case 6: //second
                    start = dateTime.LastIndexOf(':') + 1;
                    length = dateTime.Length - dateTime.LastIndexOf(':') - 1;
                    break;
            }
        }

        /// <summary>
        ///     增加与减少时间的方法
        /// </summary>
        /// <param name="kind">1为增加时间，-1为减少时间</param>
        private void AddAndReduceDateTime(int kind)
        {
            //kind Up键为1,Down键为-1
            switch (selectCell)
            {
                case 1:
                    SelectedDateTime = SelectedDateTime.AddYears(kind);
                    break;
                case 2:
                    SelectedDateTime = SelectedDateTime.AddMonths(kind);
                    break;
                case 3:
                    SelectedDateTime = SelectedDateTime.AddDays(kind);
                    break;
                case 4:
                    SelectedDateTime = SelectedDateTime.AddHours(kind);
                    break;
                case 5:
                    SelectedDateTime = SelectedDateTime.AddMinutes(kind);
                    break;
                case 6:
                    SelectedDateTime = SelectedDateTime.AddSeconds(kind);
                    break;
            }
        }

        /// <summary>
        ///     根据TextBox更改SelectedDateTime的值
        /// </summary>
        private void UpdateSelectedDateTime()
        {
            var datePickerTextBoxStr = datePickerTextBox.Text;
            try
            {
                SelectedDateTime = DateTime.Parse(datePickerTextBoxStr);
                UpdatePartOfDateTime();
                SelectedDateTime = SelectedDateTime.AddDays(1);
                SelectedDateTime = SelectedDateTime.AddDays(-1);
            }
            catch (FormatException)
            {
                SelectedDateTime = SelectedDateTime.AddDays(1);
                SelectedDateTime = SelectedDateTime.AddDays(-1);
            }
        }

        /// <summary>
        ///     更新日期时间各个块
        /// </summary>
        private void UpdatePartOfDateTime()
        {
            year = SelectedDateTime.Year;
            mouth = SelectedDateTime.Month;
            day = SelectedDateTime.Day;
            hour = SelectedDateTime.Hour;
            minute = SelectedDateTime.Minute;
            second = SelectedDateTime.Second;
        }

        /// <summary>
        ///     获取当前鼠标所在块
        /// </summary>
        /// <param name="mouseIndex">当前光标索引位置</param>
        /// <returns></returns>
        private int GetMouseCell(int mouseIndex)
        {
            var dateTime = SelectedDateTime.ToShortDateString() + " " + SelectedDateTime.ToLongTimeString();
            if (mouseIndex < dateTime.IndexOf('/'))
            {
                return 1;
            }
            if (mouseIndex < dateTime.LastIndexOf('/'))
            {
                return 2;
            }
            if (mouseIndex < (NoTime ? dateTime.Length : dateTime.IndexOf(' ')))
            {
                return 3;
            }
            if (mouseIndex < dateTime.IndexOf(':'))
            {
                return 4;
            }
            if (mouseIndex < dateTime.LastIndexOf(':'))
            {
                return 5;
            }
            return 6;
        }

        #endregion
    }
}