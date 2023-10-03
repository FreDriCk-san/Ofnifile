using Ofnifile.ViewModels.TabFeed;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ofnifile.ViewModels;

public class MainWindowVM : ReactiveObject, IDisposable
{
    private string _selectedPath;
    private bool _disposed;

    public QuickAccessVM QuickAccessVM { get; init; }
    public TabFeedVM TabFeedVM { get; init; }
    public ExplorerVM ExplorerVM { get; init; }

    public IList<string> Drives { get; }

    public MainWindowVM()
    {
        Drives = DriveInfo.GetDrives().Select(d => d.Name).ToList();

        _selectedPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "C:\\" 
            : Drives.FirstOrDefault() ?? "/";

        QuickAccessVM = new QuickAccessVM(Drives);
        TabFeedVM = new TabFeedVM();
        ExplorerVM = new ExplorerVM(_selectedPath);
    }

    public void Dispose()
    {
        if (_disposed) 
            return;
        _disposed = true;

        ExplorerVM.Dispose();
        QuickAccessVM.Dispose();
    }
}
