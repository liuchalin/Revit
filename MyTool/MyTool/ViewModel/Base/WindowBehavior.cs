using System.Windows;
using System.Windows.Interactivity;

namespace MyTool.ViewModel
{
    /// <summary>
    /// 窗口行为
    /// </summary>
    public class WindowBehavior : Behavior<Window>
    {
        //关闭
        public bool Close
        {
            get { return (bool)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(false, OnCloseChanged));

        private static void OnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = (d as WindowBehavior).AssociatedObject;
            if ((bool)e.NewValue)
            {
                window.Close();
            }
        }

        //隐藏
        public bool Hide
        {
            get { return (bool)GetValue(HideProperty); }
            set { SetValue(HideProperty, value); }
        }

        public static readonly DependencyProperty HideProperty =
            DependencyProperty.Register("Hide", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(false, OnHideChanged));

        private static void OnHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = (d as WindowBehavior).AssociatedObject;
            if ((bool)e.NewValue)
            {
                window.Visibility = Visibility.Hidden;
            }
        }
    }
}
