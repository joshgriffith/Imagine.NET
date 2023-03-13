namespace Imagine.OpenAI {

    public class OpenAIChatQuery {
        public string Model { get; set; } = "gpt-3.5-turbo";
        public List<OpenAIChatMessage> Messages { get; set; } = new();
        public int MaxTokens { get; set; } = 16;
        public decimal Temperature { get; set; } = 0;
        public List<string> Stops { get; set; } = new();
        public decimal PresencePenalty { get; set; } = 0;
        public decimal FrequencyPenalty { get; set; } = 0;

        public OpenAIChatQuery AddMessage(string content, OpenAIRoles role = OpenAIRoles.User) {
            Messages.Add(new OpenAIChatMessage {
                Content = content,
                Role = role.ToString().ToLower()
            });

            return this;
        }

        public OpenAITextCompletionRequest ToTextCompletionRequest() {
            return new OpenAITextCompletionRequest(this);
        }

        public object ToRequestBody() {
            if (Stops != null && Stops.Any()) {
                return new {
                    model = Model,
                    messages = Messages,
                    max_tokens = MaxTokens,
                    temperature = Temperature,
                    top_p = 1,
                    stop = Stops.ToArray(),
                    presence_penalty = PresencePenalty,
                    frequency_penalty = FrequencyPenalty
                };
            }

            return new {
                model = Model,
                messages = Messages,
                max_tokens = MaxTokens,
                temperature = Temperature,
                top_p = 1,
                presence_penalty = PresencePenalty,
                frequency_penalty = FrequencyPenalty
            };
        }
    }
}