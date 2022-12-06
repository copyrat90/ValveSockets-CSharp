namespace Valve.Sockets.Types;

[Flags]
public enum SendFlags {
    Unreliable = 0,
    NoNagle = 1 << 0,
    NoDelay = 1 << 2,
    Reliable = 1 << 3
}