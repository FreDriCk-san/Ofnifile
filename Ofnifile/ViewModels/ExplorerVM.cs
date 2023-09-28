﻿using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Models;
using ReactiveUI;
using System;

namespace Ofnifile.ViewModels;

public class ExplorerVM : ReactiveObject, IDisposable
{
    private string _selectedDrive;
    private string? _selectedPath;
    private IExplorerItem? _root;

    public string SelectedDrive
    {
        get => _selectedDrive;
        set => this.RaiseAndSetIfChanged(ref _selectedDrive, value);
    }

    public string? SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; }

    public ExplorerVM(string selectedDrive)
    {
        _selectedDrive = selectedDrive;

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
        TreeSource.RowSelection.SelectionChanged += TreeSelectionChanged;

        var driveSub = this.WhenAnyValue(x => x.SelectedDrive).Subscribe(x =>
        {
            _root?.Dispose();
            _root = new ExplorerItemModel(_selectedDrive, isDirectory: true, isRoot: true);
            TreeSource.Items = new[] { _root };
        });
    }

    private void TreeSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<IExplorerItem> e)
    {
        SelectedPath = TreeSource.RowSelection?.SelectedItem?.Path;
    }

    public void Dispose()
    {
        TreeSource.RowSelection!.SelectionChanged -= TreeSelectionChanged;
    }
}