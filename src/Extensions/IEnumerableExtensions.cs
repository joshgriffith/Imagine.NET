using System.Collections;

namespace Imagine.Extensions {
    internal static class IEnumerableExtensions {
        private static readonly Random rng = new();  

        internal static IEnumerable<T> ForEachReverse<T>(this IList<T> source) {
            for (int count = source.Count - 1; count >= 0; count--) {
                yield return source[count];
            }
        }

        internal static IEnumerable<T> TakeLast<T>(this List<T> source, int count) {
            return source.Skip(Math.Max(0, source.Count() - count));
        }

        internal static T Shuffle<T>(this T list) where T : IList {
            var n = list.Count;
            while (n > 1) {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }
    }
}