using Autodesk.Revit.UI;
using System;

namespace MyTool
{
    class RevitEventHandler : IExternalEventHandler
    {
        public string Name { get; set; }
        public Action<UIApplication> DoExecute { get; set; }

        public void Execute(UIApplication app)
        {
            DoExecute?.Invoke(app);
        }

        public string GetName()
        {
            return Name;
        }

        public RevitEventHandler(string name)
        {
            Name = name;
        }
    }
}
