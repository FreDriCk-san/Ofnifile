using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Models.TreeDataGrid;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ofnifile.ViewModels;

public class ExplorerVM : BaseExplorerVM, IExplorerVM
{
    private readonly IDisposable _selectedPathSub;

    private IExplorerItem? _root;
    private bool _disposed;

    public ExplorerVM(string? selectedPath) 
        : base(selectedPath)
    {
        TreeSource = new HierarchicalTreeDataGridSource<IExplorerItem>(Array.Empty<IExplorerItem>())
        {
            Columns =
            {
                new HierarchicalExpanderColumn<IExplorerItem>(
                    new TemplateColumn<IExplorerItem>(
                        header: "Name",
                        cellTemplateResourceKey: "ItemNameCell",
                        cellEditingTemplateResourceKey: "ItemNameEditCell",
                        width: GridLength.Star,
                        options: new TemplateColumnOptions<IExplorerItem>()
                        {
                            BeginEditGestures = BeginEditGestures.F2,
                            CompareAscending = Comparisons.SortAscending(x => x.Name),
                            CompareDescending = Comparisons.SortDescending(x => x.Name),
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = x => x.Name
                        }),
                    childSelector: x => x.Children,
                    hasChildrenSelector: x => x.HasChildren && x.Path == _root!.Path,
                    isExpandedSelector: x => x.IsExpanded),

                new TextColumn<IExplorerItem, string>(
                    header: "Size",
                    getter: x => x.StringSize,
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Size),
                        CompareDescending = Comparisons.SortDescending(x => x.Size),
                    }),

                new TextColumn<IExplorerItem, string>(
                    header: "Created",
                    getter: x => x.Created.ToLocalTime().ToString("dd.MM.yyyy HH:mm.ss"),
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Created),
                        CompareDescending = Comparisons.SortDescending(x => x.Created),
                    }),

                new TextColumn<IExplorerItem, string>(
                    header: "Modified",
                    getter: x => x.Modified.ToLocalTime().ToString("dd.MM.yyyy HH:mm.ss"),
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Modified),
                        CompareDescending = Comparisons.SortDescending(x => x.Modified),
                    })
            }
        };

        TreeSource.RowSelection!.SingleSelect = false;

        _selectedPathSub = this.WhenAnyValue(x => x.SelectedPath).Subscribe(SelectedPathHasChanged);
    }

    private void SelectedPathHasChanged(string? selectedPath)
    {
        if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
        {
            _selectedPath = _root?.Path;
            return;
        }

        _root?.Dispose();
        _root = new ExplorerItemModel(SelectedPath!, isDirectory: true, isRoot: true);
        TreeSource.Items = new[] { _root };
    }

    public new bool CopySelectedItems()
    {
        return base.CopySelectedItems();
    }

    public new bool PasteSavedItems()
    {
        return base.PasteSavedItems();
    }

    public new bool CutSelectedItems()
    {
        return base.CutSelectedItems();
    }

    public async Task<bool> CopySelectedItemPath()
    {
        var lastSelectedItem = TreeSource.RowSelection!.SelectedItems.LastOrDefault();
        if (lastSelectedItem is null)
            return false;

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is null)
            return false;

        try
        {
            await mainWindow.Clipboard!.SetTextAsync(lastSelectedItem.Path);
        }
        catch { return false; }

        return true;
    }

    public new bool DeleteSelectedItems()
    {
        return base.DeleteSelectedItems();
    }

    public bool RenameSelectedItem()
    {
        throw new NotImplementedException();
    }

    public bool CreateNewFolder()
    {
        throw new NotImplementedException();
    }

    public bool ShowFolderProperties()
    {
        throw new NotImplementedException();
    }

    public bool SelectAllItems()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Select(new IndexPath(0, i - 1));

        return true;
    }

    public bool RemoveSelection()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Deselect(new IndexPath(0, i - 1));

        return true;
    }

    public bool RevertSelection()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
        {
            var indexPath = new IndexPath(0, i - 1);
            if (TreeSource.RowSelection!.IsSelected(indexPath))
                TreeSource.RowSelection!.Deselect(indexPath);
            else
                TreeSource.RowSelection!.Select(indexPath);
        }

        return true;
    }

    public override void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        base.Dispose();

        _selectedPathSub.Dispose();
        _root?.Dispose();
    }
}
