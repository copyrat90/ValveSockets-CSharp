using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool FSteamNetworkingSocketsCustomSignaling_SendSignal(void* ctx, uint hConn, SteamNetConnectionInfo_t* info, void* pMsg, int cbMsg);
}
