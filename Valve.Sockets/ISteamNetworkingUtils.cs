// ----------------------------------------------------------------------------
// <auto-generated>
// This is autogenerated code by CppSharp.
// Do not edit this file or all your changes will be lost after re-generation.
// </auto-generated>
// ----------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Security;
using __CallingConvention = global::System.Runtime.InteropServices.CallingConvention;
using __IntPtr = global::System.IntPtr;


namespace Valve.Sockets
{
    /// <summary>
    /// <para>Misc networking utilities for checking the local networking environment</para>
    /// <para>and estimating pings.</para>
    /// </summary>
    public partial interface ISteamNetworkingUtils : IDisposable
    {
        /// <summary>
        /// <para>Allocate and initialize a message object.  Usually the reason</para>
        /// <para>you call this is to pass it to ISteamNetworkingSockets::SendMessages.</para>
        /// <para>The returned object will have all of the relevant fields cleared to zero.</para>
        /// </summary>
        /// <remarks>
        /// <para>Optionally you can also request that this system allocate space to</para>
        /// <para>hold the payload itself.  If cbAllocateBuffer is nonzero, the system</para>
        /// <para>will allocate memory to hold a payload of at least cbAllocateBuffer bytes.</para>
        /// <para>m_pData will point to the allocated buffer, m_cbSize will be set to the</para>
        /// <para>size, and m_pfnFreeData will be set to the proper function to free up</para>
        /// <para>the buffer.</para>
        /// <para>If cbAllocateBuffer=0, then no buffer is allocated.  m_pData will be NULL,</para>
        /// <para>m_cbSize will be zero, and m_pfnFreeData will be NULL.  You will need to</para>
        /// <para>set each of these.</para>
        /// </remarks>
        global::Valve.Sockets.SteamNetworkingMessage AllocateMessage(int cbAllocateBuffer);

        /// <summary>
        /// <para>If you know that you are going to be using the relay network (for example,</para>
        /// <para>because you anticipate making P2P connections), call this to initialize the</para>
        /// <para>relay network.  If you do not call this, the initialization will</para>
        /// <para>be delayed until the first time you use a feature that requires access</para>
        /// <para>to the relay network, which will delay that first access.</para>
        /// </summary>
        /// <remarks>
        /// <para>You can also call this to force a retry if the previous attempt has failed.</para>
        /// <para>Performing any action that requires access to the relay network will also</para>
        /// <para>trigger a retry, and so calling this function is never strictly necessary,</para>
        /// <para>but it can be useful to call it a program launch time, if access to the</para>
        /// <para>relay network is anticipated.</para>
        /// <para>Use GetRelayNetworkStatus or listen for SteamRelayNetworkStatus</para>
        /// <para>callbacks to know when initialization has completed.</para>
        /// <para>Typically initialization completes in a few seconds.</para>
        /// <para>Note: dedicated servers hosted in known data centers do *not* need</para>
        /// <para>to call this, since they do not make routing decisions.  However, if</para>
        /// <para>the dedicated server will be using P2P functionality, it will act as</para>
        /// <para>a "client" and this should be called.</para>
        /// </remarks>
        void InitRelayNetworkAccess();

        /// <summary>Fetch current status of the relay network.</summary>
        /// <remarks>
        /// <para>SteamRelayNetworkStatus is also a callback.  It will be triggered on</para>
        /// <para>both the user and gameserver interfaces any time the status changes, or</para>
        /// <para>ping measurement starts or stops.</para>
        /// <para>SteamRelayNetworkStatus::m_eAvail is returned.  If you want</para>
        /// <para>more details, you can pass a non-NULL value.</para>
        /// </remarks>
        global::Valve.Sockets.ESteamNetworkingAvailability GetRelayNetworkStatus(global::Valve.Sockets.SteamRelayNetworkStatus pDetails);

