namespace Valve.Sockets;

internal static class ArrayPool {
    [ThreadStatic]
    private static IntPtr[] pointerBuffer;

    public static IntPtr[] GetPointerBuffer()
    {
        return pointerBuffer ??= new IntPtr[Library.maxMessagesPerBatch];
    }
}
