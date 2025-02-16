using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FnSteamNetConnectionStatusChanged(ref SteamNetConnectionStatusChangedCallback param0);
}