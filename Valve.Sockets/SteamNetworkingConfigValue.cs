using System;
using System.Runtime.InteropServices;
using static Valve.Sockets.ESteamNetworkingConfigDataType;

namespace Valve.Sockets
{
    public unsafe partial struct SteamNetworkingConfigValue
    {
        public ESteamNetworkingConfigValue m_eValue;

        public ESteamNetworkingConfigDataType m_eDataType;
        public _m_val_e__Union m_val;

        public void SetInt32(ESteamNetworkingConfigValue eVal,int data)
        {
            m_eValue = eVal;
            m_eDataType = k_ESteamNetworkingConfig_Int32;
            m_val.m_int32 = data;
        }

        public void SetInt64(ESteamNetworkingConfigValue eVal,IntPtr data)
        {
            m_eValue = eVal;
            m_eDataType = k_ESteamNetworkingConfig_Int64;
            m_val.m_int64 = data;
        }

        public void SetFloat(ESteamNetworkingConfigValue eVal, float data)
        {
            m_eValue = eVal;
            m_eDataType = k_ESteamNetworkingConfig_Float;
            m_val.m_float = data;
        }

        public void SetPtr(ESteamNetworkingConfigValue eVal, void* data)
        {
            m_eValue = eVal;
            m_eDataType = k_ESteamNetworkingConfig_Ptr;
            m_val.m_ptr = data;
        }

        public void SetString(ESteamNetworkingConfigValue eVal,in string data)
        {
            m_eValue = eVal;
            m_eDataType = k_ESteamNetworkingConfig_Ptr;
            m_val.m_string = Marshal.StringToHGlobalAuto(data);
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _m_val_e__Union
        {
            [FieldOffset(0)]
            public int m_int32;

            [FieldOffset(0)]
            public IntPtr m_int64;

            [FieldOffset(0)]
            public float m_float;

            [FieldOffset(0)]
            public IntPtr m_string;

            [FieldOffset(0)]
            public void* m_ptr;
        }
    }
}