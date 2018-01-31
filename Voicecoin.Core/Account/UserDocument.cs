using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class UserDocument : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [StringLength(36)]
        public String FileStorageId { get; set; }

        [MaxLength(64)]
        public String Tag { get; set; }
    }
}
