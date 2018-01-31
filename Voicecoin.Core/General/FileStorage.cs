using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core
{
    public class FileStorage : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String OriginalFileName { get; set; }

        [MaxLength(16)]
        public String ConvertedFileName { get; set; }

        public long Size { get; set; }

        [MaxLength(32)]
        public String ContentType { get; set; }

        [MaxLength(64)]
        public String Directory { get; set; }
    }
}
