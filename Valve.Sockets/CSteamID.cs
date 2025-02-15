using System.Runtime.InteropServices;
using static Valve.Sockets.EAccountType;
using static Valve.Sockets.EChatSteamIDInstanceFlags;
using static Valve.Sockets.EUniverse;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public partial struct CSteamID
    {
        private SteamID m_steamid;

        public CSteamID()
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)(k_EAccountTypeInvalid);
            m_steamid.m_comp.m_EUniverse = k_EUniverseInvalid;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        public CSteamID(uint unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            Set(unAccountID, eUniverse, eAccountType);
        }

        public CSteamID(uint unAccountID, uint unAccountInstance, EUniverse eUniverse, EAccountType eAccountType)
        {
            InstancedSet(unAccountID, unAccountInstance, eUniverse, eAccountType);
        }

        public CSteamID(ulong ulSteamID)
        {
            SetFromUint64(ulSteamID);
        }

        public void Set(uint unAccountID,EUniverse eUniverse,EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = unAccountID;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
            if (eAccountType == k_EAccountTypeClan || eAccountType == k_EAccountTypeGameServer)
            {
                m_steamid.m_comp.m_unAccountInstance = 0;
            }
            else
            {
                m_steamid.m_comp.m_unAccountInstance = Constants.k_unSteamUserDefaultInstance;
            }
        }

        public void InstancedSet(uint unAccountID,uint unInstance,EUniverse eUniverse,EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = unAccountID;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
            m_steamid.m_comp.m_unAccountInstance = unInstance;
        }

        public void FullSet(ulong ulIdentifier,EUniverse eUniverse,EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = (uint)(ulIdentifier & Constants.k_unSteamAccountIDMask);
            m_steamid.m_comp.m_unAccountInstance = (uint)((ulIdentifier >> 32) & Constants.k_unSteamAccountInstanceMask);
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
        }

        public void SetFromUint64(ulong ulSteamID)
        {
            m_steamid.m_unAll64Bits = ulSteamID;
        }

        public void Clear()
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)(k_EAccountTypeInvalid);
            m_steamid.m_comp.m_EUniverse = k_EUniverseInvalid;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }
        public ulong ConvertToUint64()
        {
            return m_steamid.m_unAll64Bits;
        }
        public ulong GetStaticAccountKey()
        {
            return (ulong)((((ulong)(m_steamid.m_comp.m_EUniverse)) << 56) + ((ulong)(m_steamid.m_comp.m_EAccountType) << 52) + m_steamid.m_comp.m_unAccountID);
        }

        public void CreateBlankAnonLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)(k_EAccountTypeAnonGameServer);
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        public void CreateBlankAnonUserLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)(k_EAccountTypeAnonUser);
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        public bool BBlankAnonAccount()
        {
            return m_steamid.m_comp.m_unAccountID == 0 && BAnonAccount() && m_steamid.m_comp.m_unAccountInstance == 0;
        }

        public bool BGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeGameServer || m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeAnonGameServer;
        }

        public bool BPersistentGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeGameServer;
        }

        public bool BAnonGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeAnonGameServer;
        }

        public bool BContentServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeContentServer;
        }

        public bool BClanAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeClan;
        }

        public bool BChatAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeChat;
        }

        public bool IsLobby()
        {
            return (m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeChat) && (m_steamid.m_comp.m_unAccountInstance & (int)(k_EChatInstanceFlagLobby)) != 0;
        }

        public bool BIndividualAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeIndividual || m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeConsoleUser;
        }

        public bool BAnonAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeAnonUser || m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeAnonGameServer;
        }

        public bool BAnonUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeAnonUser;
        }

        public bool BConsoleUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeConsoleUser;
        }

        public void SetAccountID(uint unAccountID)
        {
            m_steamid.m_comp.m_unAccountID = unAccountID;
        }

        public void SetAccountInstance(uint unInstance)
        {
            m_steamid.m_comp.m_unAccountInstance = unInstance;
        }
        public uint GetAccountID()
        {
            return m_steamid.m_comp.m_unAccountID;
        }
        public uint GetUnAccountInstance()
        {
            return m_steamid.m_comp.m_unAccountInstance;
        }

        public EAccountType GetEAccountType()
        {
            return (EAccountType)(m_steamid.m_comp.m_EAccountType);
        }

        public EUniverse GetEUniverse()
        {
            return m_steamid.m_comp.m_EUniverse;
        }

        public void SetEUniverse(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_EUniverse = eUniverse;
        }

        public bool IsValid()
        {
            if (m_steamid.m_comp.m_EAccountType <= (int)(k_EAccountTypeInvalid) || m_steamid.m_comp.m_EAccountType >= (int)(k_EAccountTypeMax))
            {
                return false;
            }

            if (m_steamid.m_comp.m_EUniverse <= (int)(k_EUniverseInvalid) || m_steamid.m_comp.m_EUniverse >= (k_EUniverseMax))
            {
                return false;
            }

            if (m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeIndividual)
            {
                if (m_steamid.m_comp.m_unAccountID == 0 || m_steamid.m_comp.m_unAccountInstance != Constants.k_unSteamUserDefaultInstance)
                {
                    return false;
                }
            }

            if (m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeClan)
            {
                if (m_steamid.m_comp.m_unAccountID == 0 || m_steamid.m_comp.m_unAccountInstance != 0)
                {
                    return false;
                }
            }

            if (m_steamid.m_comp.m_EAccountType == (uint)k_EAccountTypeGameServer)
            {
                if (m_steamid.m_comp.m_unAccountID == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(CSteamID val)
        {
            return m_steamid.m_unAll64Bits == val.m_steamid.m_unAll64Bits;
        }

        public bool NotEquals(CSteamID val)
        {
            return !Equals(val);
        }

        public bool LessThan(CSteamID val)
        {
            return m_steamid.m_unAll64Bits < val.m_steamid.m_unAll64Bits;
        }

        public bool GreaterThan(CSteamID val)
        {
            return m_steamid.m_unAll64Bits > val.m_steamid.m_unAll64Bits;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private partial struct SteamID
        {
            [FieldOffset(0)]
            public SteamIDComponent m_comp;

            [FieldOffset(0)]
            public ulong m_unAll64Bits;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public partial struct SteamIDComponent
            {
                public uint _bitfield1;
                public uint m_unAccountID
                {
                    get
                    {
                        return _bitfield1 & 0x0u;
                    }

                    set
                    {
                        _bitfield1 = (_bitfield1 & ~0x0u) | (value & 0x0u);
                    }
                }

                public uint _bitfield2;
                public uint m_unAccountInstance
                {
                    get
                    {
                        return _bitfield2 & 0xFFFFFu;
                    }

                    set
                    {
                        _bitfield2 = (_bitfield2 & ~0xFFFFFu) | (value & 0xFFFFFu);
                    }
                }
                public uint m_EAccountType
                {
                    get
                    {
                        return (_bitfield2 >> 20) & 0xFu;
                    }

                    set
                    {
                        _bitfield2 = (_bitfield2 & ~(0xFu << 20)) | ((value & 0xFu) << 20);
                    }
                }
                public EUniverse m_EUniverse
                {
                    get
                    {
                        return (EUniverse)((_bitfield2 >> 24) & 0xFFu);
                    }

                    set
                    {
                        _bitfield2 = (_bitfield2 & ~(0xFFu << 24)) | (((uint)(value) & 0xFFu) << 24);
                    }
                }
            }
        }
    }
}