using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core
{
    /// <summary>
    /// User use this address to send coin
    /// </summary>
    public class UserWalletAddress : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        public string Currency { get; set; }
    }
}
