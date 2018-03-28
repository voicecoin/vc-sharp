using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core
{
    /// <summary>
    /// System use this address to receive coin, different address per user
    /// </summary>
    public class TokenWalletAddress : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        public string Currency { get; set; }
    }
}
