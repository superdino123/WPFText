using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MyControlTestDateTimePickerTwo
{
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    [TemplatePart(Name = ElementDatePickerTextBox, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = ElementCalendar, Type = typeof(Calendar))]
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
            _datePickerTextBox.KeyUp += DatePickerTextBoxKeyUp;
            _datePickerTextBox.GotMouseCapture += DatePickerTextBoxGotMouseCapture;

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
            _datePickerTextBox.Select(0, 4);
        }

        private void CalendarDisplayDateChanged(object sender, CalendarDateChangedEventArgs e) {}


        private void DatePickerTextBoxMouseDown(object sender, MouseButtonEventArgs e) {}

        private void DatePickerTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var timesStr = new string[3];
            var smallTimesStr = new string[3];
            int[] timesInt;
            int year;
            int mouth;
            int day;
            int hour;
            int minute;
            int minuteValue;
            int second;
            int secondValue;
            int start;
            string select;

            /*上加下减左移右移
             最传统的枚举法，将位置4种不同情况分别处理，比较麻烦，没有循环，复杂度不高*/

            #region Right(按Right/Down键，start会停留在选中元素的最后一个字符的后面)

            if (e.Key == Key.Right)
            {
                #region 冗余代码，在按非方向键时不触发文本方法，已达到修改不用每按一键就必须验证文本的正确性，避免读取出错

                if (!IsOk()) return;
                timesStr = _datePickerTextBox.Text.Split()[0].Split('/');
                smallTimesStr = _datePickerTextBox.Text.Split()[1].Split(':');
                timesInt = new int[6];
                timesInt[0] = Convert.ToInt32(timesStr[0]);
                timesInt[1] = Convert.ToInt32(timesStr[1]);
                timesInt[2] = Convert.ToInt32(timesStr[2]);
                timesInt[3] = Convert.ToInt32(smallTimesStr[0]);
                timesInt[4] = Convert.ToInt32(smallTimesStr[1]);
                timesInt[5] = Convert.ToInt32(smallTimesStr[2]);

                year = 4;
                mouth = timesInt[1] < 10 ? 1 : 2;
                day = timesInt[2] < 10 ? 1 : 2;
                hour = timesInt[3] < 10 ? 1 : 2;
                minute = 2;
                minuteValue = timesInt[4] < 10 ? 1 : 2;
                second = 2;
                secondValue = timesInt[5] < 10 ? 1 : 2;

                start = _datePickerTextBox.SelectionStart;
                select = _datePickerTextBox.SelectedText;

                #endregion

                #region 逻辑判断

                if (start <= year)
                    _datePickerTextBox.Select(start + 1, mouth);
                else if (start <= year + mouth + 1)
                    _datePickerTextBox.Select(start + 1, day);
                else if (start <= year + mouth + day + 2)
                    _datePickerTextBox.Select(start + 1, hour);
                else if (start <= year + mouth + day + hour + 3)
                    _datePickerTextBox.Select(start + 1, minute);
                else if (start <= year + mouth + day + hour + minute + 4)
                    _datePickerTextBox.Select(start + 1, second);
                else if (start <= year + mouth + day + hour + minute + second + 5)
                    _datePickerTextBox.Select(0, year);

                #endregion
            }

            #endregion

            #region Left(按Left/Up键，start会停留在选中元素的第一个字符的前面)

            if (e.Key == Key.Left)
            {
                #region 冗余代码，在按非方向键时不触发文本方法，已达到修改不用每按一键就必须验证文本的正确性，避免读取出错

                if (!IsOk()) return;
                timesStr = _datePickerTextBox.Text.Split()[0].Split('/');
                smallTimesStr = _datePickerTextBox.Text.Split()[1].Split(':');
                timesInt = new int[6];
                timesInt[0] = Convert.ToInt32(timesStr[0]);
                timesInt[1] = Convert.ToInt32(timesStr[1]);
                timesInt[2] = Convert.ToInt32(timesStr[2]);
                timesInt[3] = Convert.ToInt32(smallTimesStr[0]);
                timesInt[4] = Convert.ToInt32(smallTimesStr[1]);
                timesInt[5] = Convert.ToInt32(smallTimesStr[2]);

                year = 4;
                mouth = timesInt[1] < 10 ? 1 : 2;
                day = timesInt[2] < 10 ? 1 : 2;
                hour = timesInt[3] < 10 ? 1 : 2;
                minute = 2;
                minuteValue = timesInt[4] < 10 ? 1 : 2;
                second = 2;
                secondValue = timesInt[5] < 10 ? 1 : 2;

                start = _datePickerTextBox.SelectionStart;
                select = _datePickerTextBox.SelectedText;

                #endregion

                #region 逻辑判断

                if (start == 0)
                    _datePickerTextBox.Select(year + mouth + day + hour + minute + 5, second);
                else if (start <= year + 1)
                    _datePickerTextBox.Select(0, year);
                else if (start <= year + mouth + 2)
                    _datePickerTextBox.Select(start - mouth - 1, mouth);
                else if (start <= year + mouth + day + 3)
                    _datePickerTextBox.Select(start - day - 1, day);
                else if (start <= year + mouth + day + hour + 4)
                    _datePickerTextBox.Select(start - hour - 1, hour);
                else if (start <= year + mouth + day + hour + minute + 5)
                    _datePickerTextBox.Select(start - minute - 1, minute);
                else if (start <= year + mouth + day + hour + minute + second + 6)
                    _datePickerTextBox.Select(start - second - 1, second);

                #endregion
            }

            #endregion

            #region Up(按Left/Up键，start会停留在选中元素的第一个字符的前面)

            if (e.Key == Key.Up)
            {
                #region 冗余代码，在按非方向键时不触发文本方法，已达到修改不用每按一键就必须验证文本的正确性，避免读取出错

                if (!IsOk()) return;
                timesStr = _datePickerTextBox.Text.Split()[0].Split('/');
                smallTimesStr = _datePickerTextBox.Text.Split()[1].Split(':');
                timesInt = new int[6];
                timesInt[0] = Convert.ToInt32(timesStr[0]);
                timesInt[1] = Convert.ToInt32(timesStr[1]);
                timesInt[2] = Convert.ToInt32(timesStr[2]);
                timesInt[3] = Convert.ToInt32(smallTimesStr[0]);
                timesInt[4] = Convert.ToInt32(smallTimesStr[1]);
                timesInt[5] = Convert.ToInt32(smallTimesStr[2]);

                year = 4;
                mouth = timesInt[1] < 10 ? 1 : 2;
                day = timesInt[2] < 10 ? 1 : 2;
                hour = timesInt[3] < 10 ? 1 : 2;
                minute = 2;
                minuteValue = timesInt[4] < 10 ? 1 : 2;
                second = 2;
                secondValue = timesInt[5] < 10 ? 1 : 2;

                start = _datePickerTextBox.SelectionStart;
                select = _datePickerTextBox.SelectedText;

                #endregion

                #region 逻辑判断

                if (start >= year + mouth + day + hour + minute + 5)
                    _datePickerTextBox.Select(start, second);
                else if (start >= year + mouth + day + hour + 4)
                    _datePickerTextBox.Select(start, minute);
                else if (start >= year + mouth + day + 3)
                    _datePickerTextBox.Select(start, hour);
                else if (start >= year + mouth + 2)
                    _datePickerTextBox.Select(start, day);
                else if (start >= year + 1)
                    _datePickerTextBox.Select(start, mouth);
                else if (start == 0)
                    _datePickerTextBox.Select(0, year);
                select = _datePickerTextBox.SelectedText;
                if (!string.IsNullOrEmpty(_datePickerTextBox.SelectedText))
                    if (start == year + mouth + day + hour + 4 || start == year + mouth + day + hour + minute + 5)
                    {
                        if (Convert.ToInt32(select) >= 59)
                        {
                            _datePickerTextBox.SelectedText = "59";
                        }
                        else
                        {
                            var selectBool = Convert.ToInt32(select) + 1 < 10 ? true : false;
                            if (selectBool)
                                _datePickerTextBox.SelectedText = "0" + (Convert.ToInt32(select) + 1);
                            else
                                _datePickerTextBox.SelectedText = (Convert.ToInt32(select) + 1).ToString();
                        }
                    }
                    else
                    {
                        if (start == year + 1 && Convert.ToInt32(select) >= 12)
                            _datePickerTextBox.SelectedText = "12";
                        else if (start == year + mouth + 2 && Convert.ToInt32(select) >= 31)
                            _datePickerTextBox.SelectedText = "31";
                        else if (start == year + mouth + day + 3 && Convert.ToInt32(select) >= 23)
                            _datePickerTextBox.SelectedText = "23";
                        else
                            _datePickerTextBox.SelectedText = (Convert.ToInt32(select) + 1).ToString();
                    }
                else
                    MessageBox.Show("请选中要增加的日期段");

                #endregion
            }

            #endregion

            #region Down(按Right/Down键，start会停留在选中元素的最后一个字符的后面)

            if (e.Key == Key.Down)
            {
                #region 冗余代码，在按非方向键时不触发文本方法，已达到修改不用每按一键就必须验证文本的正确性，避免读取出错

                if (!IsOk()) return;
                timesStr = _datePickerTextBox.Text.Split()[0].Split('/');
                smallTimesStr = _datePickerTextBox.Text.Split()[1].Split(':');
                timesInt = new int[6];
                timesInt[0] = Convert.ToInt32(timesStr[0]);
                timesInt[1] = Convert.ToInt32(timesStr[1]);
                timesInt[2] = Convert.ToInt32(timesStr[2]);
                timesInt[3] = Convert.ToInt32(smallTimesStr[0]);
                timesInt[4] = Convert.ToInt32(smallTimesStr[1]);
                timesInt[5] = Convert.ToInt32(smallTimesStr[2]);

                year = 4;
                mouth = timesInt[1] < 10 ? 1 : 2;
                day = timesInt[2] < 10 ? 1 : 2;
                hour = timesInt[3] < 10 ? 1 : 2;
                minute = 2;
                minuteValue = timesInt[4] < 10 ? 1 : 2;
                second = 2;
                secondValue = timesInt[5] < 10 ? 1 : 2;

                start = _datePickerTextBox.SelectionStart;
                select = _datePickerTextBox.SelectedText;

                #endregion

                #region 逻辑判断

                if (start <= year)
                    _datePickerTextBox.Select(0, year);
                else if (start <= year + mouth + 1)
                    _datePickerTextBox.Select(start - mouth, mouth);
                else if (start <= year + mouth + day + 2)
                    _datePickerTextBox.Select(start - day, day);
                else if (start <= year + mouth + day + hour + 3)
                    _datePickerTextBox.Select(start - hour, hour);
                else if (start <= year + mouth + day + hour + minute + 4)
                    _datePickerTextBox.Select(start - minute, minute);
                else if (start <= year + mouth + day + hour + minute + second + 5)
                    _datePickerTextBox.Select(start - second, second);

                select = _datePickerTextBox.SelectedText;
                if (!string.IsNullOrEmpty(_datePickerTextBox.SelectedText))
                    if (start == year + mouth + day + hour + minute + 4 && minuteValue == 1 || start == year + mouth + day + hour + minute + second + 5 && secondValue == 1)
                    {
                        if (Convert.ToInt32(select) == 0)
                        {
                            _datePickerTextBox.SelectedText = "00";
                        }
                        else
                        {
                            var selectBool = Convert.ToInt32(select) - 1 < 10 ? true : false;
                            if (selectBool)
                                _datePickerTextBox.SelectedText = "0" + (Convert.ToInt32(select) - 1);
                            else
                                _datePickerTextBox.SelectedText = (Convert.ToInt32(select) - 1).ToString();
                        }
                    }
                    else
                    {
                        if ((start == year + mouth + 1 || start == year + mouth + day + 2) && Convert.ToInt32(select) == 1)
                            _datePickerTextBox.SelectedText = "1";
                        else if (start == year + mouth + day + hour + 3 && Convert.ToInt32(select) == 0)
                            _datePickerTextBox.SelectedText = "0";
                        else
                            _datePickerTextBox.SelectedText = Convert.ToInt32(select) == 1 ? "0" : (Convert.ToInt32(select) - 1).ToString();
                    }
                else
                    MessageBox.Show("请选中要增加的日期段");

                #endregion
            }

            #endregion
        }

        private void DatePickerTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            var timesStr = _datePickerTextBox.Text.Split()[0].Split('/');
            var smallTimesStr = _datePickerTextBox.Text.Split()[1].Split(':');
            var timesInt = new int[6];
            timesInt[0] = Convert.ToInt32(timesStr[0]);
            timesInt[1] = Convert.ToInt32(timesStr[1]);
            timesInt[2] = Convert.ToInt32(timesStr[2]);
            timesInt[3] = Convert.ToInt32(smallTimesStr[0]);
            timesInt[4] = Convert.ToInt32(smallTimesStr[1]);
            timesInt[5] = Convert.ToInt32(smallTimesStr[2]);

            var year = 4;
            var mouth = timesInt[1] < 10 ? 1 : 2;
            var day = timesInt[2] < 10 ? 1 : 2;
            var hour = timesInt[3] < 10 ? 1 : 2;
            var minute = 2;
            var minuteValue = timesInt[4] < 10 ? 1 : 2;
            var second = 2;
            var secondValue = timesInt[5] < 10 ? 1 : 2;

            var start = _datePickerTextBox.SelectionStart;
            if (start <= year)
                _datePickerTextBox.Select(0, year);
            else if (start <= year + mouth + 1)
                _datePickerTextBox.Select(year + 1, mouth);
            else if (start <= year + mouth + day + 2)
                _datePickerTextBox.Select(year + mouth + 2, day);
            else if (start <= year + mouth + day + hour + 3)
                _datePickerTextBox.Select(year + mouth + day + 3, hour);
            else if (start <= year + mouth + day + hour + minute + 4)
                _datePickerTextBox.Select(year + mouth + day + hour + 4, minute);
            else if (start <= year + mouth + day + hour + minute + second + 5)
                _datePickerTextBox.Select(year + mouth + day + hour + minute + 5, second);
        }

        /// <summary>
        ///     验证数据有效性，无效撤销
        /// </summary>
        /// <returns></returns>
        private bool IsOk()
        {
            //验证日期的正确性
            if (!Regex.IsMatch(_datePickerTextBox.Text.Trim(), @"^(((((1[6-9]|[2-9]\d)\d{2})/(0?[13578]|1[02])/(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})/(0?[13456789]|1[012])/(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})/0?2/(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))/0?2/29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$"))
            {
                _datePickerTextBox.SelectAll();
                _datePickerTextBox.SelectedText = "";
                return false;
            }
            return true;
        }
    }
}