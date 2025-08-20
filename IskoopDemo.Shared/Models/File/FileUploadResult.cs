using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class FileUploadResult
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContainerName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ETag { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
