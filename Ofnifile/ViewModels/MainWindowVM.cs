using Ofnifile.ViewModels.TabFeed;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;

namespace Ofnifile.ViewModels;

public class MainWindowVM : ReactiveObject, IDisposable
{
    private readonly Interfaces.MessageBus.IMessageBus _messageBus;
    private readonly IDisposable _quickVmPathSub;
    private readonly IDisposable _explorerPathSub;

    private Stack<string> _undoPathHistory = new();
    private Stack<string> _redoPathHistory = new();
    private string? _selectedPath;
    private bool _disposed;

    public QuickAccessVM QuickAccessVM { get; init; }
    public TabFeedVM TabFeedVM { get; init; }
    public ExplorerVM ExplorerVM { get; init; }

    public string? SelectedPath
    {
        get => _selectedPath;
        set
        {
            if (!string.IsNullOrEmpty(_selectedPath) 
                && (_undoPathHistory.Count == 0 || _undoPathHistory.Peek() != value))
            {
                _undoPathHistory.Push(_selectedPath);
                _redoPathHistory.Clear();
            }

            this.RaiseAndSetIfChanged(ref _selectedPath, value);
        }
    }

    public IList<string> Drives { get; }

    public ReactiveCommand<Unit, Unit> UndoPathCommand { get; }
    public ReactiveCommand<Unit, Unit> RedoPathCommand { get; }

    public MainWindowVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        _messageBus = messageBus;
        Drives = DriveInfo.GetDrives().Select(d => d.Name).ToList();

        _selectedPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "C:\\" 
            : Drives.FirstOrDefault() ?? "/";

        QuickAccessVM = new QuickAccessVM(Drives, _selectedPath, _messageBus);
        ExplorerVM = new ExplorerVM(_selectedPath, _messageBus);
        TabFeedVM = new TabFeedVM(_messageBus);

        _quickVmPathSub = QuickAccessVM.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);
        _explorerPathSub = ExplorerVM.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);

        UndoPathCommand = ReactiveCommand.Create(UndoPath);
        RedoPathCommand = ReactiveCommand.Create(RedoPath);
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

    private void UndoPath()
    {
        if (_undoPathHistory.Count == 0 || _undoPathHistory.Peek() == _selectedPath)
            return;

        SelectedPathHasChanged(_undoPathHistory.Peek());
        _undoPathHistory.Pop();
    }

    private void RedoPath()
    {
        if (_redoPathHistory.Count == 0 || _redoPathHistory.Peek() == _selectedPath)
            return;

        // TODO: Implement Redo logic
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
