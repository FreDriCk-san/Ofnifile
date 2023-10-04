using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace Ofnifile.Behaviours;

public class PreviewKeyDownBehaviour : Behavior<InputElement>
{
    public static readonly StyledProperty<bool> HandleEventProperty
        = AvaloniaProperty.Register<PreviewDoubleTappedBehaviour, bool>(nameof(HandleEvent));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PreviewKeyDownBehaviour, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PreviewKeyDownBehaviour, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<Key?> DownKeyProperty = 
        AvaloniaProperty.Register<PreviewDoubleTappedBehaviour, Key?>(nameof(DownKey));


    public bool HandleEvent
    {
        get => GetValue(HandleEventProperty);
        set => SetValue(HandleEventProperty, value);
    }

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

    public Key? DownKey
    {
        get => GetValue(DownKeyProperty);
        set => SetValue(DownKeyProperty, value);
    }


    protected override void OnAttached()
    {
        //AssociatedObject!.AddHandler(InputElement.KeyDownEvent, KeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        AssociatedObject!.KeyDown += KeyDown;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        //AssociatedObject!.RemoveHandler(InputElement.KeyDownEvent, KeyDown);
        AssociatedObject!.KeyDown -= KeyDown;
        base.OnDetaching();
    }

    private void KeyDown(object? sender, KeyEventArgs e)
    {
        if (DownKey.GetValueOrDefault(e.Key) != e.Key)
            return;

        e.Handled = HandleEvent;

        if (Command is null)
            return;

        object? resolvedParameter = IsSet(CommandParameterProperty)
            ? CommandParameter
            : default;

        Command.Execute(resolvedParameter);
    }
}
