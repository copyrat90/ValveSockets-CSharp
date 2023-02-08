using System;

namespace Valve.Sockets
{
    public partial struct SteamNetworkingMessage_t
    {
        public IntPtr m_pData;

        public int m_cbSize;
        public uint m_conn;

        public SteamNetworkingIdentity m_identityPeer;
        public long m_nConnUserData;
        public long m_usecTimeReceived;
        public long m_nMessageNumber;
        public IntPtr m_pfnFreeData;
        public IntPtr m_pfnRelease;

        public int m_nChannel;

        public int m_nFlags;
        public long m_nUserData;
        public ushort m_idxLane;
        public ushort _pad1__;

        public void Dispose()
        {
            Native.SteamAPI_SteamNetworkingMessage_t_Release(ref this);
        }

        public uint GetSize()
        {
            return (uint)m_cbSize;
        }
        public IntPtr GetData()
        {
            return m_pData;
        }

        public int GetChannel()
        {
            return m_nChannel;
        }
        public uint GetConnection()
        {
            return m_conn;
        }
        public long GetConnectionUserData()
        {
            return m_nConnUserData;
        }
        public long GetTimeReceived()
        {
            return m_usecTimeReceived;
        }
        public long GetMessageNumber()
        {
            return m_nMessageNumber;
        }
    }
}