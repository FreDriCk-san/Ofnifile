using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Ofnifile.Interfaces;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;

namespace Ofnifile.ViewModels;

public class BaseExplorerVM : ReactiveObject, IDisposable
{
    protected string? _selectedPath;
    private bool _disposed;

    public string? SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; init; }

    public ReactiveCommand<Unit, Unit> ChangeSelectedPathCommand { get; }
    public ReactiveCommand<Unit, Unit> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<TreeDataGridTemplateCell?, Unit> RenameSelectedItemCommand { get; }


    public BaseExplorerVM(string? selectedPath)
    {
        _selectedPath = selectedPath;

        ChangeSelectedPathCommand = ReactiveCommand.Create(ChangeSelectedPath);
        CutSelectedItemsCommand = ReactiveCommand.Create(CutSelectedItems);
        CopySelectedItemsCommand = ReactiveCommand.Create(CopySelectedItems);
        PasteSavedItemsCommand = ReactiveCommand.Create(PasteSavedItems);
        DeleteSelectedItemsCommand = ReactiveCommand.Create(DeleteSelectedItems);
        RenameSelectedItemCommand = ReactiveCommand.Create<TreeDataGridTemplateCell?>(RenameSelectedItem);
    }

    private void ChangeSelectedPath()
    {
        var lastSelectedItem = TreeSource.RowSelection!.SelectedItems.LastOrDefault();
        if (lastSelectedItem is null)
            return;

        SelectedPath = lastSelectedItem.Path;
    }

    private void CutSelectedItems()
    {

    }

    private void CopySelectedItems()
    {

    }

    private void PasteSavedItems()
    {

    }

    private void DeleteSelectedItems()
    {
        var selectedItems = TreeSource.RowSelection!.SelectedItems!;
        foreach (var item in selectedItems)
            item!.Delete();
    }

    private void RenameSelectedItem(TreeDataGridTemplateCell? cell)
    {
        if (cell is null) 
            return;

        var keyEvent = new KeyEventArgs
        {
            Source = this,
            Key = Key.F2,
            RoutedEvent = TreeDataGridTemplateCell.KeyDownEvent
        };

        cell.RaiseEvent(keyEvent);
    }

    public virtual void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        TreeSource!.Dispose();
    }
}
