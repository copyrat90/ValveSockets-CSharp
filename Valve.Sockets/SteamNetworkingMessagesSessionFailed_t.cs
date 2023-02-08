using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    /// <summary>
    /// <para>Posted when we fail to establish a connection, or we detect that communications</para>
    /// <para>have been disrupted it an unusual way.  There is no notification when a peer proactively</para>
    /// <para>closes the session.  ("Closed by peer" is not a concept of UDP-style communications, and</para>
    /// <para>SteamNetworkingMessages is primarily intended to make porting UDP code easy.)</para>
    /// </summary>
    /// <remarks>
    /// <para>Remember: callbacks are asynchronous.   See notes on SendMessageToUser,</para>
    /// <para>and k_nSteamNetworkingSend_AutoRestartBrokenSession in particular.</para>
    /// <para>Also, if a session times out due to inactivity, no callbacks will be posted.  The only</para>
    /// <para>way to detect that this is happening is that querying the session state may return</para>
    /// <para>none, connecting, and findingroute again.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct SteamNetworkingMessagesSessionFailed_t
    {
        public SteamNetConnectionInfo_t m_info;

        public const uint k_iCallback = Constants.k_iSteamNetworkingMessagesCallbacks + 2;
    }
}