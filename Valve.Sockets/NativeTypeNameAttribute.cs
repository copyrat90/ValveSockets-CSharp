using System;

namespace Valve.Sockets;

[AttributeUsage(AttributeTargets.All)]
public class NativeTypeNameAttribute : Attribute
{
    public readonly string nativeType;

    public NativeTypeNameAttribute(string nativeType)
    {
        this.nativeType = nativeType;
    }
}