using System;

namespace Valve.Sockets
{
    public class GameNetworkingSockets
    {
        private readonly SteamNetworkingErrMsg _errMsg;

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

        ~GameNetworkingSockets()
        {
            Native.GameNetworkingSockets_Kill();
        }
    }
}