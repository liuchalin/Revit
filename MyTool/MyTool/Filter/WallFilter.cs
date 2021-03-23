using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MyTool.Filter
{
    class WallFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Wall;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
