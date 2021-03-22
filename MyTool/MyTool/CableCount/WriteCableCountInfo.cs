using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MyTool.Common;
using MyTool.Filter;
using MyTool.View;
using MyTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Interop;

namespace MyTool.CableCount
{
    [Transaction(TransactionMode.Manual)]
    class WriteCableCountInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Guid cableCountSchemaId = new Guid("8D57AB90-FB70-44E3-9C43-E73A8446304A");
            CableCount(cableCountSchemaId, AccessLevel.Public, AccessLevel.Public);

            List<ElementId> selElemIds = sel.GetElementIds().ToList();
            List<Element> elems = new List<Element>();
            List<ElementId> selCableTrayIds = new List<ElementId>();
        #region 选择构件
        CheckSel:
            if (selElemIds.Count != 0)
            {
                Element selElem = null;
                foreach (ElementId id in selElemIds)
                {
                    selElem = id.GetElement(doc);
                    if (selElem is CableTray)
                    {
                        elems.Add(selElem);
                        selCableTrayIds.Add(id);
                    }
                }
                if (elems.Count == 0)
                {
                    TaskDialog.Show("错误", "选择的构件中不包含桥架，请重新选择");
                    selElemIds.Clear();
                    goto CheckSel;
                }
                sel.SetElementIds(selCableTrayIds);
            }
            else
            {
            SelectCableTrays:
                try
                {
                    uiDoc.RefreshActiveView();
                    elems = sel.PickObjects(ObjectType.Element, new CableTrayFilter(), "请多选需要录入信息的桥架构件").Select(p => p.GetElement(doc)).ToList();
                }
                catch (Exception)
                {
                    TaskDialog.Show("取消", "操作已取消");
                    return Result.Cancelled; ;
                }
                if (elems.Count == 0)
                {
                    TaskDialog.Show("错误", "未选取到桥架，请重新选择");
                    goto SelectCableTrays;
                }
            }
            #endregion

            #region 导入电缆数据
            string currentDLLPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string configFilePath = Path.Combine(Path.GetDirectoryName(currentDLLPath), "MyTool.dll.config");
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configFilePath }, ConfigurationUserLevel.None);
            string cableTypeListFilePath = config.AppSettings.Settings["cableTypeListFilePath"].Value;
            DataTable cableDT = CableInfoManager.Instance().ReadCSV(cableTypeListFilePath);
            #endregion

            List<string> typeList = new List<string>();
            #region UI窗口
            for (int i = 0; i < cableDT.Rows.Count; i++)
            {
                typeList.Add(cableDT.Rows[i][0].ToString());
            }
            VM_WriteCableCountInfo vm = new VM_WriteCableCountInfo(typeList);
            WIndow_WriteCableCountInfo window = new WIndow_WriteCableCountInfo(vm);
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ShowDialog();
            #endregion

            #region 输入信息
            while (true)
            {
                if (vm.ToClose)
                {
                    break;
                }
                else if (vm.IsUpdate)
                {
                    vm.IsUpdate = false;
                    AddDate(doc, elems, cableCountSchemaId, vm.CableType, vm.CountNum);
                    window.ShowDialog();
                }
            }
            #endregion

            return Result.Succeeded;
        }

        void AddDate(Document doc, List<Element> elems, Guid cableCountSchemaId, string typeName, int count)
        {
            Transaction trans = new Transaction(doc, "电缆统计信息拓展存储");
            trans.Start();
            for (int i = 0; i < elems.Count; i++)
            {
                Element elem = elems[i];
                Entity ent = elem.GetEntity(Schema.Lookup(cableCountSchemaId));
                if (ent.Schema == null)
                {
                    ent = new Entity(cableCountSchemaId);
                    Dictionary<string, int> newDic = new Dictionary<string, int>();
                    newDic.Add(typeName, count);
                    ent.Set<IDictionary<string, int>>(Schema.Lookup(cableCountSchemaId).GetField("dictionary"), newDic);
                    elem.SetEntity(ent);
                }
                else
                {
                    IDictionary<string, int> existDic = ent.Get<IDictionary<string, int>>("dictionary");
                    if (existDic.ContainsKey(typeName))
                    {
                        existDic[typeName] += count;
                    }
                    else
                    {
                        existDic.Add(typeName, count);
                    }
                    ent.Set<IDictionary<string, int>>(Schema.Lookup(cableCountSchemaId).GetField("dictionary"), existDic);
                    elem.SetEntity(ent);
                }
            }
            trans.Commit();
        }

        void AddDate(Document doc, Element elem, Guid cableCountSchemaId, string typeName, int count)
        {
            Transaction trans = new Transaction(doc, "电缆统计信息拓展存储");
            trans.Start();
            Entity ent = elem.GetEntity(Schema.Lookup(cableCountSchemaId));
            if (ent.Schema == null)
            {
                ent = new Entity(cableCountSchemaId);
                Dictionary<string, int> newDic = new Dictionary<string, int>();
                newDic.Add(typeName, count);
                ent.Set<IDictionary<string, int>>(Schema.Lookup(cableCountSchemaId).GetField("dictionary"), newDic);
                elem.SetEntity(ent);
            }
            else
            {
                IDictionary<string, int> existDic = ent.Get<IDictionary<string, int>>("dictionary");
                if (existDic.ContainsKey(typeName))
                {
                    existDic[typeName] += count;
                }
                else
                {
                    existDic.Add(typeName, count);
                }
                ent.Set<IDictionary<string, int>>(Schema.Lookup(cableCountSchemaId).GetField("dictionary"), existDic);
                elem.SetEntity(ent);
            }
            trans.Commit();
        }

        Schema CableCount(Guid id, AccessLevel readAccessLevel, AccessLevel writeAccessLevel)
        {
            if (Schema.Lookup(id) != null)
            {
                return Schema.Lookup(id);
            }
            SchemaBuilder schemaBuilder = new SchemaBuilder(id);
            schemaBuilder.SetSchemaName("CableCountInfo");
            schemaBuilder.SetDocumentation("在桥架中记录电缆统计信息");
            schemaBuilder.SetReadAccessLevel(readAccessLevel);
            schemaBuilder.SetWriteAccessLevel(writeAccessLevel);
            schemaBuilder.AddMapField("dictionary", typeof(string), typeof(int));
            return schemaBuilder.Finish();
        }
    }
}
