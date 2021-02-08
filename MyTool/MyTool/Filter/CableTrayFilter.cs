using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

namespace MyTool.Filter
{
    class CableTrayFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is CableTray;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
