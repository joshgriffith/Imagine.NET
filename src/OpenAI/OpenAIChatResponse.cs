using Newtonsoft.Json;

namespace Imagine.OpenAI
{
    public class OpenAIChatResponse
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public List<OpenAIChatChoice> Choices { get; set; }

        public class OpenAIChatChoice
        {
            public OpenAIChatMessage Message { get; set; }

            [JsonProperty("finish_reason")]
            public string FinishReason { get; set; }
        }

        public class OpenAIChatMessage
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }
    }
}