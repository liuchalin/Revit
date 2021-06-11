using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class CableTrayPathFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CableTray)
            {
                return true;
            }
            else if (elem is FamilyInstance)
            {
                int categoryID = (elem as FamilyInstance).Category.Id.IntegerValue;
                if (categoryID == (int)BuiltInCategory.OST_CableTrayFitting)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
