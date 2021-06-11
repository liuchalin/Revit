using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool
{
    [Transaction(TransactionMode.Manual)]
    class WallAndCableTray : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Wall wall = null;
            List<CableTray> cableTrays = null;
            try
            {
                wall = sel.PickObject(ObjectType.Element, new WallFilter(), "请选择需要开孔的墙体").GetElement(doc) as Wall;
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
        SelectCableTrays:
            try
            {
                cableTrays = sel.PickObjects(ObjectType.Element, new CableTrayFilter(), "请多选或框选穿越该墙体的电缆桥架").Select(p => p.GetElement(doc) as CableTray).ToList();
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
            if (cableTrays.Count == 0)
            {
                TaskDialog.Show("错误", "未选到电缆桥架，请重新选择");
                goto SelectCableTrays;
            }
            using (Transaction trans = new Transaction(doc, "为桥架开矩形洞口"))
            {
                trans.Start();
                foreach (CableTray ct in cableTrays)
                {
                    RectangleOpen(doc, wall, ct);
                }
                trans.Commit();
            }
            return Result.Succeeded;
        }

        //开矩形孔洞
        void RectangleOpen(Document doc, Wall wall, CableTray cableTray)
        {
            SubTransaction subTrans = new SubTransaction(doc);
            subTrans.Start();
            try
            {
                Face wallFace = GetWallFace(wall);
                Curve ctCurve = (cableTray.Location as LocationCurve).Curve;
                XYZ oPt = GetIntersection(wallFace, ctCurve);
                XYZ wallVector = GetWallVector(wall);
                Parameter widthPara = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                double width = widthPara.AsDouble();
                Parameter heightPara = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);
                double height = heightPara.AsDouble();
                XYZ pt1 = oPt + new XYZ(0, 0, 1) * (height / 2 + 15 / 304.8) + wallVector.Normalize() * (width / 2 + 15 / 304.8);
                XYZ pt2 = oPt + new XYZ(0, 0, -1) * (height / 2 + 15 / 304.8) - wallVector.Normalize() * (width / 2 + 15 / 304.8);
                doc.Create.NewOpening(wall, pt1, pt2);
                subTrans.Commit();
            }
            catch
            {
                subTrans.Dispose();
            }
        }

        //获取墙LocationCurve方向
        XYZ GetWallVector(Wall wall)
        {
            LocationCurve lCurve = wall.Location as LocationCurve;
            XYZ xyz = lCurve.Curve.GetEndPoint(1) - lCurve.Curve.GetEndPoint(0);
            return xyz;
        }

        //获取桥架LocationCurve与墙面交点
        XYZ GetIntersection(Face face, Curve curve)
        {
            XYZ intersectionResult = null;
            IntersectionResultArray resultArray = new IntersectionResultArray();
            SetComparisonResult comparisonResult = face.Intersect(curve, out resultArray);
            if (comparisonResult != SetComparisonResult.Disjoint)
            {
                if (!resultArray.IsEmpty)
                {
                    intersectionResult = resultArray.get_Item(0).XYZPoint;
                }
            }
            return intersectionResult;
        }

        //获取墙面
        Face GetWallFace(Wall wall)
        {
            Face normalface = null;
            Options opt = new Options() { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine };
            GeometryElement ge = wall.get_Geometry(opt);
            foreach (var geobject in ge)
            {
                Solid solid = geobject as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face face in solid.Faces)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (pf != null)
                        {
                            if (pf.FaceNormal.AngleTo(wall.Orientation) < 0.01)
                            {
                                normalface = face;
                            }
                        }
                    }
                }
            }
            return normalface;
        }
    }
}
