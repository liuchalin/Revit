using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.View;
using MyTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Interop;

namespace MyTool
{
    [Transaction(TransactionMode.Manual)]
    class CheckCableCountInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Guid cableCountSchemaId = new Guid("8D57AB90-FB70-44E3-9C43-E73A8446304A");

            Element elem = sel.PickObject(ObjectType.Element, new CableTrayFilter()).GetElement(doc);
            if (elem == null)
            {
                return Result.Cancelled;
            }

            Entity ent = elem.GetEntity(Schema.Lookup(cableCountSchemaId));
            if (ent.Schema == null)
            {
                TaskDialog.Show("查询无结果", "该桥架不包含电缆统计信息");
                return Result.Cancelled;
            }

            string currentDLLPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string configFilePath = Path.Combine(Path.GetDirectoryName(currentDLLPath), "MyTool.dll.config");
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configFilePath }, ConfigurationUserLevel.None);
            string cableTypeListFilePath = config.AppSettings.Settings["cableTypeListFilePath"].Value;
            DataTable cableDT = CableInfoManager.Instance().ReadCSV(cableTypeListFilePath);

            double ctWidth = elem.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsDouble() * 304.8;
            double ctHeight = elem.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsDouble() * 304.8;
            double ctLength = (elem.Location as LocationCurve).Curve.Length * 304.8;

            IDictionary<string, int> existDic = ent.Get<IDictionary<string, int>>(Schema.Lookup(cableCountSchemaId).GetField("dictionary"));
            VM_CheckCableCountInfo vm = new VM_CheckCableCountInfo(existDic, cableDT, ctWidth, ctHeight, ctLength);
            Window_CheckCableCountInfo window = new Window_CheckCableCountInfo(vm);
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ShowDialog();

            return Result.Succeeded;
        }
    }
}
