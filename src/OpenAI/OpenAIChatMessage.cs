using Newtonsoft.Json;

namespace Imagine.OpenAI
{
    public class OpenAIChatMessage
    {

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
