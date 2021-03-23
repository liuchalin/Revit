using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.Common;
using MyTool.Filter;
using MyTool.View;
using MyTool.ViewModel;
using System.Collections.Generic;
using System.Windows.Interop;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool.Arrangement
{
    [Transaction(TransactionMode.Manual)]
    class ArrangeCableTrayHanger : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            VM_ArrangeCableTrayHanger vm = new VM_ArrangeCableTrayHanger();
            Window_ArrangeCableTrayHanger win = new Window_ArrangeCableTrayHanger(vm);
            WindowInteropHelper helper = new WindowInteropHelper(win);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            win.ShowDialog();

            if (vm.ToClose)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }

            var rfaName = "桥架支吊架";
            FamilySymbol hanger = LoadRfa(doc, rfaName);

            Transaction ts = new Transaction(doc, "创建支吊架");
            try
            {
                while (true)
                {
                    ts.Start();
                    Element hostElem = sel.PickObject(ObjectType.Element, new CableTrayFilter(), "选择需要创建支吊架的桥架段").GetElement(doc);
                    XYZ hostElemVector = new XYZ();
                    List<XYZ> insertPts = GetInsertPts(hostElem, vm.Span, out hostElemVector);
                    Level lv = (hostElem.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId()).GetElement(doc) as Level;
                    double ctWidth = hostElem.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsDouble();
                    double ctHeight = hostElem.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsDouble();
                    double offset = hostElem.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).AsDouble() - ctHeight / 2;
                    double angle = (new XYZ(0, -1, 0)).AngleTo(hostElemVector);
                    CreatHanger(doc, insertPts, hanger, lv, ctWidth, offset, angle);
                    ts.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
                TaskDialog.Show("中断", "操作中断");
            }

            return Result.Succeeded;
        }

        void CreatHanger(Document doc, List<XYZ> insertPts, FamilySymbol hangerType, Level lv, double width, double offset, double angle)
        {
            for (int i = 0; i < insertPts.Count; i++)
            {
                XYZ insertPt = insertPts[i];
                FamilyInstance instance = doc.Create.NewFamilyInstance(insertPt, hangerType, lv, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                instance.LookupParameter("宽度").Set(width);
                instance.LookupParameter("偏移").Set(offset);
                Line rotateAxis = Line.CreateBound(new XYZ(insertPt.X, insertPt.Y, 0), new XYZ(insertPt.X, insertPt.Y, 1));
                ElementTransformUtils.RotateElement(doc, instance.Id, rotateAxis, angle);
            }
        }

        FamilySymbol LoadRfa(Document doc, string rfaName)
        {
            FamilySymbol familySymbol = null;
            FilteredElementCollector symbols = new FilteredElementCollector(doc);
            symbols.OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilySymbol));
            foreach (FamilySymbol item in symbols)
            {
                if (item.FamilyName == rfaName)
                {
                    familySymbol = item;
                    break;
                }
            }
            return familySymbol;
        }

        List<XYZ> GetInsertPts(Element elem, double distance, out XYZ hostElemVector)
        {
            List<XYZ> insertPts = new List<XYZ>();
            double span = distance / 304.8;
            LocationCurve lc = elem.Location as LocationCurve;
            double length = lc.Curve.Length;
            double halfLength = length / 2 - 100 / 304.8;
            XYZ centerPt = lc.Curve.Evaluate(0.5, true);
            int num = (int)(halfLength / span);
            XYZ pt1 = lc.Curve.GetEndPoint(0);
            XYZ pt2 = lc.Curve.GetEndPoint(1);
            Line line = Line.CreateBound(pt1, pt2);
            XYZ vector = line.Direction.Normalize();
            hostElemVector = vector;
            insertPts.Add(centerPt);
            if (span <= halfLength && span > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    XYZ pt = centerPt + vector * (i + 1) * span;
                    insertPts.Add(pt);
                    pt = centerPt - vector * (i + 1) * span;
                    insertPts.Add(pt);
                }
            }
            return insertPts;
        }
    }
}
