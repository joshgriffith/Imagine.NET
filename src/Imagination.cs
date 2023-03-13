using System.Collections;
using System.Diagnostics;
using Newtonsoft.Json;
using Imagine.Extensions;
using Imagine.OpenAI;
using Imagine.Queryable;
using Imagine.Schemas;
using Imagine.Utilities;

namespace Imagine
{
    public class Imagination : IDisposable {
        private readonly OpenAIClient _client;
        private readonly ImaginationSettings _settings;

        public Imagination(string openAiKey, decimal temperature = 0)
            : this(openAiKey, new ImaginationSettings { Temperature = temperature }) {
        }

        public Imagination(string openAiKey, ImaginationSettings settings) {
            _client = new OpenAIClient(openAiKey);
            _settings = settings;
        }

        public void Dispose() {
            _client.Dispose();
        }

        public IQueryable<T> Imagine<T>(object data, string prompt = "", int count = 0) {
            return new ImaginaryQuery<T>(this, data, prompt, count);
        }

        public IQueryable<T> Imagine<T>(string prompt = "", int count = 0) {
            return new ImaginaryQuery<T>(this, "", prompt, count);
        }
        
        internal async Task<List<T>> ImagineInternal<T>(object data, string prompt, int count = 0) {
            var output = new List<T>();
            var type = typeof(T);

            var dataTypeName = "rules";

            if (data is IList list) {
                dataTypeName = list.GetType().GetGenericArguments().First().Name;
            }

            dataTypeName = dataTypeName.Pluralize();
            
            var indeterminate = count == 0;
            var remaining = count;
            var userMessage = string.Empty;

            if (data != null && data.ToString().Length > 0 && dataTypeName != "rules") {
                userMessage += $"Given these {dataTypeName.ToLower()}: {JsonConvert.SerializeObject(data, JsonSanitizer.Settings)}" + Environment.NewLine;
            }

            if (indeterminate) {
                userMessage += $"Generate {type.Name.Pluralize()}";
            }
            else {
                userMessage += $"Generate {count} {type.Name.Pluralize()} as JSON";
            }

            if (data != null && data.ToString().Length > 4 && dataTypeName == "rules") {
                userMessage += $" {JsonConvert.SerializeObject(data, JsonSanitizer.Settings)}";
            }

            if (!string.IsNullOrEmpty(prompt)) {
                userMessage += " with " + prompt;
            }
            
            var schema = TypescriptSchemaProvider.GetSchema(type);

            if (!schema.Members.Any()) {
                throw new Exception("No settable public fields or properties found on type: " + type.Name);
            }

            userMessage += Environment.NewLine + $"["; //{{ \"{schema.Members.First().Name}\":

            var systemMessage = schema.Schema + Environment.NewLine + $"Generate {type.Name.Pluralize()} as JSON using the provided {dataTypeName}. Do not respond with code. Do not respond with anything except valid JSON.";
            
            while (indeterminate || remaining > 0) {

                var query = _settings.GetQuery();

                query.AddMessage(systemMessage, OpenAIRoles.System);

                if (output.Any()) {
                    var samples = output.TakeLast(_settings.SampleContinuationCount).ToList().Shuffle();
                    var serialized = JsonConvert.SerializeObject(samples);
                    query.AddMessage(serialized[..^1] + ",", OpenAIRoles.Assistant);
                }
                else {
                    //query.AddMessage($"[{{ \"{schema.Members.First().Name}\":", OpenAIRoles.Assistant); //var {type.Name}Results = 
                }

                query.AddMessage(userMessage);

                OpenAICompletionResult result;

                if (_settings.Performance == ImaginationSettings.PerformanceMode.HighQuality) {
                    result = await _client.CompleteChatAsync(query);
                }
                else {
                    var request = query.ToTextCompletionRequest();
                    request.PresencePenalty = 0.5m;
                    result = await _client.CompleteTextAsync(request);
                }

                var completedEntries = JsonSanitizer.Deserialize<T>(result.Text);

                if(completedEntries.Count == 0) {
                    throw new Exception("Failed to complete type: " + type.Name);
                }

                // Todo: Apply filter expression

                remaining -= completedEntries.Count;
                output.AddRange(completedEntries);

                Debug.WriteLine($"Received {completedEntries.Count} valid entries.");

                if (indeterminate && result.FinishReason == "stop" && count == 0) {
                    break;
                }
            }

            if (indeterminate || count == 0)
                return output.ToList();
            
            return output.Take(count).ToList();
        }
    }
}