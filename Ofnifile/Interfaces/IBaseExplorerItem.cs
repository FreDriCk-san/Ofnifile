using System;

namespace Ofnifile.Interfaces;

public interface IBaseExplorerItem : IDisposable
{
    bool Cut();
    bool Copy();
    bool Delete();
    bool Rename();
}
