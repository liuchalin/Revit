using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace MyTool.Model
{
    public class Model_FindPipePath
    {
        public int Index { get; set; }
        public List<ElementId> ElemIds { get; set; }
        public double PathLength { get; set; }

        public Model_FindPipePath(List<ElementId> path, double length)
        {
            PathLength = length * 0.3048;
            ElemIds = path;
        }
    }
}