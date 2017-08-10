using System.Windows.Controls;

namespace Edms.ClientControls
{
    class DatePickerEx:DatePicker
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox editableTextBox = base.GetTemplateChild("PART_TextBox") as TextBox;
            if (editableTextBox != null)
            {
               

            }
          
        }


    }
}
