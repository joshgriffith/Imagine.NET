namespace Imagine.OpenAI {
    public class OpenAICompletionQuery {

        public string Prompt { get; set; }
        public int MaxTokens { get; set; } = 16;
        public decimal Temperature { get; set; } = 0;
        public OpenAIEngines Engine { get; set; }
        public List<string> Stops { get; set; } = new();
        public decimal PresencePenalty { get; set; } = 0;
        public decimal FrequencyPenalty { get; set; } = 0;

        public OpenAICompletionQuery(OpenAIEngines engine, string prompt) {
            Engine = engine;
            Prompt = prompt;
            Stops = new List<string>();
        }

        public object ToRequestBody() {
            if (Stops != null && Stops.Any()) {
                return new {
                    prompt = Prompt,
                    max_tokens = MaxTokens,
                    temperature = Temperature,
                    top_p = 1,
                    stop = Stops.ToArray(),
                    presence_penalty = PresencePenalty,
                    frequency_penalty = FrequencyPenalty
                };
            }

            return new {
                prompt = Prompt,
                max_tokens = MaxTokens,
                temperature = Temperature,
                top_p = 1,
                presence_penalty = PresencePenalty,
                frequency_penalty = FrequencyPenalty
            };
        }
    }
}