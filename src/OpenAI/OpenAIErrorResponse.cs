namespace Imagine.OpenAI {
    public class OpenAIErrorResponse {

        public ErrorData Error { get; set; }

        public class ErrorData {
            public string Message { get; set; }
            public string Type { get; set; }
            public string Code { get; set; }
        }
    }
}
