using Ofnifile.Interfaces;
using System.IO;

namespace Ofnifile.Models;

public class FolderItemModel : IFolderItem
{
    public DirectoryInfo Info { get; init; }

    public FolderItemModel(string folderPath)
    {
        Info = new DirectoryInfo(folderPath);
    }

    public bool Copy()
    {
        throw new System.NotImplementedException();
    }

    public bool Cut()
    {
        throw new System.NotImplementedException();
    }

    public bool Delete()
    {
        throw new System.NotImplementedException();
    }

    public bool Rename()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
