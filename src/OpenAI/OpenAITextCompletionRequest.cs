namespace Imagine.OpenAI {
    public struct OpenAITextCompletionRequest {
        public string Text { get; set; }
        public int MaxTokens { get; set; }
        public decimal Temperature { get; set; }
        public List<string> Stops { get; set; }
        public decimal PresencePenalty { get; set; }
        public decimal FrequencyPenalty { get; set; }
    }
}