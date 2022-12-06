using System.Runtime.InteropServices;
using Valve.Sockets.Networking;

namespace Valve.Sockets.Types.Connection;

[StructLayout(LayoutKind.Sequential)]
public struct Info {
    public Identity identity;
    public long userData;
    public uint listenSocket;
    public Address address;
    private ushort pad;
    private uint popRemote;
    private uint popRelay;
    public State state;
    public int endReason;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string endDebug;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string connectionDescription;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    private uint[] reserved;
}
