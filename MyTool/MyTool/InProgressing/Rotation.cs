using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;


namespace MyTool
{
    [Transaction(TransactionMode.Manual)]
    class Rotation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            List<Element> elems = new List<Element>();
            try
            {
                elems = sel.PickObjects(ObjectType.Element, new PointInstanceFilter(), "选择需要旋转的构件").Select(p => p.GetElement(doc)).ToList();
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }

            //旋转90度
            Transaction trans = new Transaction(doc);
            trans.Start("批量旋转");
            foreach (Element elem in elems)
            {
                XYZ pt1 = (elem.Location as LocationPoint).Point;
                XYZ pt2 = new XYZ(pt1.X, pt1.Y, 100);
                Line axis = Line.CreateBound(pt1, pt2);
                double angle = Math.PI / 2;

                ElementTransformUtils.RotateElement(doc, elem.Id, axis, angle);
            }
            trans.Commit();

            return Result.Succeeded;
        }
    }
}
