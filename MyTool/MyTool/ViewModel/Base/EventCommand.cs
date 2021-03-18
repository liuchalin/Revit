using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MyTool.ViewModel
{
    public class EventCommand : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommand), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCommand), new PropertyMetadata(null));

        protected override void Invoke(object parameter)
        {
            if (CommandParameter != null)
            {
                parameter = CommandParameter;
            }
            if (Command != null)
            {
                Command.Execute(parameter);
            }
        }
    }
}
