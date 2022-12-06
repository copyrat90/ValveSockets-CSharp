using System.Runtime.InteropServices;

namespace Valve.Sockets.Types.Configuration;

[StructLayout(LayoutKind.Sequential)]
public struct Configuration {
    public Value value;
    public DataType dataType;
    public Data data;
}
