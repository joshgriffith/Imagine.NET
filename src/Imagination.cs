using System.Collections;
using Newtonsoft.Json;
using Imagine.Extensions;
using Imagine.OpenAI;
using Imagine.Queryable;
using Imagine.Schemas;
using Imagine.Utilities;

namespace Imagine
{
    public class Imagination {
        private const int SampleContinuationCount = 5;
        private readonly OpenAIClient _client;
        private readonly decimal _temperature;
        private readonly int _maxTokens;

        public Imagination(string openAiKey, decimal temperature = 0, int maxTokens = 1024) {
            _client = new OpenAIClient(openAiKey);
            _temperature = temperature;
            _maxTokens = maxTokens;
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

            var dataTypeName = "rules";

            if (data is IList list) {
                dataTypeName = list.GetType().GetGenericArguments().First().Name;
            }

            dataTypeName = dataTypeName.Pluralize();
            
            var indeterminate = count == 0 || data is IList;
            var remaining = count;
            var userMessage = string.Empty;

            if (data != null && data.ToString().Length > 0) {
                userMessage += $"Given these {dataTypeName.ToLower()}: {JsonConvert.SerializeObject(data, JsonSanitizer.Settings)}" + Environment.NewLine;
            }

            if (indeterminate) {
                userMessage += $"Generate {type.Name.Pluralize()}";
            }
            else {
                userMessage += $"Generate {count} {type.Name.Pluralize()}";
            }

            if (!string.IsNullOrEmpty(metaPrompt)) {
                userMessage += " regarding " + metaPrompt;
            }
            
            while (indeterminate || remaining > 0) {

                var query = new OpenAIChatQuery {
                    MaxTokens = _maxTokens,
                    Temperature = _temperature
                };

                query.AddMessage(GetSystemMessage(type, dataTypeName), OpenAIRoles.System);
                query.AddMessage(userMessage);

                if (output.Any()) {
                    var samples = JsonConvert.SerializeObject(output.TakeLast(SampleContinuationCount));
                    query.AddMessage(samples[..^1] + ",", OpenAIRoles.Assistant);
                }
                else {
                    query.AddMessage($"var {type.Name}Results = [", OpenAIRoles.Assistant);
                }

                var result = await _client.CompleteChatAsync(query);

                var completedEntries = JsonSanitizer.Deserialize<T>(result.Text);

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
        
        private string GetSystemMessage(Type type, string dataTypeName) {
            var schema = TypescriptSchemaProvider.GetSchema(type);
            var systemMessage = schema + Environment.NewLine + Environment.NewLine + $"Generate {type.Name} as JSON using the provided {dataTypeName}. Do not respond with anything except JSON.";
            return systemMessage;
        }
    }
}