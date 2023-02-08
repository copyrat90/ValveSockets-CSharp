using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FSteamNetworkingCustomSignalingRecvContext_SendRejectionSignal(void* ctx, SteamNetworkingIdentity* identityPeer, void* pMsg, int cbMsg);
}
