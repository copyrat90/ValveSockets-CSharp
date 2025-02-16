using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr FSteamNetworkingCustomSignalingRecvContext_OnConnectRequest(Span<byte> ctx, uint hConn, in SteamNetworkingIdentity identityPeer, int nLocalVirtualPort);
}
