namespace Imagine.OpenAI {
    public class OpenAITextCompletionRequest {
        public string Text { get; set; }
        public int MaxTokens { get; set; }
        public decimal Temperature { get; set; }
        public List<string> Stops { get; set; }
        public decimal PresencePenalty { get; set; }
        public decimal FrequencyPenalty { get; set; }

        public OpenAITextCompletionRequest() {
        }

        public OpenAITextCompletionRequest(OpenAIChatQuery query) {
            Text = string.Join(Environment.NewLine, query.Messages.Select(message => message.Content));
            MaxTokens = query.MaxTokens;
            Temperature = query.Temperature;
            Stops = query.Stops;
            PresencePenalty = query.PresencePenalty;
            FrequencyPenalty = query.FrequencyPenalty;
        }
    }
}