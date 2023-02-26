using System;

namespace Valve.Sockets
{
    public class SteamNetworkingMessages : ISteamNetworkingMessages
    {
        private readonly IntPtr _instance;
        public SteamNetworkingMessages()
        {
            _instance = Native.SteamNetworkingMessages();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public EResult SendMessageToUser(SteamNetworkingIdentity identityRemote, nint pubData, uint cubData, int nSendFlags,
            int nRemoteChannel)
        {
            throw new NotImplementedException();
        }

        public int ReceiveMessagesOnChannel(int nLocalChannel, SteamNetworkingMessage ppOutMessages, int nMaxMessages)
        {
            throw new NotImplementedException();
        }

        public bool AcceptSessionWithUser(SteamNetworkingIdentity identityRemote)
        {
            throw new NotImplementedException();
        }

        public bool CloseSessionWithUser(SteamNetworkingIdentity identityRemote)
        {
            throw new NotImplementedException();
        }

        public bool CloseChannelWithUser(SteamNetworkingIdentity identityRemote, int nLocalChannel)
        {
            throw new NotImplementedException();
        }

        public ESteamNetworkingConnectionState GetSessionConnectionInfo(SteamNetworkingIdentity identityRemote,
            SteamNetConnectionInfo pConnectionInfo, SteamNetConnectionRealTimeStatus pQuickStatus)
        {
            throw new NotImplementedException();
        }
    }
}