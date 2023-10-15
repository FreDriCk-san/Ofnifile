namespace Ofnifile.ViewModels.TabFeed;

public class TabFeedVM
{
    public IndexFeedVM IndexFeedVM { get; init; }
    public VisionFeedVM VisionFeedVM { get; init; }

    public TabFeedVM()
    {
        IndexFeedVM = new IndexFeedVM();
        VisionFeedVM = new VisionFeedVM();
    }
}
