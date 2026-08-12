using System;

public static class StringExtension
{
    public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);

}
