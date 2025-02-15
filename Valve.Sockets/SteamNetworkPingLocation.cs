using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = Native.PackSize)]
    public unsafe partial struct SteamNetworkPingLocation
    {
        public fixed byte m_data[512];
    }
}