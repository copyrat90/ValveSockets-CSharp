using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamNetworkingMessagesSessionRequest(SteamNetworkingMessagesSessionRequest_t* param0);
}
