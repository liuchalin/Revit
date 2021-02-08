using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;

namespace MyTool.Filter
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
                FamilyInstance temp = elem as FamilyInstance;
                if (temp.Category.Name == "电缆桥架配件")
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
