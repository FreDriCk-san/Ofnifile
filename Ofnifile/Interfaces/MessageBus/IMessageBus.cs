using System;
using System.Threading.Tasks;

namespace Ofnifile.Interfaces.MessageBus;

public interface IMessageBus : IDisposable
{
    Task Send<T>(T message) where T : IMessage;
    IDisposable Subscribe<T>(Func<T, Task> callBack) where T : IMessage;
}
