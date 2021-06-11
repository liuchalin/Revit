using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;

namespace MyTool
{
    class PipePathFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Pipe)
            {
                return true;
            }
            else if (elem is FamilyInstance)
            {
                int categoryID = (elem as FamilyInstance).Category.Id.IntegerValue;
                if (categoryID == (int)BuiltInCategory.OST_PipeAccessory || categoryID == (int)BuiltInCategory.OST_PipeFitting)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
