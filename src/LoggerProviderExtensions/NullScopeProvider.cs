#if NET6_0_OR_GREATER
using Microsoft.Extensions.Logging;

namespace LoggerProviderExtensions;

internal class NullScopeProvider : IExternalScopeProvider
{
    private NullScopeProvider()
    {
    }

    /// <summary>
    /// Returns a cached instance of <see cref="NullScopeProvider"/>.
    /// </summary>
    public static IExternalScopeProvider Instance { get; } = new NullScopeProvider();

    /// <inheritdoc />
    void IExternalScopeProvider.ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
    }

    /// <inheritdoc />
    IDisposable IExternalScopeProvider.Push(object? state)
    {
        return NullScope.Instance;
    }

    internal sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
#endif