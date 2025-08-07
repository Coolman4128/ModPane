using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace ModPane.Behaviors
{
    public class DoubleClickBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<DoubleClickBehavior, ICommand?>(nameof(Command));

        public static readonly StyledProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<DoubleClickBehavior, object?>(nameof(CommandParameter));

        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttachedToVisualTree()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DoubleTapped += OnDoubleTapped;
            }
        }

        protected override void OnDetachedFromVisualTree()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DoubleTapped -= OnDoubleTapped;
            }
        }

        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
