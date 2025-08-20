using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.File.Enums;

namespace IskoopDemo.Shared.Infrastructure.File.Models
{
    public class ImageResizeOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public ResizeMode Mode { get; set; } = ResizeMode.Crop;
        public ImageFormat Format { get; set; } = ImageFormat.Original;
        public int Quality { get; set; } = 85;
        public bool MaintainAspectRatio { get; set; } = true;
    }
}
