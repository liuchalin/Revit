using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.ViewModel
{
    public class VM_FindCableTrayPath
    {
        public string PathInfo { get; }
        public List<ElementId> PathList { get; }
        public VM_FindCableTrayPath(List<ElementId>)
        {

        }

    }
}
