using System;
using System.Collections.Generic;
using System.Windows;

namespace MyTool.ViewModel
{
    public class VM_WriteCableCountInfo : NotifyPropertyBase
    {
        public int CountNum { get; set; }
        public string CableType { get; set; }
        public List<string> CableTypeList { get; set; }

        private bool _isUpdate;
        public bool IsUpdate
        {
            get { return _isUpdate; }
            set
            {
                _isUpdate = value;
                OnPropertyChanged("IsUpdate");
            }
        }

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

        private CommandBase _addCmd;
        public CommandBase AddCmd
        {
            get
            {
                if (_addCmd == null)
                {
                    _addCmd = new CommandBase(new Action<object>(Write));
                }
                return _addCmd;
            }
        }

        private void Write(object obj)
        {
            if (CableType == null)
            {
                MessageBox.Show("未选择电缆类型，请重新选择");
            }
            else
            {
                IsUpdate = true;
                MessageBox.Show("统计信息已添加到桥架中");                
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

        public VM_WriteCableCountInfo(List<string> typelist)
        {
            CountNum = 1;
            CableTypeList = typelist;
        }
    }
}
