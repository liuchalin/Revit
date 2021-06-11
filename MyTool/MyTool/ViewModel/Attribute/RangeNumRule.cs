using System.ComponentModel.DataAnnotations;

namespace MyTool.ViewModel
{
    class RangeNumRule : ValidationAttribute
    {
        private double minValue;
        private double maxValue;

        public RangeNumRule(double min, double max)
        {
            minValue = min;
            maxValue = max;
        }

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
            if (num > maxValue || num < minValue)
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "数值范围在" + minValue + "和" + maxValue + "之间";
        }
    }
}
