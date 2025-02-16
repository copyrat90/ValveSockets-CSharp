using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FSteamNetworkingSocketsCustomSignaling_Release(Span<byte> ctx);
}
