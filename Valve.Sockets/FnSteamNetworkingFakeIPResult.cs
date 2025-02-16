using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnSteamNetworkingFakeIPResult(ref SteamNetworkingFakeIPResult param0);
}