        /// <summary>
        /// <para>Return location info for the current host.  Returns the approximate</para>
        /// <para>age of the data, in seconds, or -1 if no data is available.</para>
        /// </summary>
        /// <remarks>
        /// <para>It takes a few seconds to initialize access to the relay network.  If</para>
        /// <para>you call this very soon after calling InitRelayNetworkAccess,</para>
        /// <para>the data may not be available yet.</para>
        /// <para>This always return the most up-to-date information we have available</para>
        /// <para>right now, even if we are in the middle of re-calculating ping times.</para>
        /// </remarks>
        float GetLocalPingLocation(ref global::Valve.Sockets.SteamNetworkPingLocation result);

        /// <summary>
        /// <para>Estimate the round-trip latency between two arbitrary locations, in</para>
        /// <para>milliseconds.  This is a conservative estimate, based on routing through</para>
        /// <para>the relay network.  For most basic relayed connections, this ping time</para>
        /// <para>will be pretty accurate, since it will be based on the route likely to</para>
        /// <para>be actually used.</para>
        /// </summary>
        /// <remarks>
        /// <para>If a direct IP route is used (perhaps via NAT traversal), then the route</para>
        /// <para>will be different, and the ping time might be better.  Or it might actually</para>
        /// <para>be a bit worse!  Standard IP routing is frequently suboptimal!</para>
        /// <para>But even in this case, the estimate obtained using this method is a</para>
        /// <para>reasonable upper bound on the ping time.  (Also it has the advantage</para>
        /// <para>of returning immediately and not sending any packets.)</para>
        /// <para>In a few cases we might not able to estimate the route.  In this case</para>
        /// <para>a negative value is returned.  k_nSteamNetworkingPing_Failed means</para>
        /// <para>the reason was because of some networking difficulty.  (Failure to</para>
        /// <para>ping, etc)  k_nSteamNetworkingPing_Unknown is returned if we cannot</para>
        /// <para>currently answer the question for some other reason.</para>
        /// <para>Do you need to be able to do this from a backend/matchmaking server?</para>
        /// <para>You are looking for the "game coordinator" library.</para>
        /// </remarks>
        int EstimatePingTimeBetweenTwoLocations(in global::Valve.Sockets.SteamNetworkPingLocation location1, in global::Valve.Sockets.SteamNetworkPingLocation location2);

        /// <summary>
        /// <para>Same as EstimatePingTime, but assumes that one location is the local host.</para>
        /// <para>This is a bit faster, especially if you need to calculate a bunch of</para>
        /// <para>these in a loop to find the fastest one.</para>
        /// </summary>
        /// <remarks>
        /// <para>In rare cases this might return a slightly different estimate than combining</para>
        /// <para>GetLocalPingLocation with EstimatePingTimeBetweenTwoLocations.  That's because</para>
        /// <para>this function uses a slightly more complete set of information about what</para>
        /// <para>route would be taken.</para>
        /// </remarks>
        int EstimatePingTimeFromLocalHost(in global::Valve.Sockets.SteamNetworkPingLocation remoteLocation);

        /// <summary>
        /// <para>Convert a ping location into a text format suitable for sending over the wire.</para>
        /// <para>The format is a compact and human readable.  However, it is subject to change</para>
        /// <para>so please do not parse it yourself.  Your buffer must be at least</para>
        /// <para>k_cchMaxSteamNetworkingPingLocationString bytes.</para>
        /// </summary>
        void ConvertPingLocationToString(in global::Valve.Sockets.SteamNetworkPingLocation location, string pszBuf, int cchBufSize);

        /// <summary>
        /// <para>Parse back SteamNetworkPingLocation string.  Returns false if we couldn't understand</para>
        /// <para>the string.</para>
        /// </summary>
        bool ParsePingLocationString(string pszString, ref global::Valve.Sockets.SteamNetworkPingLocation result);

