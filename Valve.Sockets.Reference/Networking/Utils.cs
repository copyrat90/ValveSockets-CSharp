using Valve.Sockets.Types;
using Valve.Sockets.Types.Configuration;

namespace Valve.Sockets.Networking;

public class Utils : IDisposable {
    private IntPtr nativeUtils;

    public Utils() {
        nativeUtils = Native.SteamAPI_SteamNetworkingUtils_v003();

        if (nativeUtils == IntPtr.Zero)
            throw new InvalidOperationException("Networking utils not created");
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (nativeUtils != IntPtr.Zero) {
            Native.SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(nativeUtils, IntPtr.Zero);
            Native.SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(nativeUtils, DebugType.None, IntPtr.Zero);
            nativeUtils = IntPtr.Zero;
        }
    }

    ~Utils() {
        Dispose(false);
    }

    public long Time => Native.SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(nativeUtils);

    public Value FirstConfigurationValue => Native.SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(nativeUtils);

    public bool SetStatusCallback(StatusCallback callback) {
        return Native.SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(nativeUtils, callback);
    }

    public void SetDebugCallback(DebugType detailLevel, DebugCallback callback) {
        Native.SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(nativeUtils, detailLevel, callback);
    }

    public bool SetConfigurationValue(Value configurationValue, Scope configurationScope, IntPtr scopeObject, DataType dataType, IntPtr value) {
        return Native.SteamAPI_ISteamNetworkingUtils_SetConfigValue(nativeUtils, configurationValue, configurationScope, scopeObject, dataType, value);
    }

    public bool SetConfigurationValue(Configuration configuration, Scope configurationScope, IntPtr scopeObject) {
        return Native.SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(nativeUtils, configuration, configurationScope, scopeObject);
    }

    public ValueResult GetConfigurationValue(Value configurationValue, Scope configurationScope, IntPtr scopeObject, ref DataType dataType, ref IntPtr result, ref IntPtr resultLength) {
        return Native.SteamAPI_ISteamNetworkingUtils_GetConfigValue(nativeUtils, configurationValue, configurationScope, scopeObject, ref dataType, ref result, ref resultLength);
    }
}
