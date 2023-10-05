using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace Ofnifile.Behaviours;

public class PreviewDoubleTappedBehaviour : Behavior<InputElement>
{
    public static readonly StyledProperty<bool> HandleEventProperty
        = AvaloniaProperty.Register<PreviewDoubleTappedBehaviour, bool>(nameof(HandleEvent));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PreviewDoubleTappedBehaviour, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PreviewDoubleTappedBehaviour, object?>(nameof(CommandParameter));


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


    protected override void OnAttached()
    {
        AssociatedObject!.DoubleTapped += DoubleTapped;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.DoubleTapped -= DoubleTapped;
        base.OnDetaching();
    }


    private void DoubleTapped(object? sender, TappedEventArgs e)
    {
        e.Handled = HandleEvent;

        if (Command is null)
            return;

        object? resolvedParameter = IsSet(CommandParameterProperty) 
            ? CommandParameter 
            : default;

        Command.Execute(resolvedParameter);
    }
}
