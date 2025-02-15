using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = Native.PackSize)]
    public partial struct ValvePackingSentinel
    {
        public uint m_u32;
        public ulong m_u64;
        public ushort m_u16;

        public double m_d;
    }
}