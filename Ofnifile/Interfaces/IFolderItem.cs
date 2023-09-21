using System.IO;

namespace Ofnifile.Interfaces;

public interface IFolderItem : IBaseExplorerItem
{
    DirectoryInfo Info { get; init; }
}
