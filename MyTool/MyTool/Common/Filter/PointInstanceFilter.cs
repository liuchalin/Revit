using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class PointInstanceFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                if ((elem as FamilyInstance).Location is LocationPoint)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
