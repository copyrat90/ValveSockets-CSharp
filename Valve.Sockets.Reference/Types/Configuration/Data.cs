using System.Runtime.InteropServices;

namespace Valve.Sockets.Types.Configuration;

[StructLayout(LayoutKind.Explicit)]
public struct Data {
    [FieldOffset(0)]
    public int Int32;
    [FieldOffset(0)]
    public long Int64;
    [FieldOffset(0)]
    public float Float;
    [FieldOffset(0)]
    public IntPtr String;
    [FieldOffset(0)]
    public IntPtr FunctionPtr;
}
