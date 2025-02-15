using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    /// <summary>Posted when a remote host is sending us a message, and we do not already have a session with them</summary>
    [StructLayout(LayoutKind.Sequential, Pack = Native.PackSize)]
    public partial struct SteamNetworkingMessagesSessionRequest
    {
        public SteamNetworkingIdentity m_identityRemote;

        public const uint k_iCallback = Constants.k_iSteamNetworkingMessagesCallbacks + 1;
    }
}