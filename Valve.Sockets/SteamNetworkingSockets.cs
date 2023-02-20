using System;

namespace Valve.Sockets
{
    public class SteamNetworkingSockets: ISteamNetworkingSockets
    {
        private readonly IntPtr _instance;

        public SteamNetworkingSockets()
        {
            _instance = Native.SteamAPI_SteamNetworkingSockets_v009();

            if (_instance == IntPtr.Zero)
            {
                throw new InvalidOperationException("SteamNetworkingSockets could not be initialized.");
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public uint CreateListenSocketIP(SteamNetworkingIPAddr localAddress, int nOptions, SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(_instance, localAddress, nOptions, pOptions);
        }

        public uint ConnectByIPAddress(SteamNetworkingIPAddr address, int nOptions, SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(_instance, address, nOptions, pOptions);
        }

        public uint CreateListenSocketP2P(int nLocalVirtualPort, int nOptions, SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketP2P(_instance,
                nLocalVirtualPort, nOptions, pOptions);
        }

        public uint ConnectP2P(SteamNetworkingIdentity identityRemote, int nRemoteVirtualPort, int nOptions,
            SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ConnectP2P(_instance, identityRemote, nRemoteVirtualPort,
                nOptions, pOptions);
        }

        public EResult AcceptConnection(uint hConn)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_AcceptConnection(_instance, hConn);
        }

        public bool CloseConnection(uint hPeer, int nReason, string pszDebug, bool bEnableLinger)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CloseConnection(_instance, hPeer, nReason, pszDebug, bEnableLinger);
        }

        public bool CloseListenSocket(uint hSocket)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CloseListenSocket(_instance, hSocket);
        }

