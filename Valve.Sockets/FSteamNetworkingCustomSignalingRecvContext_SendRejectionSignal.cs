using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FSteamNetworkingCustomSignalingRecvContext_SendRejectionSignal(Span<byte> ctx, in SteamNetworkingIdentity identityPeer, ReadOnlySpan<byte> pMsg, int cbMsg);
}
