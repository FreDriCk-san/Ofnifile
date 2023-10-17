﻿using ReactiveUI;
using System.Reactive;

namespace Ofnifile.ViewModels.TabFeed;

public class IndexFeedVM : ReactiveObject
{
    private readonly Interfaces.MessageBus.IMessageBus _messageBus;

    public ReactiveCommand<Unit, bool> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> CopySelectedItemPathCommand { get; }
    public ReactiveCommand<Unit, bool> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, bool> RenameSelectedItemCommand { get; }
    public ReactiveCommand<Unit, bool> CreateNewFolderCommand { get; }
    public ReactiveCommand<Unit, bool> ShowFolderPropertiesCommand { get; }
    public ReactiveCommand<Unit, bool> SelectAllItemsCommand { get; }
    public ReactiveCommand<Unit, bool> RemoveSelectionCommand { get; }
    public ReactiveCommand<Unit, bool> RevertSelectionCommand { get; }


    public IndexFeedVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        _messageBus = messageBus;

        //CopySelectedItemsCommand = ReactiveCommand.Create(explorerVM.CopySelectedItems);
        //PasteSavedItemsCommand = ReactiveCommand.Create(explorerVM.PasteSavedItems);
        //CutSelectedItemsCommand = ReactiveCommand.Create(explorerVM.CutSelectedItems);
        //CopySelectedItemPathCommand = ReactiveCommand.CreateFromTask(explorerVM.CopySelectedItemPath);
        //DeleteSelectedItemsCommand = ReactiveCommand.Create(explorerVM.DeleteSelectedItems);
        //RenameSelectedItemCommand = ReactiveCommand.Create(explorerVM.RenameSelectedItem);
        //CreateNewFolderCommand = ReactiveCommand.Create(explorerVM.CreateNewFolder);
        //ShowFolderPropertiesCommand = ReactiveCommand.Create(explorerVM.ShowFolderProperties);
        //SelectAllItemsCommand = ReactiveCommand.Create(explorerVM.SelectAllItems);
        //RemoveSelectionCommand = ReactiveCommand.Create(explorerVM.RemoveSelection);
        //RevertSelectionCommand = ReactiveCommand.Create(explorerVM.RevertSelection);
    }
}
