using System;
using System.Runtime.InteropServices;
using System.Text;
using Valve.Sockets.ArrayStructs;
using static Valve.Sockets.ESteamNetworkingFakeIPType;
using static Valve.Sockets.ESteamNetworkingIdentityType;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct SteamNetworkingIdentity
    {
        public ESteamNetworkingIdentityType m_eType;

        public int m_cbSize;

        public IdentityUnion Union;

        public ulong m_steamID64 => Union.m_steamID64;

        public string m_szGenericString => Encoding.Default.GetString(Union.m_szGenericString.AsSpan());

        public Span<byte> m_genericBytes => Union.m_genericBytes.AsSpan();

        public string m_szUnknownRawString => Encoding.Default.GetString(Union.m_szUnknownRawString.AsSpan());

        public SteamNetworkingIPAddr m_ip => Union.m_ip;

        public Span<uint> m_reserved => Union.m_reserved.AsSpan();

        public void Clear()
        {
            Union = new IdentityUnion();
        }

        public bool IsInvalid()
        {
            return m_eType == k_ESteamNetworkingIdentityType_Invalid;
        }

        public void SetSteamID(CSteamID steamID)
        {
            SetSteamID64(steamID.ConvertToUint64());
        }

        public CSteamID GetSteamID()
        {
            return new CSteamID(GetSteamID64());
        }

        public void SetSteamID64(ulong steamID)
        {
            m_eType = k_ESteamNetworkingIdentityType_SteamID;
            m_cbSize = sizeof(ulong);
            Union.m_steamID64 = steamID;
        }
        public ulong GetSteamID64()
        {
            return m_eType == k_ESteamNetworkingIdentityType_SteamID ? Union.m_steamID64 : 0;
        }

        public void SetIPAddr(SteamNetworkingIPAddr addr)
        {
            m_eType = k_ESteamNetworkingIdentityType_IPAddress;
            m_cbSize = (int)(Marshal.SizeOf<SteamNetworkingIPAddr>());
        }
        public bool GetIPAddr(out SteamNetworkingIPAddr ip)
        {
            ip = default;

            if (m_eType == k_ESteamNetworkingIdentityType_IPAddress)
            {
                ip = Union.m_ip;

                return true;
            }

            return false;
        }

        public void SetIPv4Addr(uint nIPv4,ushort nPort)
        {
            m_eType = k_ESteamNetworkingIdentityType_IPAddress;
            m_cbSize = Marshal.SizeOf<SteamNetworkingIPAddr>();
            Union.m_ip.SetIPv4(nIPv4, nPort);
        }
        public uint GetIPv4()
        {
            return m_eType == k_ESteamNetworkingIdentityType_IPAddress ? Union.m_ip.GetIPv4() : 0;
        }

        public ESteamNetworkingFakeIPType GetFakeIPType()
        {
            return m_eType == k_ESteamNetworkingIdentityType_IPAddress ? Native.SteamNetworkingIPAddr_GetFakeIPType(Union.m_ip) : k_ESteamNetworkingFakeIPType_Invalid;
        }

        public bool IsFakeIP()
        {
            return GetFakeIPType() > (k_ESteamNetworkingFakeIPType_NotFake);
        }

        public void SetLocalHost()
        {
            m_eType = k_ESteamNetworkingIdentityType_IPAddress;
            m_cbSize = Marshal.SizeOf<SteamNetworkingIPAddr>();
            Union.m_ip.SetIPv6LocalHost();
        }

        public bool IsLocalHost()
        {
            return m_eType == k_ESteamNetworkingIdentityType_IPAddress && Union.m_ip.IsLocalHost();
        }

        public bool SetGenericString(in string pszString)
        {
            byte[] data = Encoding.Default.GetBytes(pszString);

            if (data.Length >= 32)
            {
                return false;
            }

            m_eType = k_ESteamNetworkingIdentityType_GenericString;
            m_cbSize = data.Length;
            data.AsSpan().CopyTo(Union.m_szGenericString.AsSpan());
            return true;
        }
        public string GetGenericString()
        {
            return m_eType == k_ESteamNetworkingIdentityType_GenericString ? Encoding.Default.GetString(Union.m_szGenericString.AsSpan()) : null;
        }

        public bool SetGenericBytes(in byte[] data)
        {
            if (data.Length > 32)
            {
                return false;
            }

            m_eType = k_ESteamNetworkingIdentityType_GenericBytes;
            m_cbSize = data.Length;
            data.AsSpan().CopyTo(Union.m_genericBytes.AsSpan());
            return true;
        }
        public byte[] GetGenericBytes(out int cbLen)
        {
            cbLen = -1;

            if (m_eType != k_ESteamNetworkingIdentityType_GenericBytes)
            {
                return null;
            }

            cbLen = m_cbSize;
            return Union.m_genericBytes.AsSpan().ToArray();
        }

        public const uint k_cchMaxString = 128;
        public const uint k_cchMaxGenericString = 32;
        public const uint k_cbMaxGenericBytes = 32;

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct IdentityUnion
        {
            [FieldOffset(0)]
            public ulong m_steamID64;

            [FieldOffset(0)]
            public Array32<byte> m_szGenericString;

            [FieldOffset(0)]
            public Array32<byte> m_genericBytes;

            [FieldOffset(0)]
            public Array128<byte> m_szUnknownRawString;

            [FieldOffset(0)]
            public SteamNetworkingIPAddr m_ip;

            [FieldOffset(0)]
            public Array32<uint> m_reserved;
        }
    }
}