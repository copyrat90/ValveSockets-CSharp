using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamNetworkingMessagesSessionFailed(SteamNetworkingMessagesSessionFailed_t* param0);
}
