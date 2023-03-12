using System.Collections;
using Newtonsoft.Json;
using Imagine.Extensions;
using Imagine.OpenAI;
using Imagine.Queryable;
using Imagine.Schemas;

namespace Imagine
{
    public class Imagination {
        private readonly OpenAIClient _client;

        public Imagination(string openAiKey) {
            _client = new OpenAIClient(openAiKey);
        }

        public IQueryable<T> Imagine<T>(object data, string prompt = "", int count = 0) {
            return new ImaginaryQuery<T>(this, data, prompt, count);
        }

        public IQueryable<T> Imagine<T>(string prompt = "", int count = 0) {
            return new ImaginaryQuery<T>(this, "", prompt, count);
        }
        
        internal async Task<List<T>> ImagineInternal<T>(object data, string metaPrompt, int count = 0) {
            var output = new List<T>();
            var type = typeof(T);

            if (data == string.Empty) {
                data = $"get {count} {type.Name} results";
            }

            var metaType = "rules";

            if (data is IList list) {
                metaType = list.GetType().GetGenericArguments().First().Name;

                if (!metaType.EndsWith("s")) {
                    metaType += "s";
                }
            }

            var pluralizedType = type.Name;

            if (!type.Name.EndsWith("s")) {
                pluralizedType += "s";
            }

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            var indeterminate = count == 0 || data is IList;
            var remaining = count;
            var schema = TypescriptSchemaProvider.GetSchema(type);
            var systemMessage = schema + Environment.NewLine + Environment.NewLine + $"Generate {type.Name} as JSON using the provided {metaType}. Do not respond with anything except JSON.";
            var user = $"Given these {metaType.ToLower()}: {JsonConvert.SerializeObject(data, settings)}";

            if (indeterminate) {
                user += Environment.NewLine + $"Generate {pluralizedType}";
            }
            else {
                user += Environment.NewLine + $"Generate {count} {type.Name}";
            }

            if (!string.IsNullOrEmpty(metaPrompt)) {
                user += " regarding " + metaPrompt;
            }
            
            while (indeterminate || remaining > 0) {
                
                var messages = new List<OpenAIChatMessage> {
                    new() {
                        Content = systemMessage,
                        Role = "system"
                    },
                    new () {
                        Content = user,
                        Role = "user"
                    }
                };

                if (output.Any()) {
                    var samples = JsonConvert.SerializeObject(output.TakeLast(4));

                    messages.Add(new () {
                        Content = samples[..^1] + ",",
                        Role = "assistant"
                    });
                }
                else {
                    messages.Add(new () {
                        Content = $"let {type.Name}Results = [",
                        Role = "assistant"
                    });
                }

                var result = await _client.CompleteChatAsync(new OpenAIChatQuery {
                    Messages = messages,
                    MaxTokens = 1024,
                    Temperature = 0,
                    Stops = new List<string> {
                        "]",
                        "*/"
                    }
                });

                /*var result = await _client.CompleteTextAsync(new TextCompletionRequest {
                    Text = prompt,
                    MaxTokens = 1024,
                    Temperature = 0.2m,
                    Stops = new List<string> {
                        "]"
                    }
                });*/

                var json = result.Text;
                
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

                json = json.Substring(0, closedBraces[lastRecord - 1] + 1) + "]";

                var completedEntries = JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });

                if(completedEntries.Count == 0) {
                    throw new Exception("Failed to complete type: " + type.Name);
                }

                remaining -= completedEntries.Count;
                output.AddRange(completedEntries);

                if (indeterminate && result.FinishReason == "stop" && count == 0) {
                    break;
                }
            }

            if (indeterminate && count == 0)
                return output.ToList();
            
            return output.Take(count).ToList();
        }
    }
}