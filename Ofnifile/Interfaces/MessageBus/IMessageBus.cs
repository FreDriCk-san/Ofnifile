using System;

namespace Ofnifile.Interfaces.MessageBus;

public interface IMessageBus : IDisposable
{
    void Send<T>(T message) where T : IMessage;
    ISubscription<T>? Subscribe<T>(Action<T> callBack) where T : IMessage;
    bool Unsubscribe<T>(ISubscription<T> subscription) where T : IMessage;
}
