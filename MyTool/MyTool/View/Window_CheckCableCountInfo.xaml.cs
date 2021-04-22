using MyTool.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace MyTool.View
{
    public partial class Window_CheckCableCountInfo : Window
    {
        public Window_CheckCableCountInfo(VM_CheckCableCountInfo vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.DragMove();
        }
    }
}
