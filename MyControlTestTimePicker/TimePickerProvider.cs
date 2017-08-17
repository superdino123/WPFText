using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyControlTestTimePicker
{
    public class TimePickerProvider
    {
        public TimePicker TimePickerCase = new TimePicker();

        public TimePicker GetTimePickerCase() {
            TimePickerCase.SourceHours = Enumerable.Range(0, 12);
            return TimePickerCase;
        }
    }
}
