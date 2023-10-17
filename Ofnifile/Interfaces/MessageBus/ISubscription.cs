using System;
using System.Threading.Tasks;

namespace Ofnifile.Interfaces.MessageBus;

public interface ISubscription<T> where T : IMessage
{
    Func<T, Task> Handler { get; }
}
