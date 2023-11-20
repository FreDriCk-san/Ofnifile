using Ofnifile.Interfaces.MessageBus;
using System;
using System.Threading.Tasks;

namespace Ofnifile.Misc.MessageBus;

public sealed record Subscription<T>(Func<T, Task> Handler, IDisposable Disposable) : ISubscription<T> where T : IMessage;
