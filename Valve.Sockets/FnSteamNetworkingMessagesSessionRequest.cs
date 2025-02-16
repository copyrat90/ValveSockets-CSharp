using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnSteamNetworkingMessagesSessionRequest(ref SteamNetworkingMessagesSessionRequest param0);
}