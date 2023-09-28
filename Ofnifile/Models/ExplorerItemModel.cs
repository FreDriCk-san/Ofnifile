using Avalonia.Collections;
using Avalonia.Threading;
using Ofnifile.Extensions;
using Ofnifile.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ofnifile.Models;

public class ExplorerItemModel : ReactiveObject, IExplorerItem
{
    private readonly FileSystemWatcher _watcher;

    private string _path;
    private string _name;
    private long _size;
    private DateTimeOffset _modified;
    private DateTimeOffset _created;
    private bool _hasChildren;
    private bool _isExpanded;
    private AvaloniaList<ExplorerItemModel>? _children;

    public string Path
    {
        get => _path;
        private set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public string Name
    {
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public long Size
    {
        get => _size;
        private set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public DateTimeOffset Modified
    {
        get => _modified; 
        private set => this.RaiseAndSetIfChanged(ref _modified, value);
    }

    public DateTimeOffset Created
    {
        get => _created;
        private set => this.RaiseAndSetIfChanged(ref _created, value);
    }

    public bool HasChildren
    {
        get => _hasChildren; 
        private set => this.RaiseAndSetIfChanged(ref _hasChildren, value);
    }

    public bool IsExpanded 
    {
        get => _isExpanded; 
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value); 
    }

    public bool IsDirectory { get; }
    public bool IsHidden { get; }

    public IReadOnlyList<IExplorerItem>? Children => _children ??= LoadChildren();

    public ExplorerItemModel(string path, bool isDirectory, bool isRoot = false)
    {
        _path = path;
        _name = isRoot ? path : System.IO.Path.GetFileName(path);
        _isExpanded = isRoot;
        IsDirectory = isDirectory;
        HasChildren = isDirectory;

        if (isDirectory)
        {
            var directoryInfo = new DirectoryInfo(path);
            Size = directoryInfo.Size();
            Modified = directoryInfo.LastWriteTimeUtc;
            Created = directoryInfo.CreationTime;
        }
        else
        {
            var fileInfo = new FileInfo(path);
            Size = fileInfo.Length;
            Modified = fileInfo.LastWriteTimeUtc;
            Created = fileInfo.CreationTime;
        }

        _watcher = new FileSystemWatcher
        {
            Path = _path,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.LastWrite,
            EnableRaisingEvents = true,
        };

        _watcher.Changed += ItemChanged;
        _watcher.Created += ItemCreated;
        _watcher.Deleted += ItemDeleted;
        _watcher.Renamed += ItemRenamed;
    }

    private void ItemChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType is WatcherChangeTypes.Changed 
            && (File.Exists(e.FullPath) 
            || Directory.Exists(e.FullPath)))
        {
            Dispatcher.UIThread.Post(() =>
            {
                var child = _children!.FirstOrDefault(child => child.Path == e.FullPath);
                if (child is { })
                {
                    if (child.IsDirectory)
                    {
                        var directoryInfo = new DirectoryInfo(e.FullPath);
                        child.Size = directoryInfo.Size();
                        child.Modified = directoryInfo.LastWriteTimeUtc;
                        child.Created = directoryInfo.CreationTime;
                    }
                    else
                    {
                        var fileInfo = new FileInfo(e.FullPath);
                        child.Size = fileInfo.Length;
                        child.Modified = fileInfo.LastWriteTimeUtc;
                        child.Created = fileInfo.CreationTime;
                    }
                }
            });
        }
    }

    private void ItemCreated(object sender, FileSystemEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var isDirectory = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
            var child = new ExplorerItemModel(e.FullPath, isDirectory);
            _children!.Add(child);
        });
    }

    private void ItemDeleted(object sender, FileSystemEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            for (int i = 0; i < _children!.Count; ++i)
            {
                if (_children[i].Path == e.FullPath)
                {
                    // RemoveAt is a little faster than Remove
                    _children.RemoveAt(i);
                    break;
                }
            }
        });
    }

    private void ItemRenamed(object sender, RenamedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var child = _children!.FirstOrDefault(child => child.Path == e.OldFullPath);
            if (child is { })
            {
                child.Path = e.FullPath;
                child.Name = e.Name ?? "";
            }
        });
    }

    private AvaloniaList<ExplorerItemModel> LoadChildren()
    {
        var children = new AvaloniaList<ExplorerItemModel>();
        var options = new EnumerationOptions { IgnoreInaccessible = true };

        foreach (var dir in Directory.EnumerateDirectories(Path, "*", options))
            children.Add(new ExplorerItemModel(dir, true));

        foreach (var file in Directory.EnumerateFiles(Path, "*", options))
            children.Add(new ExplorerItemModel(file, false));

        return children;
    }

    public void BeginEdit()
    {
        throw new NotImplementedException();
    }

    public void CancelEdit()
    {
        throw new NotImplementedException();
    }

    public void EndEdit()
    {
        throw new NotImplementedException();
    }

    public bool Copy()
    {
        throw new NotImplementedException();
    }

    public bool Cut()
    {
        throw new NotImplementedException();
    }

    public bool Delete()
    {
        throw new NotImplementedException();
    }

    public bool Rename()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    public static Comparison<ExplorerItemModel?> SortAscending<T>(Func<ExplorerItemModel, T> selector)
    {
        return (x, y) =>
        {
            if (x is null && y is null)
                return 0;
            else if (x is null)
                return -1;
            else if (y is null)
                return 1;
            if (x.IsDirectory == y.IsDirectory)
                return Comparer<T>.Default.Compare(selector(x), selector(y));
            else if (x.IsDirectory)
                return -1;
            else
                return 1;
        };
    }

    public static Comparison<ExplorerItemModel?> SortDescending<T>(Func<ExplorerItemModel, T> selector)
    {
        return (x, y) =>
        {
            if (x is null && y is null)
                return 0;
            else if (x is null)
                return 1;
            else if (y is null)
                return -1;
            if (x.IsDirectory == y.IsDirectory)
                return Comparer<T>.Default.Compare(selector(y), selector(x));
            else if (x.IsDirectory)
                return -1;
            else
                return 1;
        };
    }
}
