using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FSteamNetworkingSocketsDebugOutput(ESteamNetworkingSocketsDebugOutputType nType, sbyte* pszMsg);
}
