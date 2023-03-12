using Newtonsoft.Json;

namespace Imagine.OpenAI
{
    public class OpenAICompletionResponse
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public List<OpenAICompletionChoice> Choices { get; set; }

        public class OpenAICompletionChoice
        {
            public string Text { get; set; }

            [JsonProperty("finish_reason")]
            public string FinishReason { get; set; }
        }
    }
}