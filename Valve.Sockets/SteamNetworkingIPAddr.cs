using System;
using System.Runtime.InteropServices;
using Valve.Sockets.ArrayStructs;
using static Valve.Sockets.ESteamNetworkingFakeIPType;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct SteamNetworkingIPAddr
    {
        public IpUnion Union;
        public ushort m_port;

        public Span<byte> m_ipv6 => Union.m_ipv6.AsSpan();

        public IPv4MappedAddress m_ipv4 => Union.m_ipv4;

        public void Clear()
        {
            Union = new IpUnion();
        }

        public bool IsIPv6AllZeros()
        {
            foreach (byte b in Union.m_ipv6.AsSpan())
            {
                if (b != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetIPv6(byte[] ipv6,ushort nPort)
        {
            ipv6.AsSpan().CopyTo(Union.m_ipv6.AsSpan());
            m_port = nPort;
        }

        public void SetIPv4(uint nIP,ushort nPort)
        {
            Union.m_ipv4.m_8zeros = 0;
            Union.m_ipv4.m_0000 = 0;
            Union.m_ipv4.m_ffff = 0xffff;
            Union.m_ipv4.m_ip[0] = (byte)(nIP >> 24);
            Union.m_ipv4.m_ip[1] = (byte)(nIP >> 16);
            Union.m_ipv4.m_ip[2] = (byte)(nIP >> 8);
            Union.m_ipv4.m_ip[3] = (byte)(nIP);
            m_port = nPort;
        }

        public bool IsIPv4()
        {
            return Union.m_ipv4.m_8zeros == 0 && Union.m_ipv4.m_0000 == 0 && Union.m_ipv4.m_ffff == 0xffff;
        }
        public uint GetIPv4()
        {
            return IsIPv4() ? (((uint)(Union.m_ipv4.m_ip[0]) << 24) | ((uint)(Union.m_ipv4.m_ip[1]) << 16) | ((uint)(Union.m_ipv4.m_ip[2]) << 8) | (uint)(Union.m_ipv4.m_ip[3])) : 0;
        }

        public void SetIPv6LocalHost(ushort nPort = 0)
        {
            Union.m_ipv4.m_8zeros = 0;
            Union.m_ipv4.m_0000 = 0;
            Union.m_ipv4.m_ffff = 0;
            Union.m_ipv6[12] = 0;
            Union.m_ipv6[13] = 0;
            Union.m_ipv6[14] = 0;
            Union.m_ipv6[15] = 1;
            m_port = nPort;
        }

        public bool IsLocalHost()
        {
            return (Union.m_ipv4.m_8zeros == 0 && Union.m_ipv4.m_0000 == 0 && Union.m_ipv4.m_ffff == 0 && Union.m_ipv6[12] == 0 && Union.m_ipv6[13] == 0 && Union.m_ipv6[14] == 0 && Union.m_ipv6[15] == 1) || (GetIPv4() == 0x7f000001);
        }

        public bool IsFakeIP()
        {
            return Native.SteamNetworkingIPAddr_GetFakeIPType(this) > (k_ESteamNetworkingFakeIPType_NotFake);
        }

        public const uint k_cchMaxString = 48;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public partial struct IPv4MappedAddress
        {
            public ulong m_8zeros;
            public ushort m_0000;
            public ushort m_ffff;
            public Array4<byte> m_ip;
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct IpUnion
        {
            [FieldOffset(0)]
            public Array16<byte> m_ipv6;

            [FieldOffset(0)]
            public IPv4MappedAddress m_ipv4;
        }
    }
}