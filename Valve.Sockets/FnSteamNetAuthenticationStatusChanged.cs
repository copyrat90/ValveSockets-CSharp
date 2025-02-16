using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnSteamNetAuthenticationStatusChanged(ref SteamNetAuthenticationStatus param0);
}