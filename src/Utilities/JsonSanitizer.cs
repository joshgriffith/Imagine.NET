using Imagine.Extensions;
using Newtonsoft.Json;

namespace Imagine.Utilities {
    internal static class JsonSanitizer {

        internal static readonly JsonSerializerSettings Settings = new() {
            NullValueHandling = NullValueHandling.Ignore
        };

        internal static List<T>? Deserialize<T>(string json) {
            var sanitized = Sanitize(json);

            try {
                return JsonConvert.DeserializeObject<List<T>>(sanitized, Settings);
            }
            catch (Exception exception) {
                throw new Exception($"Unable to parse: {sanitized}", exception);
            }
        }

        internal static string Sanitize(string json) {
            if (string.IsNullOrEmpty(json) || json.Length <= 4) {
                return "[]";
            }

            if (!json.StartsWith("[")) {
                var offset = json.IndexOf("[");

                if (offset >= 0) {
                    json = json.Substring(offset);
                }
                else {
                    json = "[" + json;
                }
            }
                
            var results = 0;

            foreach(var closedBrace in json.GetOccurancesOf("}").ToList().ForEachReverse()) {
                if (results > 0 && json[closedBrace + 1] == ',') {
                    continue;
                }

                json = json.Insert(closedBrace + 1, ",");
                results += 1;
            }

            var openBraces = json.GetOccurancesOf("{").ToList();
            var closedBraces = json.GetOccurancesOf("}").ToList();
            var lastRecord = Math.Min(openBraces.Count, closedBraces.Count);

            json = json.Substring(0, closedBraces[lastRecord - 1] + 1);

            if (!json.EndsWith("]")) {
                json += "]";
            }

            return json;
        }
    }
}
