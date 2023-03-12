using System.Text;
using System.Text.RegularExpressions;

namespace Imagine.Extensions {
    public static class StringExtensions {
        
        public static string SplitCamelCase(this string input, char splitter = ' ') {
            return Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1" + splitter + "$2"), @"(\p{Ll})(\P{Ll})", "$1" + splitter + "$2");
        }

        public static IEnumerable<int> GetOccurancesOf(this string input, params string[] patterns) {

            foreach (var pattern in patterns) {
                var i = 0;

                while ((i = input.IndexOf(pattern, i)) != -1) {
                    yield return i;
                    i += pattern.Length;
                }
            }
        }
    }
}