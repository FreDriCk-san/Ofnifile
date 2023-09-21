using Ofnifile.Interfaces;
using System.IO;

namespace Ofnifile.Models;

public class FileItemModel : IFileItem
{
    public FileInfo Info => throw new System.NotImplementedException();

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

    public bool ShowProperties()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
