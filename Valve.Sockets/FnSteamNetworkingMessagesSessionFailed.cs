using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamNetworkingMessagesSessionFailed(SteamNetworkingMessagesSessionFailed* param0);
}