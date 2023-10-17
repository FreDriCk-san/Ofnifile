namespace Ofnifile.ViewModels.TabFeed;

public class VisionFeedVM
{
    private readonly Interfaces.MessageBus.IMessageBus _messageBus;

    public VisionFeedVM(Interfaces.MessageBus.IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
}
