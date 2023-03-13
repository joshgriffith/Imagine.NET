using System;

namespace Imagine.OpenAI {
    public class OpenAIException : Exception {
        
        public string ErrorType { get; set; }
        public string ErrorCode { get; set; }

        public OpenAIException(OpenAIErrorResponse.ErrorData error)
            : base(error.Message) {
            ErrorType = error.Type;
            ErrorCode = error.Code;
        }
    }
}
