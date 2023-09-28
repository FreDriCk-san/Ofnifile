using Ofnifile.ViewModels.TabFeed;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ofnifile.ViewModels;

public class MainWindowVM : ReactiveObject
{
    public QuickAccessVM QuickAccessVM { get; init; }
    public TabFeedVM TabFeedVM { get; init; }
    public ExplorerVM ExplorerVM { get; init; }

    private string _selectedDrive;

    public IList<string> Drives { get; }

    public MainWindowVM()
    {
        Drives = DriveInfo.GetDrives().Select(d => d.Name).ToList();

        _selectedDrive = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "C:\\" 
            : Drives.FirstOrDefault() ?? "/";

        QuickAccessVM = new QuickAccessVM();
        TabFeedVM = new TabFeedVM();
        ExplorerVM = new ExplorerVM(_selectedDrive);
    }
}
