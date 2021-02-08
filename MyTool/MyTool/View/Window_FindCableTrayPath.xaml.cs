using MyTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTool.View
{
    /// <summary>
    /// Window_FindCableTrayPath.xaml 的交互逻辑
    /// </summary>
    public partial class Window_FindCableTrayPath : Window
    {
        public Window_FindCableTrayPath(VM_FindCableTrayPath vm)
        {
            InitializeComponent();
            DataContext = vm;
        }        
    }
}
