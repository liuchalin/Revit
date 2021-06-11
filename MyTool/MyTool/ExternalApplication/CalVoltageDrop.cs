using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MyTool.View;
using System.Windows.Interop;
using MyTool.ViewModel;

namespace MyTool
{
    [Transaction(TransactionMode.Manual)]
    class CalVoltageDrop : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Window_CalVoltageDrop win = new Window_CalVoltageDrop(new VM_CalVoltageDrop());
            WindowInteropHelper helper = new WindowInteropHelper(win);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win.Show();

            return Result.Succeeded;
        }
    }
}
