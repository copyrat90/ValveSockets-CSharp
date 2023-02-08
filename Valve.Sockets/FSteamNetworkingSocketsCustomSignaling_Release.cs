using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FSteamNetworkingSocketsCustomSignaling_Release(void* ctx);
}
