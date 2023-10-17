﻿using Ofnifile.Misc;
using Ofnifile.Misc.MessageBus;
using ReactiveUI;
using System.Reactive;

namespace Ofnifile.ViewModels.TabFeed;

public class IndexFeedVM : ReactiveObject
{
    private readonly Interfaces.MessageBus.IMessageBus _messageBus;

    public ReactiveCommand<Unit, Unit> CopySelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteSavedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> CutSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> CopySelectedItemPathCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> RenameSelectedItemCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateNewFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowFolderPropertiesCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectAllItemsCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectionCommand { get; }
    public ReactiveCommand<Unit, Unit> RevertSelectionCommand { get; }


    public IndexFeedVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        _messageBus = messageBus;

        CopySelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new CopySelectedItems(ExplorerType.Explorer)));
        PasteSavedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new PasteSavedItems(ExplorerType.Explorer)));
        CutSelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new CutSelectedItems(ExplorerType.Explorer)));
        CopySelectedItemPathCommand = ReactiveCommand.CreateFromTask(
            async x => await _messageBus.Send(new CopySelectedItemPath(ExplorerType.Explorer)));
        DeleteSelectedItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new DeleteSelectedItems(ExplorerType.Explorer)));
        RenameSelectedItemCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new RenameLastSelectedItem(ExplorerType.Explorer)));
        CreateNewFolderCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new CreateNewFolder(ExplorerType.Explorer)));
        ShowFolderPropertiesCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new ShowFolderProperties(ExplorerType.Explorer)));
        SelectAllItemsCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new SelectAllItems(ExplorerType.Explorer)));
        RemoveSelectionCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new RemoveSelection(ExplorerType.Explorer)));
        RevertSelectionCommand = ReactiveCommand.CreateFromTask(
            x => _messageBus.Send(new RevertSelection(ExplorerType.Explorer)));
    }
}
