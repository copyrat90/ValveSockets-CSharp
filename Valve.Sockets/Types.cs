using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Valve.Sockets;

// These are unlikely to change, but not as easy to automatically extract in a nice format.
// So the easiest solution is to provide them manually.
// Types are from: GameNetworkingSockets/include/steam/steamnetworkingtypes.h


/// Handle used to identify a connection to a remote host.
public struct HSteamNetConnection
{
    public uint Handle;
}

/// Handle used to identify a "listen socket".  Unlike traditional
/// Berkeley sockets, a listen socket and a connection are two
/// different abstractions.
public struct HSteamListenSocket
{
    public uint Handle;
}

/// Handle used to identify a poll group, used to query many
/// connections at once efficiently.
public struct HSteamNetPollGroup
{
    public uint Handle;
}

/// Used to return English-language diagnostic error messages to caller.
/// (For debugging or spewing to a console, etc.  Not intended for UI.)
[StructLayout(LayoutKind.Sequential, Size = 1024)]
public struct SteamNetworkingErrMsg
{
    [MarshalAs(UnmanagedType.LPStr)]
    [StringLength(1024)]
    public string Value;
}

/// Identifier used for a network location point of presence.  (E.g. a Valve data center.)
/// Typically you won't need to directly manipulate these.
public struct SteamNetworkingPOPID
{
    public uint Value;
}

/// A local timestamp.  You can subtract two timestamps to get the number of elapsed
/// microseconds.  This is guaranteed to increase over time during the lifetime
/// of a process, but not globally across runs.  You don't need to worry about
/// the value wrapping around.  Note that the underlying clock might not actually have
/// microsecond resolution.
public struct SteamNetworkingMicroseconds
{
    public long Value;
}