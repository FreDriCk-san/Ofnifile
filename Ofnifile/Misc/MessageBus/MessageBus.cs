using Ofnifile.Interfaces.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ofnifile.Misc.MessageBus;

public class MessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<object>> _observers = new();
    private bool _isDisposed;

    public void Send<T>(T message) where T : IMessage
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var messageType = typeof(T);
        if (!_observers.ContainsKey(messageType))
            return;

        var subscriptions = _observers[messageType];
        if (subscriptions == null || subscriptions.Count == 0) 
            return;
        
        foreach (var handler in subscriptions
            .Select(s => s as ISubscription<T>)
            .Select(s => s!.Handler))
        {
            handler?.Invoke(message);
        }
    }

    public ISubscription<T>? Subscribe<T>(Action<T> callBack) where T : IMessage
    {
        ISubscription<T>? subscription = null;

        var messageType = typeof(T);
        var subscriptions = _observers.ContainsKey(messageType) 
            ? _observers[messageType] 
            : new List<object>();

        var existingSubscription = subscriptions
                                   .Select(s => s as ISubscription<T>)
                                   .FirstOrDefault(s => s!.Handler == callBack);

        if (existingSubscription is null)
        {
            subscription = new Subscription<T>(callBack);
            subscriptions.Add(subscription);
        }
        else
            return existingSubscription;

        _observers[messageType] = subscriptions;

        return subscription;
    }

    public bool Unsubscribe<T>(ISubscription<T> subscription) where T : IMessage
    {
        if (subscription is null)
            return false;

        var removed = false;
        var messageType = typeof(T);
        if (_observers.ContainsKey(messageType))
        {
            removed = _observers[messageType].Remove(subscription);

            if (_observers[messageType].Count == 0)
                _observers.Remove(messageType);
        }

        return removed;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        _isDisposed = true;

        foreach (var pair in _observers)
            pair.Value.Clear();
        _observers.Clear();
    }
}
