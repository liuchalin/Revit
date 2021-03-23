using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.Common;
using MyTool.Filter;
using System.Collections.Generic;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool.Arrangement
{
    [Transaction(TransactionMode.Manual)]
    class SimilarConduit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            List<Element> templateElems = null;
        SelecTemplate:
            try
            {
                templateElems = sel.PickObjects(ObjectType.Element, new ConduitAndModelLineFIlter(), "请框选样板段").Select(r => r.GetElement(doc)).ToList();
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
            List<ModelLine> templateML = (from e in templateElems where e is ModelLine select e as ModelLine).ToList();
            List<Conduit> templateConduits = (from e in templateElems where e is Conduit select e as Conduit).ToList();
            if (templateML.Count != 1)
            {
                TaskDialog.Show("错误", "未选到基准线，或样板段中存在多条基准线，请重新选择");
                goto SelecTemplate;
            }
            if (templateConduits.Count == 0)
            {
                TaskDialog.Show("错误", "未选到样板中的线管，请重新选择");
                goto SelecTemplate;
            }
            Dictionary<Conduit, XYZ> conduitVectorPairs = GetConduitVectorPairs(templateConduits, templateML.FirstOrDefault());

            Transaction ts = new Transaction(doc, "创建标准断");
            try
            {
                while (true)
                {
                    Element creatLine = sel.PickObject(ObjectType.Element, "请选择标准断基准线").GetElement(doc);
                    for (int i = 0; i < conduitVectorPairs.Count; i++)
                    {
                        ts.Start();
                        var cvPair = conduitVectorPairs.ElementAt(i);
                        var templateConduit = cvPair.Key;
                        var templateConuitType = templateConduit.GetTypeId().GetElement(doc) as ConduitType;
                        var relativeVector = cvPair.Value;
                        double conduitSize = templateConduit.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).AsDouble();
                        var newConduit = CreatOneConduit(doc, creatLine, templateConuitType, relativeVector);
                        Parameter newConduitSize = newConduit.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);
                        newConduitSize.Set(conduitSize);
                        ts.Commit();
                    }
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

        Conduit CreatOneConduit(Document doc, Element elem, ConduitType type, XYZ vector)
        {
            XYZ pt1 = (elem.Location as LocationCurve).Curve.GetEndPoint(0);
            XYZ pt2 = (elem.Location as LocationCurve).Curve.GetEndPoint(1);
            Line line = Line.CreateBound(pt1, pt2);
            XYZ y = line.Direction.Normalize();
            XYZ z = new XYZ(0, 0, 1);

            Transform tf = Transform.Identity;
            tf.BasisY = y;
            tf.BasisZ = z;
            tf.BasisX = y.CrossProduct(z).Normalize();
            tf.Origin = pt1;

            XYZ newVector = tf.OfPoint(vector);
            XYZ spt = newVector;
            XYZ ept = spt + y * line.Length;
            return Conduit.Create(doc, type.Id, spt, ept, elem.LevelId);
        }

        Dictionary<Conduit, XYZ> GetConduitVectorPairs(List<Conduit> templateConduit, ModelLine datumLine)
        {
            Dictionary<Conduit, XYZ> outPairs = new Dictionary<Conduit, XYZ>();
            List<Conduit> conduits = templateConduit;
            XYZ originPt = (datumLine.Location as LocationCurve).Curve.GetEndPoint(0);
            foreach (var conduit in conduits)
            {
                XYZ conduitPt;
                XYZ pt1 = (conduit.Location as LocationCurve).Curve.GetEndPoint(0);
                XYZ pt2 = (conduit.Location as LocationCurve).Curve.GetEndPoint(1);
                if (pt1.DistanceTo(originPt) < pt2.DistanceTo(originPt))
                {
                    conduitPt = pt1;
                }
                else
                {
                    conduitPt = pt2;
                }
                XYZ relationVector = conduitPt - originPt;
                outPairs.Add(conduit, relationVector);
            }
            return outPairs;
        }
    }
}
