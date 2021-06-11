using System.ComponentModel.DataAnnotations;

namespace MyTool.ViewModel
{
    class NonnegativeNumRule : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (!double.TryParse(value.ToString(), out double num))
            {
                return false;
            }
            if (num < 0)
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "数值不能为负数";
        }
    }
}
