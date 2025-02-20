using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Valve.Sockets
{
    public static partial class Native
    {
#if VALVE_CALLBACK_PACK_SMALL
        public const int PackSize = 4;
#elif VALVE_CALLBACK_PACK_LARGE
        public const int PackSize = 8;
#else
        #error "Unknown struct pack size. Define `VALVE_CALLBACK_PACK_SMALL` on Linux/Apple/FreeBSD, or define `VALVE_CALLBACK_PACK_LARGE` on other platform."
#endif

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr Pfn_malloc(ulong param1);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Pfn_free(IntPtr param1);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr Pfn_realloc(IntPtr param1,ulong param2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Callback([MarshalAs(UnmanagedType.LPUTF8Str)] string param1,SteamNetworkingMicroseconds param2);
    }

    public static partial class Native
    {
        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamNetworkingUtils_LibV4();

        public static IntPtr SteamNetworkingUtils_Lib()
        {
            return SteamNetworkingUtils_LibV4();
        }

        public static IntPtr SteamNetworkingUtils()
        {
            return SteamNetworkingUtils_LibV4();
        }

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingIPAddr_ToString(in SteamNetworkingIPAddr pAddr,[MarshalAs(UnmanagedType.LPUTF8Str)] string buf,UIntPtr cbBuf,[MarshalAs(UnmanagedType.I1)] bool bWithPort);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamNetworkingIPAddr_ParseString(ref SteamNetworkingIPAddr pAddr,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszStr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingFakeIPType SteamNetworkingIPAddr_GetFakeIPType(in SteamNetworkingIPAddr pAddr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingIdentity_ToString(in SteamNetworkingIdentity pIdentity,[MarshalAs(UnmanagedType.LPUTF8Str)] string buf,UIntPtr cbBuf);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamNetworkingIdentity_ParseString(ref SteamNetworkingIdentity pIdentity,UIntPtr sizeofIdentity,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszStr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamNetworkingSockets_LibV12();

        public static IntPtr SteamNetworkingSockets_Lib()
        {
            return SteamNetworkingSockets_LibV12();
        }

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamGameServerNetworkingSockets_LibV12();

        public static IntPtr SteamGameServerNetworkingSockets_Lib()
        {
            return SteamGameServerNetworkingSockets_LibV12();
        }

        public static IntPtr SteamNetworkingSockets()
        {
            return SteamNetworkingSockets_LibV12();
        }

        public static IntPtr SteamGameServerNetworkingSockets()
        {
            return SteamGameServerNetworkingSockets_LibV12();
        }

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamNetworkingMessages_LibV2();

        public static IntPtr SteamNetworkingMessages_Lib()
        {
            return SteamNetworkingMessages_LibV2();
        }

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamGameServerNetworkingMessages_LibV2();

        public static IntPtr SteamGameServerNetworkingMessages_Lib()
        {
            return SteamGameServerNetworkingMessages_LibV2();
        }

        public static IntPtr SteamNetworkingMessages()
        {
            return SteamNetworkingMessages_LibV2();
        }

        public static IntPtr SteamGameServerNetworkingMessages()
        {
            return SteamGameServerNetworkingMessages_LibV2();
        }
        public const uint k_HAuthTicketInvalid = 0;
        public const int k_cchGameExtraInfoMax = 64;

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool GameNetworkingSockets_Init(IntPtr pIdentity, ref SteamNetworkingErrMsg errMsg);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool GameNetworkingSockets_Init(in SteamNetworkingIdentity pIdentity,ref SteamNetworkingErrMsg errMsg);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void GameNetworkingSockets_Kill();

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingSockets_SetCustomMemoryAllocator(Pfn_malloc pfn_malloc,Pfn_free pfn_free,Pfn_realloc pfn_realloc);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingSockets_SetLockWaitWarningThreshold(long usecThreshold);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingSockets_SetLockAcquiredCallback(Callback callback);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamNetworkingSockets_SetLockHeldCallback(Callback callback);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_SteamNetworkingSockets_v009();

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr self,in SteamNetworkingIPAddr localAddress,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr self,in SteamNetworkingIPAddr address,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketP2P(IntPtr self,int nLocalVirtualPort,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_ConnectP2P(IntPtr self,in SteamNetworkingIdentity identityRemote,int nRemoteVirtualPort,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_AcceptConnection(IntPtr self,uint hConn);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_CloseConnection(IntPtr self,uint hPeer,int nReason,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszDebug,[MarshalAs(UnmanagedType.I1)] bool bEnableLinger);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(IntPtr self,uint hSocket);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(IntPtr self,uint hPeer,long nUserData);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial long SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(IntPtr self,uint hPeer);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingSockets_SetConnectionName(IntPtr self,uint hPeer,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszName);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(IntPtr self,uint hPeer,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszName,int nMaxLen);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr self,uint hConn,ReadOnlySpan<byte> pData,uint cbData,int nSendFlags,out long pOutMessageNumber);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingSockets_SendMessages(IntPtr self,int nMessages,ReadOnlySpan<IntPtr> pMessages,Span<long> pOutMessageNumberOrResult);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(IntPtr self,uint hConn);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(IntPtr self,uint hConn,Span<IntPtr> ppOutMessages,int nMaxMessages);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(IntPtr self,uint hConn,ref SteamNetConnectionInfo pInfo);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_GetConnectionRealTimeStatus(IntPtr self,uint hConn,ref SteamNetConnectionRealTimeStatus pStats,int nLanes,Span<SteamNetConnectionRealTimeLaneStatus> pLanes);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(IntPtr self,uint hConn,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszBuf,int cbBuf);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(IntPtr self,uint hSocket,ref SteamNetworkingIPAddr address);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(IntPtr self,out HSteamNetConnection pOutConnection1,out HSteamNetConnection pOutConnection2,[MarshalAs(UnmanagedType.I1)] bool bUseNetworkLoopback,in SteamNetworkingIdentity pIdentity1,in SteamNetworkingIdentity pIdentity2);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_ConfigureConnectionLanes(IntPtr self,uint hConn,int nNumLanes,in int pLanePriorities,in ushort pLaneWeights);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_GetIdentity(IntPtr self,ref SteamNetworkingIdentity pIdentity);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingAvailability SteamAPI_ISteamNetworkingSockets_InitAuthentication(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingAvailability SteamAPI_ISteamNetworkingSockets_GetAuthenticationStatus(IntPtr self,ref SteamNetAuthenticationStatus pDetails);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_CreatePollGroup(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(IntPtr self,uint hPollGroup);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(IntPtr self,uint hConn,uint hPollGroup);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(IntPtr self,uint hPollGroup,Span<IntPtr> ppOutMessages,int nMaxMessages);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_ReceivedRelayAuthTicket(IntPtr self,ReadOnlySpan<byte> pvTicket,int cbTicket,out SteamDatagramRelayAuthTicket pOutParsedTicket);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingSockets_FindRelayAuthTicketForServer(IntPtr self,in SteamNetworkingIdentity identityGameServer,int nRemoteVirtualPort,out SteamDatagramRelayAuthTicket pOutParsedTicket);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_ConnectToHostedDedicatedServer(IntPtr self,in SteamNetworkingIdentity identityTarget,int nRemoteVirtualPort,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ushort SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerAddress(IntPtr self,ref SteamDatagramHostedAddress pRouting);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_CreateHostedDedicatedServerListenSocket(IntPtr self,int nLocalVirtualPort,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial EResult SteamAPI_ISteamNetworkingSockets_GetGameCoordinatorServerLogin(IntPtr self,ref SteamDatagramGameCoordinatorServerLogin pLoginInfo,ref int pcbSignedBlob, [MarshalUsing(CountElementName = nameof(pcbSignedBlob))]Span<byte> pBlob);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_ISteamNetworkingSockets_ConnectP2PCustomSignaling(IntPtr self,ref ISteamNetworkingConnectionSignaling pSignaling,in SteamNetworkingIdentity pPeerIdentity,int nRemoteVirtualPort,int nOptions,ReadOnlySpan<SteamNetworkingConfigValue> pOptions);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal(IntPtr self,[MarshalUsing(CountElementName = nameof(cbMsg))]ReadOnlySpan<byte> pMsg,int cbMsg,ref ISteamNetworkingSignalingRecvContext pContext);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_GetCertificateRequest(IntPtr self,ref int pcbBlob,[MarshalUsing(CountElementName = nameof(pcbBlob))]Span<byte> pBlob, ref SteamNetworkingErrMsg errMsg);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_SetCertificate(IntPtr self,ReadOnlySpan<byte> pCertificate,int cbCertificate,ref SteamNetworkingErrMsg errMsg);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingSockets_RunCallbacks(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_SteamNetworkingUtils_v003();

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_ISteamNetworkingUtils_AllocateMessage(IntPtr self,int cbAllocateBuffer);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingAvailability SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus(IntPtr self,ref SteamRelayNetworkStatus pDetails);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial float SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation(IntPtr self,ref SteamNetworkPingLocation result);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations(IntPtr self,in SteamNetworkPingLocation location1,in SteamNetworkPingLocation location2);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost(IntPtr self,in SteamNetworkPingLocation remoteLocation);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString(IntPtr self,in SteamNetworkPingLocation location,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszBuf,int cchBufSize);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_ParsePingLocationString(IntPtr self,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszString,ref SteamNetworkPingLocation result);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate(IntPtr self,float flMaxAgeSeconds);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter(IntPtr self,uint popID,ref SteamNetworkingPOPID pViaRelayPoP);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP(IntPtr self,uint popID);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_GetPOPCount(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial int SteamAPI_ISteamNetworkingUtils_GetPOPList(IntPtr self, [MarshalUsing(CountElementName = nameof(nListSz))] Span<SteamNetworkingPOPID> list,int nListSz);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial long SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr self,ESteamNetworkingSocketsDebugOutputType eDetailLevel,FSteamNetworkingSocketsDebugOutput pfnFunc);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32(IntPtr self,ESteamNetworkingConfigValue eValue,int val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat(IntPtr self,ESteamNetworkingConfigValue eValue,float val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString(IntPtr self,ESteamNetworkingConfigValue eValue,[MarshalAs(UnmanagedType.LPUTF8Str)] string val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValuePtr(IntPtr self,ESteamNetworkingConfigValue eValue,byte val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32(IntPtr self,uint hConn,ESteamNetworkingConfigValue eValue,int val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat(IntPtr self,uint hConn,ESteamNetworkingConfigValue eValue,float val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString(IntPtr self,uint hConn,ESteamNetworkingConfigValue eValue,[MarshalAs(UnmanagedType.LPUTF8Str)] string val);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr self,IntPtr fnCallback);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetAuthenticationStatusChanged(IntPtr self,IntPtr fnCallback);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamRelayNetworkStatusChanged(IntPtr self,IntPtr fnCallback);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(IntPtr self,ESteamNetworkingConfigValue eValue,ESteamNetworkingConfigScope eScopeType,IntPtr scopeObj,ESteamNetworkingConfigDataType eDataType,ReadOnlySpan<byte> pArg);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr self,in SteamNetworkingConfigValue opt,ESteamNetworkingConfigScope eScopeType,IntPtr scopeObj);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingGetConfigValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(IntPtr self,ESteamNetworkingConfigValue eValue,ESteamNetworkingConfigScope eScopeType,IntPtr scopeObj,out ESteamNetworkingConfigDataType pOutDataType,IntPtr pResult,ref ulong cbResult);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        public static partial string SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(IntPtr self,ESteamNetworkingConfigValue eValue,out ESteamNetworkingConfigDataType pOutDataType,out ESteamNetworkingConfigScope pOutScope);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ESteamNetworkingConfigValue SteamAPI_ISteamNetworkingUtils_IterateGenericEditableConfigValues(IntPtr self,ESteamNetworkingConfigValue eCurrent,[MarshalAs(UnmanagedType.I1)] bool bEnumerateDevVars);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIPAddr_Clear(ref SteamNetworkingIPAddr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros(ref SteamNetworkingIPAddr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref SteamNetworkingIPAddr self,ReadOnlySpan<byte> ipv6,ushort nPort);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref SteamNetworkingIPAddr self,uint nIP,ushort nPort);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIPAddr_IsIPv4(ref SteamNetworkingIPAddr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(ref SteamNetworkingIPAddr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref SteamNetworkingIPAddr self,ushort nPort);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(ref SteamNetworkingIPAddr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(ref SteamNetworkingIPAddr self,in SteamNetworkingIPAddr x);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIPAddr_ToString(in SteamNetworkingIPAddr self,[MarshalAs(UnmanagedType.LPUTF8Str)] string buf,UIntPtr cbBuf,[MarshalAs(UnmanagedType.I1)] bool bWithPort);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIPAddr_ParseString(ref SteamNetworkingIPAddr self,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszStr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_Clear(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_IsInvalid(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_SetSteamID(ref SteamNetworkingIdentity self,ulong steamID);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ulong SteamAPI_SteamNetworkingIdentity_GetSteamID(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref SteamNetworkingIdentity self,ulong steamID);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_SetIPAddr(ref SteamNetworkingIdentity self,in SteamNetworkingIPAddr addr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_SteamNetworkingIdentity_GetIPAddr(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_SetLocalHost(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_IsLocalHost(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_SetGenericString(ref SteamNetworkingIdentity self,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszString);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_SteamNetworkingIdentity_GetGenericString(ref SteamNetworkingIdentity self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_SetGenericBytes(ref SteamNetworkingIdentity self,ReadOnlySpan<byte> data,uint cbLen);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_SteamNetworkingIdentity_GetGenericBytes(ref SteamNetworkingIdentity self,int cbLen);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_IsEqualTo(ref SteamNetworkingIdentity self,in SteamNetworkingIdentity x);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingIdentity_ToString(in SteamNetworkingIdentity self,[MarshalAs(UnmanagedType.LPUTF8Str)] string buf,UIntPtr cbBuf);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_SteamNetworkingIdentity_ParseString(ref SteamNetworkingIdentity self,UIntPtr sizeofIdentity,[MarshalAs(UnmanagedType.LPUTF8Str)] string pszStr);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial void SteamAPI_SteamNetworkingMessage_t_Release(IntPtr self);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        public static partial IntPtr SteamAPI_ISteamNetworkingSockets_CreateCustomSignaling(Span<byte> ctx,IntPtr fnSendSignal,IntPtr fnRelease);

        [LibraryImport("GameNetworkingSockets")]
        [UnmanagedCallConv(CallConvs=new [] { typeof(CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal2(IntPtr self,ReadOnlySpan<byte> pMsg,int cbMsg,Span<byte> ctx,IntPtr fnOnConnectRequest,IntPtr fnSendRejectionSignal);
        public const uint k_HSteamNetConnection_Invalid = 0;
        public const uint k_HSteamListenSocket_Invalid = 0;
        public const uint k_HSteamNetPollGroup_Invalid = 0;
        public const int k_cchMaxSteamNetworkingErrMsg = 1024;
        public const int k_cchSteamNetworkingMaxConnectionCloseReason = 128;
        public const int k_cchSteamNetworkingMaxConnectionDescription = 128;
        public const int k_cchSteamNetworkingMaxConnectionAppName = 32;
        public const int k_nSteamNetworkConnectionInfoFlags_Unauthenticated = 1;
        public const int k_nSteamNetworkConnectionInfoFlags_Unencrypted = 2;
        public const int k_nSteamNetworkConnectionInfoFlags_LoopbackBuffers = 4;
        public const int k_nSteamNetworkConnectionInfoFlags_Fast = 8;
        public const int k_nSteamNetworkConnectionInfoFlags_Relayed = 16;
        public const int k_nSteamNetworkConnectionInfoFlags_DualWifi = 32;
        public const int k_cbMaxSteamNetworkingSocketsMessageSizeSend = 512 * 1024;
        public const int k_nSteamNetworkingSend_Unreliable = 0;
        public const int k_nSteamNetworkingSend_NoNagle = 1;
        public const int k_nSteamNetworkingSend_UnreliableNoNagle = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_NoDelay = 4;
        public const int k_nSteamNetworkingSend_UnreliableNoDelay = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoDelay | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_Reliable = 8;
        public const int k_nSteamNetworkingSend_ReliableNoNagle = k_nSteamNetworkingSend_Reliable | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_UseCurrentThread = 16;
        public const int k_nSteamNetworkingSend_AutoRestartBrokenSession = 32;
        public const int k_cchMaxSteamNetworkingPingLocationString = 1024;
        public const int k_nSteamNetworkingPing_Failed = -1;
        public const int k_nSteamNetworkingPing_Unknown = -2;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_Default = -1;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_Disable = 0;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_Relay = 1;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_Private = 2;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_Public = 4;
        public const int k_nSteamNetworkingConfig_P2Pransport_ICE_Enable_All = 0x7fffffff;
        public const uint k_SteamDatagramPOPID_dev = ('d' << 16) | (('e') << 8) | ('v');
    }
}