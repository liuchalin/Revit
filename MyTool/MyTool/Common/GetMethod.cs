using Autodesk.Revit.DB;

namespace MyTool.Common
{
    public static class GetMethod
    {
        public static Element GetElement(this Reference refe, Document doc)
        {
            return doc.GetElement(refe);
        }

        public static Element GetElement(this ElementId id, Document doc)
        {
            return doc.GetElement(id);
        }
    }
}
