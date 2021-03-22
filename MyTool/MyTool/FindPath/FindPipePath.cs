using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.Common;
using MyTool.Filter;
using MyTool.Model;
using MyTool.View;
using MyTool.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace MyTool.FindPath
{
    [Transaction(TransactionMode.Manual)]
    class FindPipePath : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            Element startElem = null;
            Element endElem = null;
            List<Element> allCandidateElems = new List<Element>();
            #region 选择起点管道，终点管道，管道网络
            try
            {
                startElem = sel.PickObject(ObjectType.Element, new PipeFilter(), "选择起始管道").GetElement(doc);
                endElem = sel.PickObject(ObjectType.Element, new PipeFilter(), "选择终点管道").GetElement(doc);
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
        SelectPipeLayout:
            try
            {
                allCandidateElems = sel.PickObjects(ObjectType.Element, new PipePathFilter(), "请框选管道网络").Select(p => p.GetElement(doc)).ToList();
            }
            catch (OperationCanceledException)
            {
                TaskDialog.Show("取消", "操作已取消");
                return Result.Cancelled;
            }
            if (allCandidateElems.Count == 0)
            {
                TaskDialog.Show("错误", "未选到网络，请重新选择");
                goto SelectPipeLayout;
            }
            #endregion

            #region 元素邻接表，路径BFS算法
            Dictionary<ElementId, List<ElementId>> neighborKVPairs = GetNeighborList(doc, allCandidateElems);
            List<List<ElementId>> pathList = GetAllPath(doc, neighborKVPairs, startElem.Id, endElem.Id);
            #endregion

            List<Model_FindPipePath> models = new List<Model_FindPipePath>();
            foreach (var path in pathList)
            {
                models.Add(new Model_FindPipePath(path, GetPathLength(doc, path)));
            }
            models = models.OrderBy(p => p.PathLength).ToList();
            foreach (var item in models)
            {
                item.Index = models.IndexOf(item) + 1;
            }
            #region WPF窗口，MVVM模式
            VM_FindPipePath vm = new VM_FindPipePath(models);
            Window_FindPipePath win = new Window_FindPipePath(vm);
            WindowInteropHelper helper = new WindowInteropHelper(win);
            helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            win.ShowDialog();

            List<ElementId> reviewElem = new List<ElementId>();
            while (true)
            {
                if (vm.ToClose)
                {
                    break;
                }
                else if (vm.ToHide)
                {
                    vm.ToHide = false;
                    reviewElem = vm.Path.ElemIds;
                    sel.SetElementIds(reviewElem);
                    uiDoc.RefreshActiveView();
                    win.ShowDialog();
                }
            }
            #endregion

            return Result.Succeeded;
        }

        double GetPathLength(Document doc, List<ElementId> route)
        {
            double length = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                length += GetCenterPoint(route[i].GetElement(doc)).DistanceTo(GetCenterPoint(route[i + 1].GetElement(doc)));
            }
            Curve startCurve = (route.First().GetElement(doc).Location as LocationCurve).Curve;
            Curve endCurve = (route.Last().GetElement(doc).Location as LocationCurve).Curve;
            length = length + startCurve.Length / 2 + endCurve.Length / 2;
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
