using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Valve.Sockets.Networking;
using Valve.Sockets.Types;
using Valve.Sockets.Types.Configuration;
using Valve.Sockets.Types.Connection;

namespace Valve.Sockets;

[SuppressUnmanagedCodeSecurity]
internal static class Native {
    private const string NativeLibrary = "GameNetworkingSockets";

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool GameNetworkingSockets_Init(IntPtr identity, StringBuilder errorMessage);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool GameNetworkingSockets_Init(ref Identity identity, StringBuilder errorMessage);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void GameNetworkingSockets_Kill();

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_SteamNetworkingSockets_v009();

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr SteamAPI_SteamNetworkingUtils_v003();

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr sockets, ref Address address, int configurationsCount, IntPtr configurations);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr sockets, ref Address address, int configurationsCount, Configuration[] configurations);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr sockets, ref Address address, int configurationsCount, IntPtr configurations);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr sockets, ref Address address, int configurationsCount, Configuration[] configurations);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Result SteamAPI_ISteamNetworkingSockets_AcceptConnection(IntPtr sockets, uint connection);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseConnection(IntPtr sockets, uint peer, int reason, string debug, bool enableLinger);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(IntPtr sockets, uint socket);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(IntPtr sockets, uint peer, long userData);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(IntPtr sockets, uint peer);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamNetworkingSockets_SetConnectionName(IntPtr sockets, uint peer, string name);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(IntPtr sockets, uint peer, StringBuilder name, int maxLength);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr sockets, uint connection, IntPtr data, uint length, SendFlags flags, IntPtr outMessageNumber);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr sockets, uint connection, byte[] data, uint length, SendFlags flags, IntPtr outMessageNumber);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Result SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(IntPtr sockets, uint connection);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(IntPtr sockets, uint connection, IntPtr[] messages, int maxMessages);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(IntPtr sockets, uint connection, ref Info info);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(IntPtr sockets, uint connection, ref Status status);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(IntPtr sockets, uint connection, StringBuilder status, int statusLength);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(IntPtr sockets, uint socket, ref Address address);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamNetworkingSockets_RunCallbacks(IntPtr sockets);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(IntPtr sockets, uint connectionLeft, uint connectionRight, bool useNetworkLoopback, ref Identity identityLeft, ref Identity identityRight);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_GetIdentity(IntPtr sockets, ref Identity identity);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_ISteamNetworkingSockets_CreatePollGroup(IntPtr sockets);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(IntPtr sockets, uint pollGroup);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(IntPtr sockets, uint connection, uint pollGroup);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(IntPtr sockets, uint pollGroup, IntPtr[] messages, int maxMessages);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref Address address, byte[] ip, ushort port);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref Address address, uint ip, ushort port);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(ref Address address);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref Address address, ushort port);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(ref Address address);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(ref Address address, ref Address other);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_SteamNetworkingIdentity_IsInvalid(ref Identity identity);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref Identity identity, ulong steamID);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(ref Identity identity);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr utils);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr utils, IntPtr callback);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr utils, StatusCallback callback);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr utils, DebugType detailLevel, IntPtr callback);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr utils, DebugType detailLevel, DebugCallback callback);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(IntPtr utils, Value configurationValue, Scope configurationScope, IntPtr scopeObject, DataType dataType, IntPtr value);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr utils, Configuration configuration, Scope configurationScope, IntPtr scopeObject);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(IntPtr utils, Value configurationValue, Scope configurationScope, IntPtr scopeObject, ref DataType dataType, ref IntPtr result, ref IntPtr resultLength);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Value SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(IntPtr utils);

    [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void SteamAPI_SteamNetworkingMessage_t_Release(IntPtr nativeMessage);
}
