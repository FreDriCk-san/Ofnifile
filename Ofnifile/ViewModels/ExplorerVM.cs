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

public class ExplorerVM : BaseExplorerVM
{
    private readonly IDisposable _selectedPathSub;

    private IExplorerItem? _root;
    private bool _disposed;

    public ExplorerVM(string? selectedPath, Interfaces.MessageBus.IMessageBus messageBus) 
        : base(selectedPath, messageBus)
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
