using System;

namespace Valve.Sockets
{
    public sealed class GameNetworkingSockets : IDisposable
    {
        private readonly SteamNetworkingErrMsg _errMsg;

        private bool _disposed;

        public GameNetworkingSockets()
        {
            if (!Native.GameNetworkingSockets_Init(IntPtr.Zero, ref _errMsg))
            {
                throw new SystemException($"Failed to initialize GameNetworkingSockets: {_errMsg.Value}");
            }
        }

        public GameNetworkingSockets(SteamNetworkingIdentity identity)
        {
            if (!Native.GameNetworkingSockets_Init(identity, ref _errMsg))
            {
                throw new SystemException($"Failed to initialize GameNetworkingSockets: {_errMsg.Value}");
            }
        }

        ~GameNetworkingSockets() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // No managed resource here
                }

                // Unmanaged resource cleanup
                Native.GameNetworkingSockets_Kill();

                _disposed = true;
            }
        }
    }
}