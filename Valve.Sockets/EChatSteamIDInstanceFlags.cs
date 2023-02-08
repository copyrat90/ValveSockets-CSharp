namespace Valve.Sockets
{
    public enum EChatSteamIDInstanceFlags : uint
    {
        k_EChatAccountInstanceMask = 0x00000FFF,
        k_EChatInstanceFlagClan = (Constants.k_unSteamAccountInstanceMask + 1) >> 1,
        k_EChatInstanceFlagLobby = (Constants.k_unSteamAccountInstanceMask + 1) >> 2,
        k_EChatInstanceFlagMMSLobby = (Constants.k_unSteamAccountInstanceMask + 1) >> 3,
    }
}