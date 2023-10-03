using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Models;
using ReactiveUI;
using System;

namespace Ofnifile.ViewModels;

public class ExplorerVM : ReactiveObject, IDisposable
{
    private readonly IDisposable _selectedPathSub;

    private string _selectedPath;
    private IExplorerItem? _root;

    public string SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; }

    public ExplorerVM(string selectedPath)
    {
        _selectedPath = selectedPath;

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
                    hasChildrenSelector: x => x.HasChildren && x.Path == _root!.Path,
                    isExpandedSelector: x => x.IsExpanded),
                new TextColumn<IExplorerItem, long>(
                    header: "Size",
                    getter: x => x.Size,
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Size),
                        CompareDescending = Comparisons.SortDescending(x => x.Size),
                    }),
                new TextColumn<IExplorerItem, DateTimeOffset>(
                    header: "Created",
                    getter: x => x.Created,
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Created),
                        CompareDescending = Comparisons.SortDescending(x => x.Created),
                    }),
                new TextColumn<IExplorerItem, DateTimeOffset>(
                    header: "Modified",
                    getter: x => x.Modified,
                    options: new TextColumnOptions<IExplorerItem>()
                    {
                        CompareAscending = Comparisons.SortAscending(x => x.Modified),
                        CompareDescending = Comparisons.SortDescending(x => x.Modified),
                    })
            }
        };

        TreeSource.RowSelection!.SingleSelect = false;

        _selectedPathSub = this.WhenAnyValue(x => x.SelectedPath).Subscribe(x =>
        {
            _root?.Dispose();
            _root = new ExplorerItemModel(SelectedPath, isDirectory: true, isRoot: true);
            TreeSource.Items = new[] { _root };
        });
    }

    public void Dispose()
    {
        _selectedPathSub.Dispose();
        _root?.Dispose();
        TreeSource.Dispose();
    }
}
