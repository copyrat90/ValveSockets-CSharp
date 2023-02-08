using System;
using System.Runtime.CompilerServices;

namespace LibraryGenerator
{
    public static class Helper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FirstToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return string.Create(input.Length, input, static (Span<char> chars, string str) =>
            {
                chars[0] = char.ToUpperInvariant(str[0]);
                str.AsSpan(1).CopyTo(chars[1..]);
            });
        }
    }
}