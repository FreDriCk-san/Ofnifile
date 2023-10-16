using System.Threading.Tasks;

namespace Ofnifile.Interfaces;

public interface IExplorerVM
{
    bool CopySelectedItems();
    bool PasteSavedItems();
    bool CutSelectedItems();
    Task<bool> CopySelectedItemPath();
    bool DeleteSelectedItems();
    bool RenameSelectedItem();
    bool CreateNewFolder();
    bool ShowFolderProperties();
    bool SelectAllItems();
    bool RemoveSelection();
    bool RevertSelection();
}
