using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    public class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = Activity.Current?.Id ?? string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
