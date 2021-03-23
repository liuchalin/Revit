using System;

namespace MyTool.ViewModel
{
    public class VM_ArrangeCableTrayHanger : NotifyPropertyBase
    {
        public double Span { get; set; }

        private bool _toConfirm;
        public bool ToConfirm
        {
            get { return _toConfirm; }
            set
            {
                _toConfirm = value;
                OnPropertyChanged("ToConfirm");
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

        private CommandBase _confirmCmd;
        public CommandBase ConfirmCmd
        {
            get
            {
                if (_confirmCmd == null)
                {
                    _confirmCmd = new CommandBase(new Action<object>((o) =>
                    {
                        ToConfirm = true;
                    }));
                }
                return _confirmCmd;
            }
        }

        private CommandBase _closeCmd;
        public CommandBase CloseCmd
        {
            get
            {
                if (_closeCmd == null)
                {
                    _closeCmd = new CommandBase(new Action<object>((o) =>
                      {
                          ToClose = true;
                      }));
                }
                return _closeCmd;
            }
        }

        public VM_ArrangeCableTrayHanger()
        {
            Span = 2000;
        }
    }
}
