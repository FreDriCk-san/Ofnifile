using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

namespace Ofnifile.Interfaces;

public interface IExplorerItem : IDisposable, IEditableObject
{
    /// <summary>
    /// Item full path
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Item name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Item new name created by control
    /// </summary>
    string? NewName { get; }

    /// <summary>
    /// Item size
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Last modified date
    /// </summary>
    DateTimeOffset Modified { get; }

    /// <summary>
    /// Creation date
    /// </summary>
    DateTimeOffset Created { get; }

    
    /// <summary>
    /// Has children flag
    /// </summary>
    bool HasChildren { get; }

    /// <summary>
    /// Item is expanded
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Item is hidden
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// Item is directory
    /// </summary>
    bool IsDirectory { get; }

    /// <summary>
    /// Inner child items
    /// </summary>
    IReadOnlyList<IExplorerItem>? Children { get; }

    ReactiveCommand<Unit, Unit> CutCommand { get; }
    ReactiveCommand<Unit, Unit> CopyCommand { get; }
    ReactiveCommand<Unit, Unit> PasteCommand { get; }
    ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    ReactiveCommand<Unit, Unit> RenameCommand { get; }


    bool Rename(string? newName);
}
