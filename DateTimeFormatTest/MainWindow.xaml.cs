using System.Text.RegularExpressions;
using System.Windows;

namespace DateTimeFormatTest
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //验证方法-格式为：YYYY-MM-DD
        public static bool ValidateDataTime(string InputStr)
        {
            if (InputStr.Length > 0)
                if (Regex.IsMatch(InputStr.Trim(), @"^((((1[6-9]|[2-9]\d)\d{2})/(0?[13578]|1[02])/(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})/(0?[13456789]|1[012])/(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})/0?2/(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))/0?2/29/))$"))
                    return true;
                else
                    return false;
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var s = textBox1.Text;
            var b = IsDateTime(s);
            var rs = "";
            if (b)
                rs = "true";
            else
                rs = "false";
            MessageBox.Show(rs);
        }

        public static bool IsDateTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^(((((1[6-9]|[2-9]\d)\d{2})/(0?[13578]|1[02])/(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})/(0?[13456789]|1[012])/(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})/0?2/(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))/0?2/29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }
    }
}