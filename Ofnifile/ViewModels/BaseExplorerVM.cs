using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Ofnifile.Interfaces;
using Ofnifile.Misc;
using Ofnifile.Misc.MessageBus;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Ofnifile.ViewModels;

public class BaseExplorerVM : ReactiveObject, IDisposable
{
    protected readonly Interfaces.MessageBus.IMessageBus _messageBus;

    protected string? _selectedPath;
    private bool _disposed;

    public string? SelectedPath
    {
        get => _selectedPath;
        set => this.RaiseAndSetIfChanged(ref _selectedPath, value);
    }

    public HierarchicalTreeDataGridSource<IExplorerItem> TreeSource { get; init; }

    public ReactiveCommand<Unit, Unit> ChangeSelectedPathCommand { get; }
    public ReactiveCommand<Unit, bool> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<TreeDataGridTemplateCell?, Unit> RenameSelectedItemCommand { get; }


    public BaseExplorerVM(string? selectedPath, Interfaces.MessageBus.IMessageBus messageBus)
    {
        _selectedPath = selectedPath;
        _messageBus = messageBus;

        ChangeSelectedPathCommand = ReactiveCommand.Create(ChangeSelectedPath);
        CutSelectedItemsCommand = ReactiveCommand.Create(CutSelectedItems);
        CopySelectedItemsCommand = ReactiveCommand.Create(CopySelectedItems);
        PasteSavedItemsCommand = ReactiveCommand.Create(PasteSavedItems);
        DeleteSelectedItemsCommand = ReactiveCommand.Create(DeleteSelectedItems);
        RenameSelectedItemCommand = ReactiveCommand.Create<TreeDataGridTemplateCell?>(RenameSelectedItem);

        // TODO: Subscribe on actions
    }

    private void ChangeSelectedPath()
    {
        var lastSelectedItem = TreeSource.RowSelection!.SelectedItems.LastOrDefault();
        if (lastSelectedItem is null)
            return;

        SelectedPath = lastSelectedItem.Path;
    }

    private bool CutSelectedItems()
    {
        throw new NotImplementedException();
    }

    private bool CopySelectedItems()
    {
        throw new NotImplementedException();
    }

    private bool PasteSavedItems()
    {
        throw new NotImplementedException();
    }

    private bool DeleteSelectedItems()
    {
        var selectedItems = TreeSource.RowSelection!.SelectedItems!.Where(x => !TreeSource.Items.Contains(x));
        if (!selectedItems.Any())
            return false;

        foreach (var item in selectedItems)
            item!.Delete();

        return true;
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
        return;
    }

    private async Task<bool> CopySelectedItemPath()
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

    private bool CreateNewFolder()
    {
        throw new NotImplementedException();
    }

    private bool ShowFolderProperties()
    {
        throw new NotImplementedException();
    }

    private bool SelectAllItems()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Select(new IndexPath(0, i - 1));

        return true;
    }

    private bool RemoveSelection()
    {
        if (TreeSource.Rows.Count == 0)
            return false;

        for (int i = 1; i < TreeSource.Rows.Count; i++)
            TreeSource.RowSelection!.Deselect(new IndexPath(0, i - 1));

        return true;
    }

    private bool RevertSelection()
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

    public virtual void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        TreeSource!.Dispose();
    }
}
