namespace MyControlTestDatePickerEasy
{
    public enum TimePartVisibility
    {
        Hour = 1 << 1,             //00000010
        Minute = 1 << 2,           //00000100
        Second = 1 << 3,           //00001000
        HourMinute = Hour | Minute,//00000110
        All = HourMinute | Second  //00001110
    }
}