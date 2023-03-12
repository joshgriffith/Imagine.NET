namespace Imagine.Extensions {
    internal static class IEnumerableExtensions {
        internal static IEnumerable<T> ForEachReverse<T>(this IList<T> source) {
            for (int count = source.Count - 1; count >= 0; count--) {
                yield return source[count];
            }
        }

        internal static IEnumerable<T> TakeLast<T>(this List<T> source, int count) {
            return source.Skip(Math.Max(0, source.Count() - count));
        }
    }
}