using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class FileInfo
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string ContainerName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public string ETag { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string Url { get; set; }
    }
}
