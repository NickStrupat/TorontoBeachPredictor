using System;

namespace TorontoBeachPredictor.Service
{
    static class ParseExtensions
    {
        public static T? TryParse<T>(this String s, Func<String, T> parse) where T : struct => s == null ? (T?)null : parse(s);
        public static T? TryParse<T>(this String s, TryParseDelegate<T> tryParse) where T : struct => tryParse(s, out var result) ? result : (T?)null;
        public static String TryIntern(this String s) => s == null ? null : String.Intern(s);

        public delegate Boolean TryParseDelegate<T>(String s, out T result) where T : struct;
    }
}
