using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Models;
using System;
using System.Collections.Generic;

namespace Ofnifile.ViewModels;

public class QuickAccessVM : BaseExplorerVM
{
    private readonly List<IExplorerItem> _localDrives;
    
    private bool _disposed;

    public Interfaces.MessageBus.IMessageBus MessageBus => _messageBus;

    public QuickAccessVM(IList<string> drives, string? selectedPath, Interfaces.MessageBus.IMessageBus messageBus) 
        : base(selectedPath, messageBus, ExplorerType.QuickAccess)
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
                    hasChildrenSelector: x => x.HasChildren,
                    isExpandedSelector: x => x.IsExpanded),
            }
        };

        TreeSource.RowSelection!.SingleSelect = true;

        _localDrives = new List<IExplorerItem>();
        foreach (var drive in drives)
        {
            var newDrive = new ExplorerItemModel(
                path: drive, 
                isDirectory: true, 
                isRoot: true);
            _localDrives.Add(newDrive);
        }
        TreeSource.Items = _localDrives;
    }


    public override void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        base.Dispose();

        foreach (var drive in _localDrives)
            drive.Dispose();
    }
}
