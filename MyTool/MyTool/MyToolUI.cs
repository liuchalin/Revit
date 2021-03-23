using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace MyTool
{
    [Transaction(TransactionMode.Manual)]
    public class MyToolUI : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("我的工具");
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            //功能分区A
            RibbonPanel ribbonPaneA = application.CreateRibbonPanel("我的工具", "分割");

            PushButtonData pushButtonDataA1 = new PushButtonData("MEPSplit", "分割管道", assemblyPath, "MyTool.Split.MEPSplit");
            PushButton pushButtonA1 = ribbonPaneA.AddItem(pushButtonDataA1) as PushButton;
            pushButtonA1.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/cut.png", UriKind.Absolute));
            pushButtonA1.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/cut.png", UriKind.Absolute));
            pushButtonA1.ToolTip = "在指定的位置切割管道";

            //功能分区B
            RibbonPanel ribbonPaneB = application.CreateRibbonPanel("我的工具", "管段");

            PushButtonData pushButtonDataB1 = new PushButtonData("FindCableTrayPath", "桥架路径", assemblyPath, "MyTool.FindPath.FindCableTrayPath");
            PushButton pushButtonB1 = ribbonPaneB.AddItem(pushButtonDataB1) as PushButton;
            pushButtonB1.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/route.png", UriKind.Absolute));
            pushButtonB1.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/route.png", UriKind.Absolute));
            pushButtonB1.ToolTip = "用于显示桥架的路径走向";

            PushButtonData pushButtonDataB2 = new PushButtonData("WriteCableCountInfo", "录入电缆信息", assemblyPath, "MyTool.CableCount.WriteCableCountInfo");
            PushButton pushButtonB2 = ribbonPaneB.AddItem(pushButtonDataB2) as PushButton;
            pushButtonB2.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/WriteCableCountInfo.png", UriKind.Absolute));
            pushButtonB2.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/WriteCableCountInfo.png", UriKind.Absolute));
            pushButtonB2.ToolTip = "用于记录该桥架中敷设的电缆信息";

            PushButtonData pushButtonDataB3 = new PushButtonData("CheckCableCountInfo", "查询电缆信息", assemblyPath, "MyTool.CableCount.CheckCableCountInfo");
            PushButton pushButtonB3 = ribbonPaneB.AddItem(pushButtonDataB3) as PushButton;
            pushButtonB3.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/CheckCableCountInfo.png", UriKind.Absolute));
            pushButtonB3.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/CheckCableCountInfo.png", UriKind.Absolute));
            pushButtonB3.ToolTip = "用于查询该桥架中敷设的电缆信息";

            PushButtonData pushButtonDataB4 = new PushButtonData("FindPipePath", "管道路径", assemblyPath, "MyTool.FindPath.FindPipePath");
            PushButton pushButtonB4 = ribbonPaneB.AddItem(pushButtonDataB4) as PushButton;
            pushButtonB4.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/route.png", UriKind.Absolute));
            pushButtonB4.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/route.png", UriKind.Absolute));
            pushButtonB4.ToolTip = "用于显示桥架的路径走向";

            PushButtonData pushButtonDataB5 = new PushButtonData("SimilarConduit", "标准段", assemblyPath, "MyTool.Arrangement.SimilarConduit");
            PushButton pushButtonB5 = ribbonPaneB.AddItem(pushButtonDataB5) as PushButton;
            pushButtonB5.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/creatsimilar.png", UriKind.Absolute));
            pushButtonB5.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/creatsimilar.png", UriKind.Absolute));
            pushButtonB5.ToolTip = "根据模型线创建标准段";

            PushButtonData pushButtonDataB6 = new PushButtonData("ArrangeCableTrayHanger", "桥架支吊架", assemblyPath, "MyTool.Arrangement.ArrangeCableTrayHanger");
            PushButton pushButtonB6 = ribbonPaneB.AddItem(pushButtonDataB6) as PushButton;
            pushButtonB6.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/hanger.png", UriKind.Absolute));
            pushButtonB6.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/hanger.png", UriKind.Absolute));
            pushButtonB6.ToolTip = "为桥架创建支吊架";

            //功能分区C
            RibbonPanel ribbonPaneC = application.CreateRibbonPanel("我的工具", "开孔");
            PushButtonData pushButtonDataC1 = new PushButtonData("WallAndCableTray", "桥架开孔", assemblyPath, "MyTool.Opening.WallAndCableTray");
            PushButton pushButtonC1 = ribbonPaneC.AddItem(pushButtonDataC1) as PushButton;
            pushButtonC1.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/clip.png", UriKind.Absolute));
            pushButtonC1.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/clip.png", UriKind.Absolute));
            pushButtonC1.ToolTip = "用于桥架在墙上的开洞";

            PushButtonData pushButtonDataC2 = new PushButtonData("WallAndConduit", "线管开孔", assemblyPath, "MyTool.Opening.WallAndConduit");
            PushButton pushButtonC2 = ribbonPaneC.AddItem(pushButtonDataC2) as PushButton;
            pushButtonC2.LargeImage = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/32px/clip.png", UriKind.Absolute));
            pushButtonC2.Image = new BitmapImage(new Uri("pack://application:,,,/MyTool;component/ButtonIcon/16px/clip.png", UriKind.Absolute));
            pushButtonC2.ToolTip = "用于线管在墙上的开洞";

            return Result.Succeeded;
        }
    }
}
