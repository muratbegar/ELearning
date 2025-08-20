using IskoopDemo.Shared.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.ErrorHandling
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
        public List<ValidationError> Errors { get; set; }
    }
}
