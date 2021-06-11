using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class ConduitAndModelLineFIlter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return (elem is Conduit || elem is ModelLine);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
