using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyTool.ViewModel
{
    public class VM_CheckCableCountInfo : NotifyPropertyBase
    {
        public List<Model_CheckCableCountInfo> CableInfos { get; set; }
        public Model_CheckCableCountInfo cableInfo;
        public double CableWidth { get; set; }
        public double CableHeight { get; set; }
        public double CableLength { get; set; }
        public double SectionArea { get; set; }
        public double TotalUnitWeight { get; set; }
        public double TotalWeight { get; set; }
        public double SingleLayerWidth { get; set; }
        public double MultiLayerSectionArea { get; set; }
        public double FillingRate { get; set; }

        private bool _toClose;
        public bool ToClose
        {
            get { return _toClose; }
            set
            {
                _toClose = value;
                OnPropertyChanged("ToClose");
            }
        }

        private CommandBase _closeCmd;
        public CommandBase CloseCmd
        {
            get
            {
                if (_closeCmd == null)
                {
                    _closeCmd = new CommandBase(new Action<object>(o =>
                      {
                          ToClose = true;
                      }));
                }
                return _closeCmd;
            }
        }

        public VM_CheckCableCountInfo(IDictionary<string, int> data, DataTable dt, double width, double height, double length)
        {
            CableWidth = width;
            CableHeight = height;
            CableLength = length / 1000;
            SectionArea = width * height / 1000000;
            CableInfos = new List<Model_CheckCableCountInfo>();
            Dictionary<string, int> dic = (Dictionary<string, int>)data;
            for (int i = 0; i < data.Count; i++)
            {
                cableInfo = new Model_CheckCableCountInfo();
                cableInfo.TypeName = dic.ElementAt(i).Key;
                cableInfo.Count = dic.ElementAt(i).Value;
                string columnName = dt.Columns[0].ColumnName;
                DataRow dr = dt.Select(columnName + "='" + cableInfo.TypeName + "'").FirstOrDefault();
                cableInfo.OutsideDiameter = Convert.ToDouble(dr[1]);
                cableInfo.UnitWeight = Convert.ToDouble(dr[2]);
                CableInfos.Add(cableInfo);
            }
            TotalUnitWeight = CalTotalUnitWeight(CableInfos);
            TotalWeight = CalTotalWeight(TotalUnitWeight, CableLength);
            SingleLayerWidth = CalSingleLayerWidth(CableInfos);
            MultiLayerSectionArea = CalMultiLayerSectionArea(CableInfos);
            FillingRate = MultiLayerSectionArea / SectionArea * 100;
        }

        double CalMultiLayerSectionArea(List<Model_CheckCableCountInfo> cables)
        {
            double result = 0;
            for (int i = 0; i < cables.Count; i++)
            {
                result += cables[i].Count * cables[i].OutsideDiameter * cables[i].OutsideDiameter / 4;
            }
            return result * 3.14 / 1000000;
        }

        double CalSingleLayerWidth(List<Model_CheckCableCountInfo> cables)
        {
            double result = 0;
            for (int i = 0; i < cables.Count; i++)
            {
                result += cables[i].OutsideDiameter * cables[i].Count * 1.25;
            }
            return result;
        }

        double CalTotalWeight(double totalUnitWeight, double length)
        {
            return totalUnitWeight * length;
        }

        double CalTotalUnitWeight(List<Model_CheckCableCountInfo> cables)
        {
            double result = 0;
            for (int i = 0; i < cables.Count; i++)
            {
                result += cables[i].UnitWeight * cables[i].Count / 1000;
            }
            return result;
        }
    }
}
