using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnSteamNetworkingMessagesSessionFailed(ref SteamNetworkingMessagesSessionFailed param0);
}