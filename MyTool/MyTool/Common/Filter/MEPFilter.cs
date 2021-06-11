using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class MEPFilter : ISelectionFilter
    {
        Document doc = null;
        public MEPFilter(Document document)
        {
            doc = document;
        }

        public bool AllowElement(Element elem)
        {
            if (elem is CableTray || elem is Conduit || elem is Pipe)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            Element elem = reference.GetElement(doc);
            if (elem is CableTray || elem is Conduit || elem is Pipe)
            {
                return true;
            }
            return false;
        }
    }
}
