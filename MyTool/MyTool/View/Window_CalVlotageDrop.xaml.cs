using System.ComponentModel;
using System.Windows;

namespace MyTool.View
{
    public partial class Window_CalVlotageDrop : Window, INotifyPropertyChanged
    {
        public double ElectricPower { get; set; }
        public double SectionArea { get; set; }
        public double PowerFactor { get; set; }

        private double _length;
        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        private double _voltageDrop;
        public double VoltageDrop
        {
            get { return _voltageDrop; }
            set
            {
                _voltageDrop = value;
                OnPropertyChanged("VoltageDrop");
            }
        }


        public double Resistivity { get; set; }

        public Window_CalVlotageDrop()
        {
            InitializeComponent();
            this.DataContext = this;
            ElectricPower = 0;
            Length = 0;
            SectionArea = 16;
            VoltageDrop = 0;
            PowerFactor = 0.85;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        double Calculation(double electricPower, double length, double sectionArea, double resistivity, double powerFactor)
        {
            if (sectionArea.Equals(0) || powerFactor.Equals(0))
            {
                MessageBox.Show("电缆截面积 / 功率因数: 数值不能为0，请重新输入");
                return 0;
            }
            double result = electricPower * length * resistivity * 1.519 / sectionArea / powerFactor;
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VoltageDrop = Calculation(ElectricPower, Length, SectionArea, Resistivity, PowerFactor);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Resistivity = 0.0175;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Resistivity = 0.0283;
        }
    }
}
