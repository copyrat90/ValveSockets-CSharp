using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamNetworkingFakeIPResult(SteamNetworkingFakeIPResult_t* param0);
}
