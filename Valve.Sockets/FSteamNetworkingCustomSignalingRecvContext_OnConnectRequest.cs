using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate ISteamNetworkingConnectionSignaling* FSteamNetworkingCustomSignalingRecvContext_OnConnectRequest(void* ctx, uint hConn, SteamNetworkingIdentity* identityPeer, int nLocalVirtualPort);
}
