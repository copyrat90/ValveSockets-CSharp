using System.Runtime.InteropServices;

namespace Valve.Sockets.Networking;

[StructLayout(LayoutKind.Sequential)]
public struct Message {
    public IntPtr data;
    public int length;
    public uint connection;
    public Identity identity;
    public long connectionUserData;
    public long timeReceived;
    public long messageNumber;
    internal IntPtr freeData;
    internal IntPtr release;
    public int channel;
    public int flags;
    public long userData;

    public void CopyTo(byte[] destination) {
        if (destination == null)
            throw new ArgumentNullException("destination");

        Marshal.Copy(data, destination, 0, length);
    }

#if !VALVESOCKETS_SPAN
    public void Destroy() {
        if (release == IntPtr.Zero)
            throw new InvalidOperationException("Message not created");

        Native.SteamAPI_SteamNetworkingMessage_t_Release(release);
    }
#endif
}
