using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    public class SteamNetworkingUtils : ISteamNetworkingUtils
    {
        private readonly IntPtr _instance;

        public SteamNetworkingUtils()
        {
            _instance = Native.SteamNetworkingUtils();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public SteamNetworkingMessage AllocateMessage(int cbAllocateBuffer)
        {
            return Marshal.PtrToStructure<SteamNetworkingMessage>(Native.SteamAPI_ISteamNetworkingUtils_AllocateMessage(_instance, cbAllocateBuffer));
        }

        public void InitRelayNetworkAccess()
        {
            Native.SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess(_instance);
        }

        public ESteamNetworkingAvailability GetRelayNetworkStatus(SteamRelayNetworkStatus pDetails)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus(_instance, ref pDetails);
        }

        public float GetLocalPingLocation(SteamNetworkPingLocation result)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation(_instance, result);
        }

        public int EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation location1, SteamNetworkPingLocation location2)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations(_instance, location1,
                location2);
        }

        public int EstimatePingTimeFromLocalHost(SteamNetworkPingLocation remoteLocation)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost(_instance, remoteLocation);
        }

        public void ConvertPingLocationToString(SteamNetworkPingLocation location, string pszBuf, int cchBufSize)
        {
            Native.SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString(_instance, location, pszBuf, cchBufSize);
        }

        public bool ParsePingLocationString(string pszString, SteamNetworkPingLocation result)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_ParsePingLocationString(_instance, pszString, result);
        }

        public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate(_instance, flMaxAgeSeconds);
        }

        public int GetPingToDataCenter(uint popID, ref SteamNetworkingPOPID pViaRelayPoP)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter(_instance, popID, ref pViaRelayPoP);
        }

        public int GetDirectPingToPOP(uint popID)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP(_instance, popID);
        }

        public int GetPOPCount()
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetPOPCount(_instance);
        }

        public int GetPOPList(SteamNetworkingPOPID[] list, int nListSz)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetPOPList(_instance, list, nListSz);
        }

        public long GetLocalTimestamp()
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(_instance);
        }

        public void SetDebugOutputFunction(ESteamNetworkingSocketsDebugOutputType eDetailLevel,
            FSteamNetworkingSocketsDebugOutput pfnFunc)
        {
            Native.SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(_instance, eDetailLevel, ref pfnFunc);
        }

        public bool IsFakeIPv4()
        {
            throw new NotImplementedException();
        }

        public ESteamNetworkingFakeIPType GetIPv4FakeIPType(uint nIPv4)
        {
            throw new NotImplementedException();
        }

        public EResult GetRealIdentityForFakeIP(SteamNetworkingIPAddr fakeIP, SteamNetworkingIdentity pOutRealIdentity)
        {
            throw new NotImplementedException();
        }

        public bool SetConfigValue(ESteamNetworkingConfigValue eValue, ESteamNetworkingConfigScope eScopeType, nint scopeObj,
            ESteamNetworkingConfigDataType eDataType, byte[] pArg)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_SetConfigValue(_instance, eValue, eScopeType, scopeObj,
                eDataType, pArg);
        }

        public bool SetConfigValueStruct()
        {
            throw new NotImplementedException();
        }

        public ESteamNetworkingGetConfigValueResult GetConfigValue(ESteamNetworkingConfigValue eValue,
            ESteamNetworkingConfigScope eScopeType, nint scopeObj, ref ESteamNetworkingConfigDataType pOutDataType,
            nint pResult, ref ulong cbResult)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetConfigValue(_instance, eValue, eScopeType, scopeObj,
                ref pOutDataType, pResult, ref cbResult);
        }

        public string GetConfigValueInfo(ESteamNetworkingConfigValue eValue, ref ESteamNetworkingConfigDataType pOutDataType,
            ref ESteamNetworkingConfigScope pOutScope)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(_instance, eValue, ref pOutDataType,
                ref pOutScope);
        }

        public ESteamNetworkingConfigValue IterateGenericEditableConfigValues(ESteamNetworkingConfigValue eCurrent,
            bool bEnumerateDevVars)
        {
            return Native.SteamAPI_ISteamNetworkingUtils_IterateGenericEditableConfigValues(_instance, eCurrent,
                bEnumerateDevVars);
        }

        public void SteamNetworkingIPAddrToString(SteamNetworkingIPAddr addr, string buf, ulong cbBuf, bool bWithPort)
        {
            throw new NotImplementedException();
        }

        public bool SteamNetworkingIPAddrParseString(SteamNetworkingIPAddr pAddr, string pszStr)
        {
            throw new NotImplementedException();
        }

        public ESteamNetworkingFakeIPType SteamNetworkingIPAddrGetFakeIPType(SteamNetworkingIPAddr addr)
        {
            throw new NotImplementedException();
        }

        public void SteamNetworkingIdentityToString(SteamNetworkingIdentity identity, string buf, ulong cbBuf)
        {
            throw new NotImplementedException();
        }

        public bool SteamNetworkingIdentityParseString(SteamNetworkingIdentity pIdentity, string pszStr)
        {
            throw new NotImplementedException();
        }

        public void SteamNetworkingIPAddrToString()
        {
            throw new NotImplementedException();
        }

        public bool SteamNetworkingIPAddrParseString()
        {
            throw new NotImplementedException();
        }

        public ESteamNetworkingFakeIPType SteamNetworkingIPAddrGetFakeIPType()
        {
            throw new NotImplementedException();
        }

        public void SteamNetworkingIdentityToString()
        {
            throw new NotImplementedException();
        }

        public bool SteamNetworkingIdentityParseString()
        {
            throw new NotImplementedException();
        }

        public int POPCount => Native.SteamAPI_ISteamNetworkingUtils_GetPOPCount(_instance);
        public long LocalTimestamp => Native.SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(_instance);
    }
}