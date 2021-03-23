using MyTool.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace MyTool.View
{
    public partial class Window_ArrangeCableTrayHanger : Window
    {
        public Window_ArrangeCableTrayHanger(VM_ArrangeCableTrayHanger vm)
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
