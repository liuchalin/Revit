using MyTool.ViewModel;
using System.Windows;

namespace MyTool.View
{
    public partial class Window_FindCableTrayPath : Window
    {
        public Window_FindCableTrayPath(VM_FindCableTrayPath vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.DragMove();
        }
    }
}
