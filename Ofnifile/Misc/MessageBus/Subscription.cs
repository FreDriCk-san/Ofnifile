using Ofnifile.Interfaces.MessageBus;
using System;
using System.Threading.Tasks;

namespace Ofnifile.Misc.MessageBus;

public class Subscription<T> : ISubscription<T> where T : IMessage
{
    public Func<T, Task> Handler { get; init; }

    public Subscription(Func<T, Task> func)
    {
        Handler = func;
    }
}