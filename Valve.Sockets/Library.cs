using System.Text;
using Valve.Sockets.Networking;

namespace Valve.Sockets;

public static class Library {
    public const int maxCloseMessageLength = 128;
    public const int maxErrorMessageLength = 1024;
    public const int maxMessagesPerBatch = 256;
    public const int maxMessageSize = 512 * 1024;
    public const int socketsCallbacks = 1220;

    public static bool Initialize() {
        return Initialize(null);
    }

    public static bool Initialize(StringBuilder errorMessage) {
        if (errorMessage != null && errorMessage.Capacity != maxErrorMessageLength)
            throw new ArgumentOutOfRangeException("Capacity of the error message must be equal to " + maxErrorMessageLength);

        return Native.GameNetworkingSockets_Init(IntPtr.Zero, errorMessage);
    }

    public static bool Initialize(ref Identity identity, StringBuilder errorMessage) {
        if (errorMessage != null && errorMessage.Capacity != maxErrorMessageLength)
            throw new ArgumentOutOfRangeException("Capacity of the error message must be equal to " + maxErrorMessageLength);

        if (object.Equals(identity, null))
            throw new ArgumentNullException("identity");

        return Native.GameNetworkingSockets_Init(ref identity, errorMessage);
    }

    public static void Deinitialize() {
        Native.GameNetworkingSockets_Kill();
    }
}