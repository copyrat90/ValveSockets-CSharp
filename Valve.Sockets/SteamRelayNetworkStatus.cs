namespace Valve.Sockets
{
    public unsafe partial struct SteamRelayNetworkStatus
    {
        public ESteamNetworkingAvailability m_eAvail;

        public int m_bPingMeasurementInProgress;

        public ESteamNetworkingAvailability m_eAvailNetworkConfig;

        public ESteamNetworkingAvailability m_eAvailAnyRelay;
        public fixed sbyte m_debugMsg[256];

        public const uint k_iCallback = Constants.k_iSteamNetworkingUtilsCallbacks + 1;
    }
}