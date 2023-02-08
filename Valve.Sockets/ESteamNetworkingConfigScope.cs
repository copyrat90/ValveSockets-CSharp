namespace Valve.Sockets
{
    public enum ESteamNetworkingConfigScope : uint
    {
        k_ESteamNetworkingConfig_Global = 1,
        k_ESteamNetworkingConfig_SocketsInterface = 2,
        k_ESteamNetworkingConfig_ListenSocket = 3,
        k_ESteamNetworkingConfig_Connection = 4,
        k_ESteamNetworkingConfigScope__Force32Bit = 0x7fffffff,
    }
}