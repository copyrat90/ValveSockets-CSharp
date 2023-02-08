using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe partial struct SteamNetConnectionInfo_t
    {
        public SteamNetworkingIdentity m_identityRemote;
        public long m_nUserData;
        public uint m_hListenSocket;

        public SteamNetworkingIPAddr m_addrRemote;
        public ushort m__pad1;
        public uint m_idPOPRemote;
        public uint m_idPOPRelay;

        public ESteamNetworkingConnectionState m_eState;

        public int m_eEndReason;
        public fixed sbyte m_szEndDebug[128];
        public fixed sbyte m_szConnectionDescription[128];

        public int m_nFlags;
        public fixed uint reserved[63];
    }
}
