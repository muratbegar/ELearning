using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class PagedFileList
    {
        public IEnumerable<FileInfo> Files { get; set; }
        public string ContinuationToken { get; set; }
        public bool HasMore { get; set; }
        public int Count { get; set; }
    }
}