        /// <summary>
        /// <para>Check if the ping data of sufficient recency is available, and if</para>
        /// <para>it's too old, start refreshing it.</para>
        /// </summary>
        /// <remarks>
        /// <para>Please only call this function when you *really* do need to force an</para>
        /// <para>immediate refresh of the data.  (For example, in response to a specific</para>
        /// <para>user input to refresh this information.)  Don't call it "just in case",</para>
        /// <para>before every connection, etc.  That will cause extra traffic to be sent</para>
        /// <para>for no benefit. The library will automatically refresh the information</para>
        /// <para>as needed.</para>
        /// <para>Returns true if sufficiently recent data is already available.</para>
        /// <para>Returns false if sufficiently recent data is not available.  In this</para>
        /// <para>case, ping measurement is initiated, if it is not already active.</para>
        /// <para>(You cannot restart a measurement already in progress.)</para>
        /// <para>You can use GetRelayNetworkStatus or listen for SteamRelayNetworkStatus</para>
        /// <para>to know when ping measurement completes.</para>
        /// </remarks>
        bool CheckPingDataUpToDate(float flMaxAgeSeconds);

        /// <summary>
        /// <para>Fetch ping time of best available relayed route from this host to</para>
        /// <para>the specified data center.</para>
        /// </summary>
        int GetPingToDataCenter(uint popID, ref SteamNetworkingPOPID pViaRelayPoP);

        /// <summary>Get *direct* ping time to the relays at the data center.</summary>
        int GetDirectPingToPOP(uint popID);

        /// <summary>Get number of network points of presence in the config</summary>
        int GetPOPCount();

        /// <summary>
        /// <para>Get list of all POP IDs.  Returns the number of entries that were filled into</para>
        /// <para>your list.</para>
        /// </summary>
        int GetPOPList(SteamNetworkingPOPID[] list, int nListSz);

        /// <summary>Fetch current timestamp.  This timer has the following properties:</summary>
        /// <remarks>
        /// <para>- Monotonicity is guaranteed.</para>
        /// <para>- The initial value will be at least 24*3600*30*1e6, i.e. about</para>
        /// <para>30 days worth of microseconds.  In this way, the timestamp value of</para>
        /// <para>0 will always be at least "30 days ago".  Also, negative numbers</para>
        /// <para>will never be returned.</para>
        /// <para>- Wraparound / overflow is not a practical concern.</para>
        /// <para>If you are running under the debugger and stop the process, the clock</para>
        /// <para>might not advance the full wall clock time that has elapsed between</para>
        /// <para>calls.  If the process is not blocked from normal operation, the</para>
        /// <para>timestamp values will track wall clock time, even if you don't call</para>
        /// <para>the function frequently.</para>
        /// <para>The value is only meaningful for this run of the process.  Don't compare</para>
        /// <para>it to values obtained on another computer, or other runs of the same process.</para>
        /// </remarks>
        long GetLocalTimestamp();

        /// <summary>
        /// <para>Set a function to receive network-related information that is useful for debugging.</para>
        /// <para>This can be very useful during development, but it can also be useful for troubleshooting</para>
        /// <para>problems with tech savvy end users.  If you have a console or other log that customers</para>
        /// <para>can examine, these log messages can often be helpful to troubleshoot network issues.</para>
        /// <para>(Especially any warning/error messages.)</para>
        /// </summary>
        /// <remarks>
        /// <para>The detail level indicates what message to invoke your callback on.  Lower numeric</para>
        /// <para>value means more important, and the value you pass is the lowest priority (highest</para>
        /// <para>numeric value) you wish to receive callbacks for.</para>
        /// <para>The value here controls the detail level for most messages.  You can control the</para>
        /// <para>detail level for various subsystems (perhaps only for certain connections) by</para>
        /// <para>adjusting the configuration values k_ESteamNetworkingConfig_LogLevel_Xxxxx.</para>
        /// <para>Except when debugging, you should only use k_ESteamNetworkingSocketsDebugOutputType_Msg</para>
        /// <para>or k_ESteamNetworkingSocketsDebugOutputType_Warning.  For best performance, do NOT</para>
        /// <para>request a high detail level and then filter out messages in your callback.  This incurs</para>
        /// <para>all of the expense of formatting the messages, which are then discarded.  Setting a high</para>
        /// <para>priority value (low numeric value) here allows the library to avoid doing this work.</para>
        /// <para>IMPORTANT: This may be called from a service thread, while we own a mutex, etc.</para>
        /// <para>Your output function must be threadsafe and fast!  Do not make any other</para>
        /// <para>Steamworks calls from within the handler.</para>
        /// </remarks>
        void SetDebugOutputFunction(global::Valve.Sockets.ESteamNetworkingSocketsDebugOutputType eDetailLevel, global::Valve.Sockets.FSteamNetworkingSocketsDebugOutput pfnFunc);

