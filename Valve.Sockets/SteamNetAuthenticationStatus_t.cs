namespace Valve.Sockets
{
    public unsafe partial struct SteamNetAuthenticationStatus_t
    {
        public ESteamNetworkingAvailability m_eAvail;
        public fixed sbyte m_debugMsg[256];

        public const uint k_iCallback = Constants.k_iSteamNetworkingSocketsCallbacks + 2;
    }
}