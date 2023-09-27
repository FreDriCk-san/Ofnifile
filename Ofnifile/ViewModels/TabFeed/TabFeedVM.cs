namespace Ofnifile.ViewModels.TabFeed;

public class TabFeedVM
{
    private IndexFeedVM _indexFeedVM;
    private VisionFeedVM _visionFeedVM;

    public TabFeedVM()
    {
        _indexFeedVM = new IndexFeedVM();
        _visionFeedVM = new VisionFeedVM();
    }
}
