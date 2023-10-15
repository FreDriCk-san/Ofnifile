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
    private readonly IDisposable _quickVmPathSub;
    private readonly IDisposable _explorerPathSub;

    private string? _selectedPath;
    private bool _disposed;

    public QuickAccessVM QuickAccessVM { get; init; }
    public TabFeedVM TabFeedVM { get; init; }
    public ExplorerVM ExplorerVM { get; init; }

    public string? SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public IList<string> Drives { get; }

    public MainWindowVM()
    {
        Drives = DriveInfo.GetDrives().Select(d => d.Name).ToList();

        _selectedPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "C:\\" 
            : Drives.FirstOrDefault() ?? "/";

        TabFeedVM = new TabFeedVM();
        QuickAccessVM = new QuickAccessVM(Drives, _selectedPath);
        ExplorerVM = new ExplorerVM(_selectedPath);

        _quickVmPathSub = QuickAccessVM.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);
        _explorerPathSub = ExplorerVM.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);
    }

    private void SelectedPathHasChanged(string? selectedPath)
    {
        // TODO: Sync QuickVm selection with ExplorerVm?
        if (QuickAccessVM.SelectedPath != selectedPath)
            QuickAccessVM.SelectedPath = selectedPath;

        if (ExplorerVM.SelectedPath != selectedPath)
            ExplorerVM.SelectedPath = selectedPath;

        if (SelectedPath != selectedPath)
            SelectedPath = selectedPath;
    }

    public void Dispose()
    {
        if (_disposed) 
            return;
        _disposed = true;

        _quickVmPathSub.Dispose();
        _explorerPathSub.Dispose();

        ExplorerVM.Dispose();
        QuickAccessVM.Dispose();
    }
}
