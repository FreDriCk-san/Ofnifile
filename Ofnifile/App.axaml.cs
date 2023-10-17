using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Ofnifile.Interfaces.MessageBus;
using Ofnifile.Misc.MessageBus;
using Ofnifile.ViewModels;
using Ofnifile.Views;

namespace Ofnifile
{
    public partial class App : Application
    {
        private readonly IMessageBus _messageBus;

        public App()
        {
            _messageBus = new MessageBus();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowVM(_messageBus),
                };
                desktop.Exit += Desktop_Exit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            desktop.Exit -= Desktop_Exit;
            
            var mainWindowVm = (MainWindowVM)desktop.MainWindow!.DataContext!;
            mainWindowVm.Dispose();

            _messageBus.Dispose();
        }
    }
}