namespace Valve.Sockets
{
    /// <summary>
    /// <para>This callback is posted whenever a connection is created, destroyed, or changes state.</para>
    /// <para>The m_info field will contain a complete description of the connection at the time the</para>
    /// <para>change occurred and the callback was posted.  In particular, m_eState will have the</para>
    /// <para>new connection state.</para>
    /// </summary>
    /// <remarks>
    /// <para>You will usually need to listen for this callback to know when:</para>
    /// <para>- A new connection arrives on a listen socket.</para>
    /// <para>m_info.m_hListenSocket will be set, m_eOldState = k_ESteamNetworkingConnectionState_None,</para>
    /// <para>and m_info.m_eState = k_ESteamNetworkingConnectionState_Connecting.</para>
    /// <para>See ISteamNetworkigSockets::AcceptConnection.</para>
    /// <para>- A connection you initiated has been accepted by the remote host.</para>
    /// <para>m_eOldState = k_ESteamNetworkingConnectionState_Connecting, and</para>
    /// <para>m_info.m_eState = k_ESteamNetworkingConnectionState_Connected.</para>
    /// <para>Some connections might transition to k_ESteamNetworkingConnectionState_FindingRoute first.</para>
    /// <para>- A connection has been actively rejected or closed by the remote host.</para>
    /// <para>m_eOldState = k_ESteamNetworkingConnectionState_Connecting or k_ESteamNetworkingConnectionState_Connected,</para>
    /// <para>and m_info.m_eState = k_ESteamNetworkingConnectionState_ClosedByPeer.  m_info.m_eEndReason</para>
    /// <para>and m_info.m_szEndDebug will have for more details.</para>
    /// <para>NOTE: upon receiving this callback, you must still destroy the connection using</para>
    /// <para>ISteamNetworkingSockets::CloseConnection to free up local resources.  (The details</para>
    /// <para>passed to the function are not used in this case, since the connection is already closed.)</para>
    /// <para>- A problem was detected with the connection, and it has been closed by the local host.</para>
    /// <para>The most common failure is timeout, but other configuration or authentication failures</para>
    /// <para>can cause this.  m_eOldState = k_ESteamNetworkingConnectionState_Connecting or</para>
    /// <para>k_ESteamNetworkingConnectionState_Connected, and m_info.m_eState = k_ESteamNetworkingConnectionState_ProblemDetectedLocally.</para>
    /// <para>m_info.m_eEndReason and m_info.m_szEndDebug will have for more details.</para>
    /// <para>NOTE: upon receiving this callback, you must still destroy the connection using</para>
    /// <para>ISteamNetworkingSockets::CloseConnection to free up local resources.  (The details</para>
    /// <para>passed to the function are not used in this case, since the connection is already closed.)</para>
    /// <para>Remember that callbacks are posted to a queue, and networking connections can</para>
    /// <para>change at any time.  It is possible that the connection has already changed</para>
    /// <para>state by the time you process this callback.</para>
    /// <para>Also note that callbacks will be posted when connections are created and destroyed by your own API calls.</para>
    /// </remarks>
    public partial struct SteamNetConnectionStatusChangedCallback_t
    {
        public uint m_hConn;

        public SteamNetConnectionInfo_t m_info;

        public ESteamNetworkingConnectionState m_eOldState;

        public const uint k_iCallback = Constants.k_iSteamNetworkingSocketsCallbacks + 1;
    }
}