﻿using Avalonia.Collections;
using Avalonia.Threading;
using Ofnifile.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;

namespace Ofnifile.Models;

public class ExplorerItemModel : ReactiveObject, IExplorerItem
{
    private readonly FileSystemWatcher _watcher;

    private string _path;
    private string _name;
    private string? _newName;
    private long _size;
    private DateTimeOffset _modified;
    private DateTimeOffset _created;
    private bool _hasChildren;
    private bool _isExpanded;
    private bool _isHidden;
    private AvaloniaList<ExplorerItemModel>? _children;
    private bool _disposed;

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

    public string? NewName
    {
        get => _newName;
        private set => this.RaiseAndSetIfChanged(ref _newName, value);
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

    public bool IsHidden
    {
        get => _isHidden;
        set => this.RaiseAndSetIfChanged(ref _isHidden, value);
    }

    public bool IsDirectory { get; }

    public IReadOnlyList<IExplorerItem>? Children => _children ??= LoadChildren();

    public ReactiveCommand<Unit, Unit> CutCommand { get; }

    public ReactiveCommand<Unit, Unit> CopyCommand { get; }

    public ReactiveCommand<Unit, Unit> PasteCommand { get; }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> RenameCommand { get; }


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
            //Size = directoryInfo.Size();
            Modified = directoryInfo.LastWriteTime;
            Created = directoryInfo.CreationTime;
            IsHidden = directoryInfo.Attributes.HasFlag(FileAttributes.Hidden);

            _watcher = new FileSystemWatcher
            {
                Path = _path,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.LastWrite,
                EnableRaisingEvents = true,
            };
        }
        else
        {
            var fileInfo = new FileInfo(path);
            Size = fileInfo.Length;
            Modified = fileInfo.LastWriteTime;
            Created = fileInfo.CreationTime;
            IsHidden = fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            _watcher = new FileSystemWatcher
            {
                Path = System.IO.Path.GetDirectoryName(path)!,
                Filter = System.IO.Path.GetFileName(path),
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.LastWrite,
                EnableRaisingEvents = true,
            };
        }

        _watcher.Changed += ItemChanged;
        _watcher.Created += ItemCreated;
        _watcher.Deleted += ItemDeleted;
        _watcher.Renamed += ItemRenamed;

        CutCommand = ReactiveCommand.Create(Cut);
        CopyCommand = ReactiveCommand.Create(Copy);
        PasteCommand = ReactiveCommand.Create(Paste);
        DeleteCommand = ReactiveCommand.Create(Delete);
        RenameCommand = ReactiveCommand.Create(Rename);
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
                        //child.Size = directoryInfo.Size();
                        child.Modified = directoryInfo.LastWriteTime;
                        child.Created = directoryInfo.CreationTime;
                        child.IsHidden = directoryInfo.Attributes.HasFlag(FileAttributes.Hidden);
                    }
                    else
                    {
                        var fileInfo = new FileInfo(e.FullPath);
                        child.Size = fileInfo.Length;
                        child.Modified = fileInfo.LastWriteTime;
                        child.Created = fileInfo.CreationTime;
                        child.IsHidden = fileInfo.Attributes.HasFlag(FileAttributes.Hidden);
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
                    _children[i].Dispose();
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
            if (!IsDirectory)
                return;

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
        if (!IsDirectory)
            return children;

        var options = new EnumerationOptions 
        { 
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Temporary
        };

        foreach (var dir in Directory.EnumerateDirectories(Path, "*", options))
            children.Add(new ExplorerItemModel(dir, true));

        foreach (var file in Directory.EnumerateFiles(Path, "*", options))
            children.Add(new ExplorerItemModel(file, false));

        return children;
    }

    public void BeginEdit()
    {
        _newName = Name;
    }

    public void CancelEdit()
    {
        _newName = null;
    }

    public void EndEdit()
    {
        Rename(_newName);
        _newName = null;
    }

    private void Cut()
    {

    }

    private void Copy()
    {

    }

    private void Paste()
    {

    }

    private void Delete()
    {

    }

    private void Rename()
    {

    }

    public bool Rename(string? newName)
    {
        if (string.IsNullOrEmpty(newName) || newName == Name)
            return false;

        string parentPath;
        string newPath;
        if (IsDirectory)
        {
            parentPath = new DirectoryInfo(Path).Parent!.FullName;
            newPath = System.IO.Path.Combine(parentPath, newName);

            if (Directory.Exists(newPath))
            {
                Debug.Fail($"Path {newPath} already exists!");
                return false;
            }

            try
            {
                Directory.Move(Path, newPath);
            }
            catch (Exception exception)
            {
                Debug.Fail(exception.Message);
                return false;
            }
        }
        else
        {
            var fileInfo = new FileInfo(Path);
            parentPath = fileInfo.Directory!.FullName;
            var extension = fileInfo.Extension;
            if (!newName.Contains(extension))
                newName = $"{newName}.{extension}";
            newPath = System.IO.Path.Combine(parentPath, newName);

            if (File.Exists(newPath))
            {
                Debug.Fail($"Path {newPath} already exists!");
                return false;
            }

            try
            {
                File.Move(Path, newPath);
            }
            catch (Exception exception)
            {
                Debug.Fail(exception.Message);
                return false;
            }
        }

        return true;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        _watcher.Dispose();

        if (_children is { })
        {
            foreach (var child in _children)
                child.Dispose();
        }

        CutCommand.Dispose();
        CopyCommand.Dispose();
        PasteCommand.Dispose();
        DeleteCommand.Dispose();
        RenameCommand.Dispose();
    }
}
