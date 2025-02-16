using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool FSteamNetworkingSocketsCustomSignaling_SendSignal(Span<byte> ctx, uint hConn, in SteamNetConnectionInfo info, Span<byte> pMsg, int cbMsg);
}