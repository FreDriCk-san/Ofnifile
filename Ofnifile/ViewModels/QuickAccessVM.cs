using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace Ofnifile.ViewModels;

public class QuickAccessVM : ReactiveObject, IDisposable
{
    private readonly List<IExplorerItem> _localDrives;
    
    private bool _disposed;

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; }
    public ReactiveCommand<IExplorerItem, Unit> ChangeSelectedPathCommand { get; }

    public QuickAccessVM(IList<string> drives)
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
                        width: new GridLength(1, GridUnitType.Star),
                        options: new TemplateColumnOptions<IExplorerItem>()
                        {
                            CompareAscending = Comparisons.SortAscending(x => x.Name),
                            CompareDescending = Comparisons.SortDescending(x => x.Name),
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = x => x.Name
                        }),
                    childSelector: x => x.Children,
                    hasChildrenSelector: x => x.HasChildren,
                    isExpandedSelector: x => x.IsExpanded),
            }
        };

        TreeSource.RowSelection!.SingleSelect = true;

        _localDrives = new List<IExplorerItem>();
        foreach (var drive in drives)
        {
            var newDrive = new ExplorerItemModel(drive, isDirectory: true, isRoot: true);
            _localDrives.Add(newDrive);
        }
        TreeSource.Items = _localDrives;

        ChangeSelectedPathCommand = ReactiveCommand.Create<IExplorerItem>(ChangeSelectedPath);
    }

    private void ChangeSelectedPath(IExplorerItem explorerItem)
    {
        // TODO: Change selected path
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        foreach (var drive in _localDrives)
            drive.Dispose();

        TreeSource.Dispose();
        ChangeSelectedPathCommand.Dispose();
    }
}
