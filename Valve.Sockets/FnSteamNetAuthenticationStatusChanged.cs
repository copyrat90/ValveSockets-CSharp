using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamNetAuthenticationStatusChanged(SteamNetAuthenticationStatus_t* param0);
}
