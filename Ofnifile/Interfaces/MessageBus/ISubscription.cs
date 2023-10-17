using System;

namespace Ofnifile.Interfaces.MessageBus;

public interface ISubscription<T> where T : IMessage
{
    Action<T> Handler { get; }
}
