using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class ConduitFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Conduit;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}