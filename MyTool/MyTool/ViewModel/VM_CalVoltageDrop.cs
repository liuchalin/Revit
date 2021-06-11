using System.ComponentModel.DataAnnotations;

namespace MyTool.ViewModel
{
    public class VM_CalVoltageDrop : ValidateBase, IValidationExceptionHandler
    {
        private double _electircPower;
        private double _sectionArea;
        private double _length;
        private double _powerFactor;
        private double _resistivity;

        [Required]
        [NonnegativeNumRule]
        public double ElectricPower
        {
            get { return _electircPower; }
            set
            {
                if (_electircPower != value)
                {
                    _electircPower = value;
                    OnPropertyChanged(nameof(ElectricPower));
                }
            }
        }

        [Required]
        [PositiveNumRule]
        public double SectionArea
        {
            get { return _sectionArea; }
            set
            {
                if (_sectionArea != value)
                {
                    _sectionArea = value;
                    OnPropertyChanged(nameof(SectionArea));
                }
            }
        }

        [Required]
        [NonnegativeNumRule]
        public double Length
        {
            get { return _length; }
            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        [Required]
        [PositiveNumRule]
        [RangeNumRule(0, 1)]
        public double PowerFactor
        {
            get { return _powerFactor; }
            set
            {
                if (_powerFactor != value)
                {
                    _powerFactor = value;
                    OnPropertyChanged(nameof(PowerFactor));
                }
            }
        }

        public double Resistivity
        {
            get { return _resistivity; }
            set
            {
                if (_resistivity != value)
                {
                    _resistivity = value;
                    OnPropertyChanged(nameof(Resistivity));
                }
            }
        }

        private double _voltageDrop;
        public double VoltageDrop
        {
            get { return _voltageDrop; }
            set
            {
                if (_voltageDrop != value)
                {
                    _voltageDrop = value;
                    OnPropertyChanged(nameof(VoltageDrop));
                }
            }
        }

        private CommandBase _calCmd;
        public CommandBase CalCmd
        {
            get
            {
                if (_calCmd == null)
                {
                    _calCmd = new CommandBase(Calculation, CanExcute);
                }
                return _calCmd;
            }
        }

        void Calculation(object o)
        {
            VoltageDrop = ElectricPower * Length * Resistivity * 1.519 / SectionArea / PowerFactor;
        }

        bool CanExcute(object o)
        {
            return IsValid;
        }

        public bool IsValid
        {
            get => base.IsEnable;
            set
            {
                base.IsEnable = value;
            }
        }

        public VM_CalVoltageDrop()
        {
            ElectricPower = 0;
            Length = 0;
            SectionArea = 16;
            VoltageDrop = 0;
            PowerFactor = 0.85;
            Resistivity = 0.0175;
            IsValid = true;
        }
    }
}
