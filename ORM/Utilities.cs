using System.Collections.Generic;

namespace ORM
{
    public static class Utilities
    {
        public static string Implode<T>(this IEnumerable<T> parts)
        {
            return parts.Implode(", ");
        }

        public static string Implode<T>(this IEnumerable<T> parts, string separator)
        {
            return string.Join(separator, parts);
        }
    }
}
