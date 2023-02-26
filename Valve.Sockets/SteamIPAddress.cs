using System.Runtime.InteropServices;
using static Valve.Sockets.ESteamIPType;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe partial struct SteamIPAddress
    {
        public _Anonymous_e__Union Anonymous;

        public ESteamIPType m_eType;

        public ref uint m_unIPv4
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->m_unIPv4;
                }
            }
        }

        public ref byte m_rgubIPv6
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->m_rgubIPv6[0];
                }
            }
        }

        public ref ulong m_ipv6Qword
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->m_ipv6Qword[0];
                }
            }
        }

        public bool IsSet()
        {
            if (k_ESteamIPTypeIPv4 == m_eType)
            {
                return Anonymous.m_unIPv4 != 0;
            }
            else
            {
                return Anonymous.m_ipv6Qword[0] != 0 || Anonymous.m_ipv6Qword[1] != 0;
            }
        }

        public static SteamIPAddress IPv4Any()
        {
            SteamIPAddress ipOut = new SteamIPAddress();

            ipOut.m_eType = k_ESteamIPTypeIPv4;
            ipOut.Anonymous.m_unIPv4 = 0;
            return ipOut;
        }

        public static SteamIPAddress IPv6Any()
        {
            SteamIPAddress ipOut = new SteamIPAddress();

            ipOut.m_eType = k_ESteamIPTypeIPv6;
            ipOut.Anonymous.m_ipv6Qword[0] = 0;
            ipOut.Anonymous.m_ipv6Qword[1] = 0;
            return ipOut;
        }

        public static SteamIPAddress IPv4Loopback()
        {
            SteamIPAddress ipOut = new SteamIPAddress();

            ipOut.m_eType = k_ESteamIPTypeIPv4;
            ipOut.Anonymous.m_unIPv4 = 0x7f000001;
            return ipOut;
        }

        public static SteamIPAddress IPv6Loopback()
        {
            SteamIPAddress ipOut = new SteamIPAddress();

            ipOut.m_eType = k_ESteamIPTypeIPv6;
            ipOut.Anonymous.m_ipv6Qword[0] = 0;
            ipOut.Anonymous.m_ipv6Qword[1] = 0;
            ipOut.Anonymous.m_rgubIPv6[15] = 1;
            return ipOut;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public uint m_unIPv4;

            [FieldOffset(0)]
            public fixed byte m_rgubIPv6[16];

            [FieldOffset(0)]
            public fixed ulong m_ipv6Qword[2];
        }
    }
}