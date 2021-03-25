using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MyTool.View;
using System.Windows.Interop;

namespace MyTool.Calculation
{
    [Transaction(TransactionMode.Manual)]
    class CalVoltageDrop : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Window_CalVlotageDrop win = new Window_CalVlotageDrop();
            WindowInteropHelper helper = new WindowInteropHelper(win);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win.Show();

            return Result.Succeeded;
        }
    }
}
