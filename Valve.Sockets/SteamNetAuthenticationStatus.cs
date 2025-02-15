using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = Native.PackSize)]
    public unsafe partial struct SteamNetAuthenticationStatus
    {
        public ESteamNetworkingAvailability m_eAvail;
        public fixed sbyte m_debugMsg[256];

        public const uint k_iCallback = Constants.k_iSteamNetworkingSocketsCallbacks + 2;
    }
}