using MyTool.ViewModel;
using System.Windows;

namespace MyTool.View
{
    public partial class WIndow_WriteCableCountInfo : Window
    {
        public WIndow_WriteCableCountInfo(VM_WriteCableCountInfo vm)
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
