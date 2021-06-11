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
    class WallAndConduit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Wall wall = null;
            List<Conduit> conduits = null;
            try
            {
                wall = sel.PickObject(ObjectType.Element, new WallFilter(), "请选择需要开孔的墙体").GetElement(doc) as Wall;
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
        SelectConduits:
            try
            {
                conduits = sel.PickObjects(ObjectType.Element, new ConduitFilter(), "请多选或框选穿越该墙体的电缆线管").Select(p => p.GetElement(doc) as Conduit).ToList();
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
            if (conduits.Count == 0)
            {
                TaskDialog.Show("错误", "未选到电缆线管，请重新选择");
                goto SelectConduits;
            }
            var rfaname = "洞口 - 圆形";
            FamilySymbol circleOpen = LoadRfa(doc, rfaname);
            if (circleOpen == null)
            {
                TaskDialog.Show("错误", "未找到洞口族文件，请载入名称为\"洞口 - 圆形\"的族文件");
                return Result.Cancelled;
            }
            using (Transaction trans = new Transaction(doc, "为线管开圆形洞口"))
            {
                trans.Start();
                foreach (Conduit con in conduits)
                {
                    CircularOpen(doc, wall, con, circleOpen);
                }
                trans.Commit();
            }

            return Result.Succeeded;
        }

        //获取线管LocationCurve与墙面交点
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

        //布置孔洞族
        void CircularOpen(Document doc, Wall wall, Conduit conduit, FamilySymbol familySymbol)
        {
            SubTransaction subTrans = new SubTransaction(doc);
            subTrans.Start();
            try
            {
                Face wallFace = GetWallFace(wall);
                Level lv = wall.LevelId.GetElement(doc) as Level;
                Curve conCurve = (conduit.Location as LocationCurve).Curve;
                XYZ intersectionPt = GetIntersection(wallFace, conCurve);
                double conduitSize = conduit.get_Parameter(BuiltInParameter.RBS_CONDUITRUN_OUTER_DIAM_PARAM).AsDouble();
                double openSize = conduitSize + 10 / 304.8;
                XYZ insertPt = new XYZ(intersectionPt.X, intersectionPt.Y, intersectionPt.Z - openSize / 2);
                FamilyInstance instance = doc.Create.NewFamilyInstance(insertPt, familySymbol, wall, lv, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                Parameter instanceSize = instance.LookupParameter("直径");
                instanceSize.Set(openSize);
                subTrans.Commit();
            }
            catch
            {
                subTrans.Dispose();
            }
        }

        //加载圆形孔洞族
        FamilySymbol LoadRfa(Document doc, string rfaname)
        {
            FamilySymbol familySymbol = null;
            FilteredElementCollector symbols = new FilteredElementCollector(doc);
            symbols.OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilySymbol));

            foreach (FamilySymbol elem in symbols)
            {
                if (elem.FamilyName == rfaname)
                {
                    familySymbol = elem;
                    break;
                }
            }
            return familySymbol;
        }
    }
}