        /// <summary>
        /// <para>Return true if an IPv4 address is one that might be used as a "fake" one.</para>
        /// <para>This function is fast; it just does some logical tests on the IP and does</para>
        /// <para>not need to do any lookup operations.</para>
        /// </summary>
        bool IsFakeIPv4();

        global::Valve.Sockets.ESteamNetworkingFakeIPType GetIPv4FakeIPType(uint nIPv4);

        /// <summary>Get the real identity associated with a given FakeIP.</summary>
        /// <remarks>
        /// <para>On failure, returns:</para>
        /// <para>- k_EResultInvalidParam: the IP is not a FakeIP.</para>
        /// <para>- k_EResultNoMatch: we don't recognize that FakeIP and don't know the corresponding identity.</para>
        /// <para>FakeIP's used by active connections, or the FakeIPs assigned to local identities,</para>
        /// <para>will always work.  FakeIPs for recently destroyed connections will continue to</para>
        /// <para>return results for a little while, but not forever.  At some point, we will forget</para>
        /// <para>FakeIPs to save space.  It's reasonably safe to assume that you can read back the</para>
        /// <para>real identity of a connection very soon after it is destroyed.  But do not wait</para>
        /// <para>indefinitely.</para>
        /// </remarks>
        global::Valve.Sockets.EResult GetRealIdentityForFakeIP(global::Valve.Sockets.SteamNetworkingIPAddr fakeIP, global::Valve.Sockets.SteamNetworkingIdentity pOutRealIdentity);

        /// <summary>
        /// <para>Set a configuration value.</para>
        /// <para>- eValue: which value is being set</para>
        /// <para>- eScope: Onto what type of object are you applying the setting?</para>
        /// <para>- scopeArg: Which object you want to change?  (Ignored for global scope).  E.g. connection handle, listen socket handle, interface pointer, etc.</para>
        /// <para>- eDataType: What type of data is in the buffer at pValue?  This must match the type of the variable exactly!</para>
        /// <para>- pArg: Value to set it to.  You can pass NULL to remove a non-global setting at this scope,</para>
        /// <para>causing the value for that object to use global defaults.  Or at global scope, passing NULL</para>
        /// <para>will reset any custom value and restore it to the system default.</para>
        /// <para>NOTE: When setting pointers (e.g. callback functions), do not pass the function pointer directly.</para>
        /// <para>Your argument should be a pointer to a function pointer.</para>
        /// </summary>
        bool SetConfigValue(global::Valve.Sockets.ESteamNetworkingConfigValue eValue, global::Valve.Sockets.ESteamNetworkingConfigScope eScopeType, nint scopeObj, global::Valve.Sockets.ESteamNetworkingConfigDataType eDataType, byte[] pArg);

        /// <summary>
        /// <para>Set a configuration value, using a struct to pass the value.</para>
        /// <para>(This is just a convenience shortcut; see below for the implementation and</para>
        /// <para>a little insight into how SteamNetworkingConfigValue is used when</para>
        /// <para>setting config options during listen socket and connection creation.)</para>
        /// </summary>
        bool SetConfigValueStruct();

        /// <summary>
        /// <para>Get a configuration value.</para>
        /// <para>- eValue: which value to fetch</para>
        /// <para>- eScopeType: query setting on what type of object</para>
        /// <para>- eScopeArg: the object to query the setting for</para>
        /// <para>- pOutDataType: If non-NULL, the data type of the value is returned.</para>
        /// <para>- pResult: Where to put the result.  Pass NULL to query the required buffer size.  (k_ESteamNetworkingGetConfigValue_BufferTooSmall will be returned.)</para>
        /// <para>- cbResult: IN: the size of your buffer.  OUT: the number of bytes filled in or required.</para>
        /// </summary>
        global::Valve.Sockets.ESteamNetworkingGetConfigValueResult GetConfigValue(global::Valve.Sockets.ESteamNetworkingConfigValue eValue, global::Valve.Sockets.ESteamNetworkingConfigScope eScopeType, nint scopeObj, ref global::Valve.Sockets.ESteamNetworkingConfigDataType pOutDataType, __IntPtr pResult, ref ulong cbResult);

