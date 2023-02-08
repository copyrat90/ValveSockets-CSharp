namespace Valve.Sockets
{
    public static class Constants
    {
        public const int k_iSteamNetworkingSocketsCallbacks = 1220;
        public const int k_iSteamNetworkingMessagesCallbacks = 1250;
        public const int k_iSteamNetworkingUtilsCallbacks = 1280;

        public const uint k_uAppIdInvalid = 0x0;
        public const uint k_uDepotIdInvalid = 0x0;
        public const ulong k_uAPICallInvalid = 0x0;
        public const ulong k_ulPartyBeaconIdInvalid = 0;

        public const uint k_unSteamAccountIDMask = 0xFFFFFFFF;
        public const uint k_unSteamAccountInstanceMask = 0x000FFFFF;
        public const uint k_unSteamUserDefaultInstance = 1;
    }
}