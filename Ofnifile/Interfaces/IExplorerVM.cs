namespace Ofnifile.Interfaces;

public interface IExplorerVM
{
    bool CopySelectedItems();
    bool PasteSavedItems();
    bool CutSelectedItems();
    bool CopySelectedItemPath();
    bool DeleteSelectedItems();
    bool RenameSelectedItem();
    bool CreateNewFolder();
    bool ShowFolderProperties();
    bool SelectAllItems();
    bool RemoveSelection();
    bool RevertSelection();
}
