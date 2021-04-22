using MyTool.ViewModel;
using System.Windows;

namespace MyTool.View
{
    public partial class Window_CalVoltageDrop : Window
    {
        public Window_CalVoltageDrop(VM_CalVoltageDrop vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
