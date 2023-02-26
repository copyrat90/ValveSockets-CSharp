using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe partial struct SteamNetConnectionRealTimeLaneStatus
    {
        public int m_cbPendingUnreliable;

        public int m_cbPendingReliable;

        public int m_cbSentUnackedReliable;

        public int _reservePad1;
        public long m_usecQueueTime;
        public fixed uint reserved[10];
    }
}