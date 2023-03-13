using System;
using Imagine.OpenAI;

namespace Imagine {
    public class ImaginationSettings {
        public decimal Temperature { get; set; } = 0;
        public int MaxTokens { get; set; } = 256;
        public int MaxParallelRequests { get; set; } = 2;
        public decimal FrequencyPenalty { get; set; }
        public decimal PresencePenalty { get; set; }
        public int SampleContinuationCount { get; set; } = 5;
        public PerformanceMode Performance { get; set; } = PerformanceMode.HighQuality;

        internal OpenAIChatQuery GetQuery() {
            return new OpenAIChatQuery {
                MaxTokens = MaxTokens,
                Temperature = Temperature,
                FrequencyPenalty = FrequencyPenalty,
                PresencePenalty = PresencePenalty
            };
        }

        public enum PerformanceMode {
            HighQuality,
            Fast
        }
    }
}