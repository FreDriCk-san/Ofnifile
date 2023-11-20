using Ofnifile.Interfaces;

namespace Ofnifile.ViewModels.TabFeed;

public class TabFeedVM
{
    public IndexFeedVM IndexFeedVM { get; init; }
    public VisionFeedVM VisionFeedVM { get; init; }

    public TabFeedVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        IndexFeedVM = new IndexFeedVM(messageBus);
        VisionFeedVM = new VisionFeedVM(messageBus);
    }
}
