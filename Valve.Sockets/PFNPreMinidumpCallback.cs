using System;
using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PFNPreMinidumpCallback(Span<byte> context);
}
