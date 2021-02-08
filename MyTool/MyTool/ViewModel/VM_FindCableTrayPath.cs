using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.ViewModel
{
    public class VM_FindCableTrayPath : NotifyPropertyBase
    {        
        public List<List<ElementId>> PathList { get; }
        public List<ElementId> PathElemIds { get; set; }
        public CommandBase CloseCmd { get; set; }
        public bool IsReview { get; set; }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set
            {
                if (_dialogResult != value)
                {
                    _dialogResult = value;
                    OnPropertyChanged("DialogResult");
                }
            }
        }

        public VM_FindCableTrayPath(List<List<ElementId>> pathList)
        {
            PathList = pathList;
            CloseCmd = new CommandBase();
            CloseCmd.DoExecute = new Action<object>(Close);
        }

        private void Close(object obj)
        {
            DialogResult = true;
        }
    }
}
