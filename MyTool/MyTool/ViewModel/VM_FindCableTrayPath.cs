using System;
using System.Collections.Generic;

namespace MyTool.ViewModel
{
    public class VM_FindCableTrayPath : NotifyPropertyBase
    {
        public List<Model_FindCableTrayPath> PathList { get; set; }
        public Model_FindCableTrayPath Path { get; set; }

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

        private bool _toHide;
        public bool ToHide
        {
            get { return _toHide; }
            set
            {
                _toHide = value;
                OnPropertyChanged("ToHide");
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

        private CommandBase _reviewSelection;
        public CommandBase ReviewSelection
        {
            get
            {
                if (_reviewSelection == null)
                {
                    _reviewSelection = new CommandBase(new Action<object>(o =>
                    {
                        ToHide = true;
                    }));
                }
                return _reviewSelection;
            }
        }

        public VM_FindCableTrayPath(List<Model_FindCableTrayPath> models)
        {
            PathList = models;
        }
    }
}
