using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class FileUploadRequest
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
