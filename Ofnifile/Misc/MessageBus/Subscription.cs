using Ofnifile.Interfaces.MessageBus;
using System;

namespace Ofnifile.Misc.MessageBus;

public class Subscription<T> : ISubscription<T> where T : IMessage
{
    public Action<T> Handler { get; init; }

    public Subscription(Action<T> action)
    {
        Handler = action;
    }
}