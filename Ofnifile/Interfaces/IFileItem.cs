using System.IO;

namespace Ofnifile.Interfaces;

public interface IFileItem : IBaseExplorerItem
{
    FileInfo Info { get; }
}
