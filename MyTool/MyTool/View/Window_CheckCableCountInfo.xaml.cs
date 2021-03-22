using MyTool.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace MyTool.View
{
    /// <summary>
    /// Window_CheckCableCountInfo.xaml 的交互逻辑
    /// </summary>
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
