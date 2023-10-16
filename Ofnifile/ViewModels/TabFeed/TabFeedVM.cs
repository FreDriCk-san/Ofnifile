using Ofnifile.Interfaces;

namespace Ofnifile.ViewModels.TabFeed;

public class TabFeedVM
{
    public IndexFeedVM IndexFeedVM { get; init; }
    public VisionFeedVM VisionFeedVM { get; init; }

    public TabFeedVM(IExplorerVM explorerVM)
    {
        IndexFeedVM = new IndexFeedVM(explorerVM);
        VisionFeedVM = new VisionFeedVM();
    }
}