        public bool SetConnectionUserData(uint hPeer, long nUserData)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(_instance, hPeer, nUserData);
        }

        public long GetConnectionUserData(uint hPeer)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(_instance, hPeer);
        }

        public void SetConnectionName(uint hPeer, string pszName)
        {
            Native.SteamAPI_ISteamNetworkingSockets_SetConnectionName(_instance, hPeer, pszName);
        }

        public bool GetConnectionName(uint hPeer, string pszName, int nMaxLen)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionName(_instance, hPeer, ref pszName, nMaxLen);
        }

        public EResult SendMessageToConnection(uint hConn, byte[] pData, uint cbData, int nSendFlags, ref long pOutMessageNumber)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(_instance, hConn, pData, cbData,
                nSendFlags, ref pOutMessageNumber);
        }

        public void SendMessages(int nMessages, SteamNetworkingMessage_t[] pMessages, ref long pOutMessageNumberOrResult)
        {
            Native.SteamAPI_ISteamNetworkingSockets_SendMessages(_instance, nMessages, ref pMessages, ref pOutMessageNumberOrResult);
        }

        public EResult FlushMessagesOnConnection(uint hConn)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(_instance, hConn);
        }

        public int ReceiveMessagesOnConnection(uint hConn, ref SteamNetworkingMessage_t[] ppOutMessages, int nMaxMessages)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(_instance, hConn,
                ref ppOutMessages, nMaxMessages);
        }

        public bool GetConnectionInfo(uint hConn, SteamNetConnectionInfo_t pInfo)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(_instance, hConn, ref pInfo);
        }

        public EResult GetConnectionRealTimeStatus(uint hConn, SteamNetConnectionRealTimeStatus_t pStatus, int nLanes,
            SteamNetConnectionRealTimeLaneStatus_t pLanes)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionRealTimeStatus(_instance, hConn, ref pStatus,
                nLanes, ref pLanes);
        }

        public int GetDetailedConnectionStatus(uint hConn, string pszBuf, int cbBuf)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(_instance, hConn, ref pszBuf, cbBuf);
        }

        public bool GetListenSocketAddress(uint hSocket, SteamNetworkingIPAddr address)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(_instance, hSocket, ref address);
        }

        public bool CreateSocketPair(ref HSteamNetConnection pOutConnection1, ref HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback,
            SteamNetworkingIdentity pIdentity1, SteamNetworkingIdentity pIdentity2)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CreateSocketPair(_instance, ref pOutConnection1,
                ref pOutConnection2, bUseNetworkLoopback, pIdentity1, pIdentity2);
        }

        public EResult ConfigureConnectionLanes(uint hConn, int nNumLanes, ref int pLanePriorities, ref ushort pLaneWeights)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ConfigureConnectionLanes(_instance, hConn, nNumLanes,
                pLanePriorities, pLaneWeights);
        }

        public bool GetIdentity(SteamNetworkingIdentity pIdentity)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetIdentity(_instance, ref pIdentity);
        }

        public ESteamNetworkingAvailability InitAuthentication()
        {
            return Native.SteamAPI_ISteamNetworkingSockets_InitAuthentication(_instance);
        }

        public ESteamNetworkingAvailability GetAuthenticationStatus(SteamNetAuthenticationStatus_t pDetails)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetAuthenticationStatus(_instance, ref pDetails);
        }

        public uint CreatePollGroup()
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CreatePollGroup(_instance);
        }

        public bool DestroyPollGroup(uint hPollGroup)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(_instance, hPollGroup);
        }

        public bool SetConnectionPollGroup(uint hConn, uint hPollGroup)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(_instance, hConn, hPollGroup);
        }

        public int ReceiveMessagesOnPollGroup(uint hPollGroup, ref SteamNetworkingMessage_t[] ppOutMessages, int nMaxMessages)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(_instance, hPollGroup,
                ref ppOutMessages, nMaxMessages);
        }

        public bool ReceivedRelayAuthTicket(byte[] pvTicket, int cbTicket, SteamDatagramRelayAuthTicket pOutParsedTicket)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ReceivedRelayAuthTicket(_instance, pvTicket, cbTicket,
                ref pOutParsedTicket);
        }

        public int FindRelayAuthTicketForServer(SteamNetworkingIdentity identityGameServer, int nRemoteVirtualPort,
            SteamDatagramRelayAuthTicket pOutParsedTicket)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_FindRelayAuthTicketForServer(_instance, identityGameServer,
                nRemoteVirtualPort, ref pOutParsedTicket);
        }

        public uint ConnectToHostedDedicatedServer(SteamNetworkingIdentity identityTarget, int nRemoteVirtualPort, int nOptions,
            SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ConnectToHostedDedicatedServer(_instance, identityTarget,
                nRemoteVirtualPort, nOptions, pOptions);
        }

        public ushort GetHostedDedicatedServerPort()
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort(_instance);
        }

        public uint GetHostedDedicatedServerPOPID()
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID(_instance);
        }

        public EResult GetHostedDedicatedServerAddress(ref SteamDatagramHostedAddress pRouting)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerAddress(_instance, ref pRouting);
        }

        public uint CreateHostedDedicatedServerListenSocket(int nLocalVirtualPort, int nOptions,
            SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_CreateHostedDedicatedServerListenSocket(_instance,
                nLocalVirtualPort, nOptions, pOptions);
        }

        public EResult GetGameCoordinatorServerLogin(SteamDatagramGameCoordinatorServerLogin pLoginInfo, ref int pcbSignedBlob,
            ref byte[] pBlob)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetGameCoordinatorServerLogin(_instance, ref pLoginInfo,
                ref pcbSignedBlob, ref pBlob);
        }

        public uint ConnectP2PCustomSignaling(ISteamNetworkingConnectionSignaling pSignaling, SteamNetworkingIdentity pPeerIdentity,
            int nRemoteVirtualPort, int nOptions, SteamNetworkingConfigValue_t pOptions)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_ConnectP2PCustomSignaling(_instance, ref pSignaling,
                pPeerIdentity, nRemoteVirtualPort, nOptions, pOptions);
        }

        public bool ReceivedP2PCustomSignal(ref byte[] pMsg, int cbMsg, ISteamNetworkingSignalingRecvContext pContext)
        {
            // TODO: Check if this is correct
            return Native.SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal(_instance, ref pMsg, cbMsg,
                ref pContext);
        }

        public bool GetCertificateRequest(ref int pcbBlob, ref byte[] pBlob, ref SteamNetworkingErrMsg errMsg)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_GetCertificateRequest(_instance, ref pcbBlob, ref pBlob,
                ref errMsg);
        }

        public bool SetCertificate(byte[] pCertificate, int cbCertificate, ref SteamNetworkingErrMsg errMsg)
        {
            return Native.SteamAPI_ISteamNetworkingSockets_SetCertificate(_instance, pCertificate, cbCertificate,
                ref errMsg);
        }

        public void ResetIdentity(SteamNetworkingIdentity pIdentity)
        {
            throw new NotImplementedException();
        }

        public void RunCallbacks()
        {
            Native.SteamAPI_ISteamNetworkingSockets_RunCallbacks(_instance);
        }

        public bool BeginAsyncRequestFakeIP(int nNumPorts)
        {
            throw new NotImplementedException();
        }

        public void GetFakeIP()
        {
            throw new NotImplementedException();
        }

        public uint CreateListenSocketP2PFakeIP(int idxFakePort, int nOptions, SteamNetworkingConfigValue_t pOptions)
        {
            throw new NotImplementedException();
        }

        public EResult GetRemoteFakeIPForConnection(uint hConn, SteamNetworkingIPAddr pOutAddr)
        {
            throw new NotImplementedException();
        }

        public ISteamNetworkingFakeUDPPort CreateFakeUDPPort(int idxFakeServerPort)
        {
            throw new NotImplementedException();
        }

        public ushort HostedDedicatedServerPort =>
            Native.SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort(_instance);

        public uint HostedDedicatedServerPOPID =>
            Native.SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID(_instance);
    }
}