namespace Valve.Sockets
{
    public enum ESteamNetworkingFakeIPType : uint
    {
        k_ESteamNetworkingFakeIPType_Invalid,
        k_ESteamNetworkingFakeIPType_NotFake,
        k_ESteamNetworkingFakeIPType_GlobalIPv4,
        k_ESteamNetworkingFakeIPType_LocalIPv4,
        k_ESteamNetworkingFakeIPType__Force32Bit = 0x7fffffff,
    }
}