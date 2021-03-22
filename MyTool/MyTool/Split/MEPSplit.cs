using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool.Split
{
    [Transaction(TransactionMode.Manual)]
    class MEPSplit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Transaction ts = new Transaction(doc);
            try
            {
                while (true)
                {
                    ts.Start("打断");
                    Reference refElement = sel.PickObject(ObjectType.PointOnElement, new MEPFilter(doc), "请选择需要打断的点");
                    XYZ point = refElement.GlobalPoint;
                    Element elem = doc.GetElement(refElement);
                    LocationCurve lc = elem.Location as LocationCurve;
                    XYZ cutPt = lc.Curve.Project(point).XYZPoint;
                    if (lc == null)
                    {
                        TaskDialog.Show("错误", "你选择的构件不可以分割");
                        return Result.Cancelled;
                    }
                    XYZ startPt = lc.Curve.GetEndPoint(0);
                    XYZ endPt = lc.Curve.GetEndPoint(1);
                    Line l1 = Line.CreateBound(startPt, cutPt);
                    Line l2 = Line.CreateBound(cutPt, endPt);
                    Element elem1 = doc.GetElement(ElementTransformUtils.CopyElement(doc, elem.Id, new XYZ(0, 0, 0)).First());
                    Element elem2 = doc.GetElement(ElementTransformUtils.CopyElement(doc, elem.Id, new XYZ(0, 0, 0)).First());
                    (elem1.Location as LocationCurve).Curve = l1;
                    (elem2.Location as LocationCurve).Curve = l2;
                    List<Connector> originalCons = GetConnectors(elem);
                    List<Connector> linkCons = new List<Connector>();
                    List<Connector> targetCons = GetConnectors(elem1).Union(GetConnectors(elem2)).ToList();
                    foreach (Connector con in originalCons)
                    {
                        var conRefs = con.AllRefs;
                        foreach (Connector conRef in conRefs)
                        {
                            if (conRef.ConnectorType == ConnectorType.Logical)
                            {
                                continue;
                            }
                            else if (conRef.Owner.Id == elem.Id)
                            {
                                continue;
                            }
                            linkCons.Add(conRef);
                        }
                    }
                    if (linkCons.Count == 1)
                    {
                        Connector nearCon = NearConnector(targetCons, linkCons[0]);
                        nearCon.ConnectTo(linkCons[0]);
                    }
                    else if (linkCons.Count == 2)
                    {
                        Connector nearCon1 = NearConnector(targetCons, linkCons[0]);
                        nearCon1.ConnectTo(linkCons[0]);
                        Connector nearCon2 = NearConnector(targetCons, linkCons[1]);
                        nearCon2.ConnectTo(linkCons[1]);
                    }
                    doc.Delete(elem.Id);
                    ts.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }
            return Result.Succeeded;
        }

        Connector NearConnector(List<Connector> connectors, Connector targetCon)
        {
            double distance = double.MaxValue;
            Connector nearCon = null;
            XYZ targetOrigin = targetCon.Origin;
            foreach (Connector con in connectors)
            {
                if (con.Origin.DistanceTo(targetOrigin) < distance)
                {
                    distance = con.Origin.DistanceTo(targetOrigin);
                    nearCon = con;
                }
            }
            return nearCon;
        }

        List<Connector> GetConnectors(Element elem)
        {
            List<Connector> outList = new List<Connector>();
            MEPCurve temp = elem as MEPCurve;
            foreach (Connector con in temp.ConnectorManager.Connectors)
            {
                if (
                    (con.ConnectorType == ConnectorType.End
                    || con.ConnectorType == ConnectorType.Curve
                    || con.ConnectorType == ConnectorType.Physical)
                    )
                {
                    outList.Add(con);
                }
            }
            return outList;
        }
    }
}
