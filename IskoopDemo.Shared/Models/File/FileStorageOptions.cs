using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class FileStorageOptions
    {
        public string ConnectionString { get; set; }
        public string DefaultContainer { get; set; }
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".mp4", ".mp3" };
        public string[] BlockedExtensions { get; set; } = { ".exe", ".bat", ".cmd", ".scr", ".pif", ".com" };
        public bool EnableVirusScanning { get; set; } = false;
        public bool EnableImageResizing { get; set; } = true;
        public TimeSpan DefaultUrlExpiration { get; set; } = TimeSpan.FromHours(1);
    }
}