        /// <summary>
        /// <para>Get info about a configuration value.  Returns the name of the value,</para>
        /// <para>or NULL if the value doesn't exist.  Other output parameters can be NULL</para>
        /// <para>if you do not need them.</para>
        /// </summary>
        string GetConfigValueInfo(global::Valve.Sockets.ESteamNetworkingConfigValue eValue, ref global::Valve.Sockets.ESteamNetworkingConfigDataType pOutDataType, ref global::Valve.Sockets.ESteamNetworkingConfigScope pOutScope);

        /// <summary>
        /// <para>Iterate the list of all configuration values in the current environment that it might</para>
        /// <para>be possible to display or edit using a generic UI.  To get the first iterable value,</para>
        /// <para>pass k_ESteamNetworkingConfig_Invalid.  Returns k_ESteamNetworkingConfig_Invalid</para>
        /// <para>to signal end of list.</para>
        /// </summary>
        /// <remarks>
        /// <para>The bEnumerateDevVars argument can be used to include "dev" vars.  These are vars that</para>
        /// <para>are recommended to only be editable in "debug" or "dev" mode and typically should not be</para>
        /// <para>shown in a retail environment where a malicious local user might use this to cheat.</para>
        /// </remarks>
        global::Valve.Sockets.ESteamNetworkingConfigValue IterateGenericEditableConfigValues(global::Valve.Sockets.ESteamNetworkingConfigValue eCurrent, bool bEnumerateDevVars);

        void SteamNetworkingIPAddrToString(global::Valve.Sockets.SteamNetworkingIPAddr addr, string buf, ulong cbBuf, bool bWithPort);

        bool SteamNetworkingIPAddrParseString(global::Valve.Sockets.SteamNetworkingIPAddr pAddr, string pszStr);

        global::Valve.Sockets.ESteamNetworkingFakeIPType SteamNetworkingIPAddrGetFakeIPType(global::Valve.Sockets.SteamNetworkingIPAddr addr);

        void SteamNetworkingIdentityToString(global::Valve.Sockets.SteamNetworkingIdentity identity, string buf, ulong cbBuf);

        bool SteamNetworkingIdentityParseString(global::Valve.Sockets.SteamNetworkingIdentity pIdentity, string pszStr);
        void SteamNetworkingIPAddrToString();

        bool SteamNetworkingIPAddrParseString();

        global::Valve.Sockets.ESteamNetworkingFakeIPType SteamNetworkingIPAddrGetFakeIPType();

        void SteamNetworkingIdentityToString();

        bool SteamNetworkingIdentityParseString();

        /// <summary>Get number of network points of presence in the config</summary>
        int POPCount { get; }

        /// <summary>Fetch current timestamp.  This timer has the following properties:</summary>
        /// <remarks>
        /// <para>- Monotonicity is guaranteed.</para>
        /// <para>- The initial value will be at least 24*3600*30*1e6, i.e. about</para>
        /// <para>30 days worth of microseconds.  In this way, the timestamp value of</para>
        /// <para>0 will always be at least "30 days ago".  Also, negative numbers</para>
        /// <para>will never be returned.</para>
        /// <para>- Wraparound / overflow is not a practical concern.</para>
        /// <para>If you are running under the debugger and stop the process, the clock</para>
        /// <para>might not advance the full wall clock time that has elapsed between</para>
        /// <para>calls.  If the process is not blocked from normal operation, the</para>
        /// <para>timestamp values will track wall clock time, even if you don't call</para>
        /// <para>the function frequently.</para>
        /// <para>The value is only meaningful for this run of the process.  Don't compare</para>
        /// <para>it to values obtained on another computer, or other runs of the same process.</para>
        /// </remarks>
        long LocalTimestamp { get; }
    }
}