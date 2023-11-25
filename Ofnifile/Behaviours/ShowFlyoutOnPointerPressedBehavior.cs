using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Ofnifile.Behaviours;

public class ShowFlyoutOnPointerPressedBehavior : Behavior<Control>
{
    private static FlyoutBase? _lastOpenedFlyout = null;

    protected override void OnAttached()
    {
        AssociatedObject!.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, 
            Avalonia.Interactivity.RoutingStrategies.Tunnel);
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        base.OnDetaching();
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        var props = e.GetCurrentPoint(AssociatedObject!).Properties;
        if (!props.IsRightButtonPressed || FlyoutBase.GetAttachedFlyout(AssociatedObject!) is not { } flyout)
            return;

        // Close previously opened flyout (if exists)
        // Actually this is only for TreeDataGrid
        _lastOpenedFlyout?.Hide();

        flyout.ShowAt(AssociatedObject!);
        _lastOpenedFlyout = flyout;

        // In case of inner child flyout - uncomment this
        //var childWithFlyout = AssociatedObject!.GetChildControlWithFlyout();
        //if (childWithFlyout is { })
        //    FlyoutBase.ShowAttachedFlyout(childWithFlyout);
    }
}
