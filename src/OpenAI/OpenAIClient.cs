using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Imagine.Extensions;
using Newtonsoft.Json;

namespace Imagine.OpenAI {

    public class OpenAIClient : IDisposable {

        public OpenAIEngines Engine { get; set; }

        private const string _baseUrl = "https://api.openai.com/v1/";
        private readonly HttpClient _client;

        public OpenAIClient(string apiKey, OpenAIEngines engine = OpenAIEngines.TextDavinci002) {
            _client = new HttpClient();
            SetApiKey(apiKey);

            Engine = engine;
        }

        public void Dispose() {
            _client?.Dispose();
        }

        public void SetApiKey(string apiKey) {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }
        
        public async Task<OpenAICompletionResult> CompleteChatAsync(OpenAIChatQuery query) {
            var url = _baseUrl + "chat/completions";

            var body = query.ToRequestBody();
            var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            serialized = serialized.Replace(@"\r", "");

            var serializedRequest = new StringContent(serialized, Encoding.UTF8, "application/json");

            var queryTime = DateTime.UtcNow;
            var response = await _client.PostAsync(url, serializedRequest);
            var json = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK) {
                var error = JsonConvert.DeserializeObject<OpenAIErrorResponse>(json);
                throw new OpenAIException(error.Error);
            }

            var result = JsonConvert.DeserializeObject<OpenAIChatResponse>(json);

            return new OpenAICompletionResult {
                Id = result.Id,
                Model = result.Model,
                Latency = DateTime.UtcNow - queryTime,
                Text = result.Choices[0].Message.Content,
                FinishReason = result.Choices[0].FinishReason
            };
        }

        public async Task<OpenAICompletionResult> CompleteTextAsync(OpenAITextCompletionRequest request) {

            return await Completion(new OpenAICompletionQuery(Engine, request.Text) {
                Temperature = request.Temperature,
                MaxTokens = request.MaxTokens,
                FrequencyPenalty = request.FrequencyPenalty,
                PresencePenalty = request.PresencePenalty,
                Stops = request.Stops
            });
        }

        public async Task<OpenAICompletionResult> Completion(OpenAICompletionQuery query) {
            var engineName = GetEngineName(query.Engine);
            var url = _baseUrl + $"engines/{engineName}/completions";
            var queryTime = DateTime.UtcNow;

            var body = query.ToRequestBody();
            var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            serialized = serialized.Replace(@"\r", "");

            var serializedRequest = new StringContent(serialized, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, serializedRequest);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OpenAICompletionResponse>(json);

            return new OpenAICompletionResult {
                Id = result.Id,
                Latency = DateTime.UtcNow - queryTime,
                Model = result.Model,
                Text = result.Choices[0].Text,
                FinishReason = result.Choices[0].FinishReason,
                Query = query
            };
        }

        public static string GetEngineName(OpenAIEngines engine) {
            return engine.ToString().SplitCamelCase('-').ToLower();
        }
    }
}