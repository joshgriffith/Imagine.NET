namespace Imagine.OpenAI
{
    public class OpenAICompletionResult
    {
        public OpenAICompletionQuery Query { get; set; }
        public string Id { get; set; }
        public string Model { get; set; }
        public string Text { get; set; }
        public string FinishReason { get; set; }
        public int Latency { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}