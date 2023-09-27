using Ofnifile.ViewModels.TabFeed;
using ReactiveUI;

namespace Ofnifile.ViewModels;

public class MainWindowVM : ReactiveObject
{
    public string Greeting => "Welcome to Avalonia!";

    private QuickAccessVM _quickAccessVM;
    private TabFeedVM _tabFeedVM;


    public MainWindowVM()
    {
        _quickAccessVM = new QuickAccessVM();
        _tabFeedVM = new TabFeedVM();
    }
}
