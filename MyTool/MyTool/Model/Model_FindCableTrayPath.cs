using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace MyTool
{
    public class Model_FindCableTrayPath
    {
        public int Index { get; set; }
        public List<ElementId> ElemIds { get; set; }
        public double PathLength { get; set; }

        public Model_FindCableTrayPath(List<ElementId> path, double length)
        {
            PathLength = length * 0.3048;
            ElemIds = path;
        }
    }
}