using System.Runtime.InteropServices;
using Valve.Sockets.Types.Connection;

namespace Valve.Sockets.Types;

[StructLayout(LayoutKind.Sequential)]
public struct StatusInfo {
    private const int callback = Library.socketsCallbacks + 1;
    public uint connection;
    public Info connectionInfo;
    private State oldState;
}
