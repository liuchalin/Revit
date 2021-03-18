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
using MyTool.ViewModel;

namespace MyTool.View
{
    public partial class Window_FindPipePath : Window
    {
        public Window_FindPipePath(VM_FindPipePath vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
