using System.Runtime.InteropServices;
using static Valve.Sockets.CGameID.EGameIDType;

namespace Valve.Sockets
{
    public unsafe partial struct CGameID
    {
        public _Anonymous_e__Union Anonymous;

        public ref ulong m_ulGameID
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->m_ulGameID;
                }
            }
        }

        public ref GameID m_gameID
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->m_gameID;
                }
            }
        }

        public CGameID()
        {
            Anonymous.m_gameID.m_nType = (uint)(k_EGameIDTypeApp);
            Anonymous.m_gameID.m_nAppID = Constants.k_uAppIdInvalid;
            Anonymous.m_gameID.m_nModID = 0;
        }

        public CGameID(ulong ulGameID)
        {
            Anonymous.m_ulGameID = ulGameID;
        }

        public CGameID(int nAppID)
        {
            Anonymous.m_ulGameID = 0;
            Anonymous.m_gameID.m_nAppID = (uint)nAppID;
        }

        public CGameID(uint nAppID)
        {
            Anonymous.m_ulGameID = 0;
            Anonymous.m_gameID.m_nAppID = nAppID;
        }

        public CGameID(uint nAppID, uint nModID)
        {
            Anonymous.m_ulGameID = 0;
            Anonymous.m_gameID.m_nAppID = nAppID;
            Anonymous.m_gameID.m_nModID = nModID;
            Anonymous.m_gameID.m_nType = (uint)(k_EGameIDTypeGameMod);
        }

        public CGameID(CGameID* that)
        {
            Anonymous.m_ulGameID = that->Anonymous.m_ulGameID;
        }

        public ulong ToUint64()
        {
            return Anonymous.m_ulGameID;
        }

        public void Set(ulong ulGameID)
        {
            Anonymous.m_ulGameID = ulGameID;
        }

        public bool IsMod()
        {
            return (Anonymous.m_gameID.m_nType == (uint)k_EGameIDTypeGameMod);
        }

        public bool IsShortcut()
        {
            return (Anonymous.m_gameID.m_nType == (uint)k_EGameIDTypeShortcut);
        }

        public bool IsP2PFile()
        {
            return (Anonymous.m_gameID.m_nType == (uint)k_EGameIDTypeP2P);
        }

        public bool IsSteamApp()
        {
            return (Anonymous.m_gameID.m_nType == (uint)k_EGameIDTypeApp);
        }
        public uint ModID()
        {
            return Anonymous.m_gameID.m_nModID;
        }
        public uint AppID()
        {
            return Anonymous.m_gameID.m_nAppID;
        }

        public bool Equals(CGameID rhs)
        {
            return Anonymous.m_ulGameID == rhs.Anonymous.m_ulGameID;
        }

        public bool LessThan(CGameID rhs)
        {
            return (Anonymous.m_ulGameID < rhs.Anonymous.m_ulGameID);
        }

        public bool IsValid()
        {
            switch (Anonymous.m_gameID.m_nType)
            {
                case (int)(k_EGameIDTypeApp):
                {
                    return Anonymous.m_gameID.m_nAppID != Constants.k_uAppIdInvalid;
                }

                case (int)(k_EGameIDTypeGameMod):
                {
                    return Anonymous.m_gameID.m_nAppID != Constants.k_uAppIdInvalid && (Anonymous.m_gameID.m_nModID & 0x80000000) != 0;
                }

                case (int)(k_EGameIDTypeShortcut):
                {
                    return (Anonymous.m_gameID.m_nModID & 0x80000000) != 0;
                }

                case (int)(k_EGameIDTypeP2P):
                {
                    return Anonymous.m_gameID.m_nAppID == Constants.k_uAppIdInvalid && (Anonymous.m_gameID.m_nModID & 0x80000000) != 0;
                }

                default:
                {
                    return false;
                }
            }
        }

        public void Reset()
        {
            Anonymous.m_ulGameID = 0;
        }

        public enum EGameIDType : uint
        {
            k_EGameIDTypeApp = 0,
            k_EGameIDTypeGameMod = 1,
            k_EGameIDTypeShortcut = 2,
            k_EGameIDTypeP2P = 3,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public partial struct GameID
        {
            public uint _bitfield1;
            public uint m_nAppID
            {
                get
                {
                    return _bitfield1 & 0xFFFFFFu;
                }

                set
                {
                    _bitfield1 = (_bitfield1 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
                }
            }
            public uint m_nType
            {
                get
                {
                    return (_bitfield1 >> 24) & 0xFFu;
                }

                set
                {
                    _bitfield1 = (_bitfield1 & ~(0xFFu << 24)) | ((value & 0xFFu) << 24);
                }
            }

            public uint _bitfield2;
            public uint m_nModID
            {
                get
                {
                    return _bitfield2 & 0x0u;
                }

                set
                {
                    _bitfield2 = (_bitfield2 & ~0x0u) | (value & 0x0u);
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public ulong m_ulGameID;

            [FieldOffset(0)]
            public GameID m_gameID;
        }
    }
}