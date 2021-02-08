using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.ExtensionMethod;
using MyTool.Filter;
using System.Collections.Generic;
using System.Linq;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool.FindPath
{
    class FindCableTrayPath
    {
        [Transaction(TransactionMode.Manual)]
        class FindCableTrayRoute : IExternalCommand
        {
            public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Selection sel = uiDoc.Selection;

                Element startElem = null;
                Element endElem = null;
                List<Element> allCandidateElems = new List<Element>();
                #region 选择起点桥架，终点桥架，路径网络
                try
                {
                    startElem = sel.PickObject(ObjectType.Element, new CableTrayFilter(), "选择起始桥架").GetElement(doc);
                    endElem = sel.PickObject(ObjectType.Element, new CableTrayFilter(), "选择终点桥架").GetElement(doc);

                }
                catch (OperationCanceledException)
                {
                    TaskDialog.Show("取消", "操作已取消");
                    return Result.Cancelled;
                }
                SelectCableTrayLayout:
                try
                {
                    allCandidateElems = sel.PickObjects(ObjectType.Element, new CableTrayPathFilter(), "请框选桥架网络").Select(p => p.GetElement(doc)).ToList();
                }
                catch (OperationCanceledException)
                {
                    TaskDialog.Show("取消", "操作已取消");
                    return Result.Cancelled;
                }
                if (allCandidateElems.Count == 0)
                {
                    TaskDialog.Show("错误", "未选到网络，请重新选择");
                    goto SelectCableTrayLayout;
                }
                #endregion

                #region 元素邻接表，路径BFS算法
                Dictionary<ElementId, List<ElementId>> neighborList = GetNeighborList(doc, allCandidateElems);
                List<List<ElementId>> pathList = GetAllPath(doc, neighborList, startElem.Id, endElem.Id);
                pathList = pathList.OrderBy(p => GetPathLength(doc, p)).ToList();
                #endregion

                #region WPF窗口，MVVM模式
                List<VM_RouteList> vm_RouteList = new List<VM_RouteList>();
                foreach (var route in pathList)
                {
                    VM_RouteList vm = new VM_RouteList(route, pathList.IndexOf(route) + 1);
                    vm_RouteList.Add(vm);
                }

                Window_RouteList window_route = new Window_RouteList(vm_RouteList);
                WindowInteropHelper helper = new WindowInteropHelper(window_route);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                window_route.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                window_route.ShowDialog();

                List<ElementId> reviewElem = null;
                while (true)
                {
                    if (window_route.IsReview)
                    {
                        window_route.IsReview = false;
                        reviewElem = (window_route.listBox.SelectedItem as VM_RouteList).RouteList;
                        sel.SetElementIds(reviewElem);
                        uiDoc.RefreshActiveView();
                        window_route.ShowDialog();
                    }
                    else if (window_route.IsDirectClose)
                    {
                        break;
                    }
                }

                return Result.Succeeded;
            }

            double GetPathLength(Document doc, List<ElementId> route)
            {
                double length = -1;
                for (int i = 0; i < route.Count - 1; i++)
                {
                    length += GetCenterPoint(route[i].GetElement(doc)).DistanceTo(GetCenterPoint(route[i + 1].GetElement(doc)));
                }
                Curve startCurve = (route.First().GetElement(doc).Location as LocationCurve).Curve;
                Curve endCurve = (route.Last().GetElement(doc).Location as LocationCurve).Curve;
                length = length + startCurve.Length + endCurve.Length;
                return length;
            }

            XYZ GetCenterPoint(Element elem)
            {
                if (elem != null)
                {
                    XYZ centerPoint;
                    if (elem.Location is LocationCurve)
                    {
                        Curve c = (elem.Location as LocationCurve).Curve;
                        centerPoint = c.Evaluate(0.5, true);
                    }
                    else
                    {
                        centerPoint = (elem.Location as LocationPoint).Point;
                    }
                    return centerPoint;
                }
                else
                {
                    return null;
                }
            }

            List<List<ElementId>> GetAllPath(Document doc, Dictionary<ElementId, List<ElementId>> neighborList, ElementId startElemId, ElementId endElemId)
            {
                List<List<ElementId>> result = new List<List<ElementId>>();
                Queue<List<ElementId>> queue = new Queue<List<ElementId>>();
                List<ElementId> startList = new List<ElementId> { startElemId };
                queue.Enqueue(startList);
                while (queue.Count > 0)
                {
                    List<ElementId> route = queue.Dequeue();
                    ElementId tailNode = route.Last();
                    if (tailNode == endElemId)
                    {
                        result.Add(route);
                    }
                    else
                    {
                        if (!neighborList.Keys.Contains(tailNode))
                        {
                            continue;
                        }
                        List<ElementId> neighbors = neighborList[tailNode];
                        foreach (ElementId neighbor in neighbors)
                        {
                            List<ElementId> newList = (from id in route select id).ToList();
                            if (!newList.Contains(neighbor))
                            {
                                newList.Add(neighbor);
                                queue.Enqueue(newList);
                            }
                        }
                    }
                }
                return result;
            }

            Dictionary<ElementId, List<ElementId>> GetNeighborList(Document doc, List<Element> allElems)
            {
                Dictionary<ElementId, List<ElementId>> outDic = new Dictionary<ElementId, List<ElementId>>();
                foreach (Element elem in allElems)
                {
                    List<ElementId> neighborElemIds = new List<ElementId>();
                    if (elem is MEPCurve)
                    {
                        var elemCons = (elem as MEPCurve).ConnectorManager.Connectors;
                        foreach (Connector con in elemCons)
                        {
                            var linkCons = con.AllRefs;
                            foreach (Connector linkCon in linkCons)
                            {
                                if (linkCon.ConnectorType == ConnectorType.Logical)
                                {
                                    continue;
                                }
                                else if (linkCon.Owner.Id == elem.Id)
                                {
                                    continue;
                                }
                                neighborElemIds.Add(linkCon.Owner.Id);
                            }
                        }
                    }
                    else
                    {
                        var elemCons = (elem as FamilyInstance).MEPModel.ConnectorManager.Connectors;
                        foreach (Connector con in elemCons)
                        {
                            var linkCons = con.AllRefs;
                            foreach (Connector linkCon in linkCons)
                            {
                                if (linkCon.ConnectorType == ConnectorType.Logical)
                                {
                                    continue;
                                }
                                else if (linkCon.Owner.Id == elem.Id)
                                {
                                    continue;
                                }
                                neighborElemIds.Add(linkCon.Owner.Id);
                            }
                        }
                    }
                    outDic.Add(elem.Id, neighborElemIds);
                }
                return outDic;
            }
        }
    }
}
