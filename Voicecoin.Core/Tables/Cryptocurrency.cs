using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Voicecoin.Core.Models;

namespace Voicecoin.Core.Tables
{
    public class Cryptocurrency : DbRecord, IDbRecord
    {
        [Required]
        [MaxLength(16)]
        public CurrencyType Symbol { get; set; }

        [Required]
        [MaxLength(64)]
        public String Name { get; set; }

        /// <summary>
        /// Project website
        /// </summary>
        [MaxLength(256)]
        public String WebSite { get; set; }

        /// <summary>
        /// Source code repository url
        /// </summary>
        [MaxLength(256)]
        public String CodeRepository { get; set; }

        public CurrencyStatus Status { get; set; }

        public Decimal AvailableSupply { get; set; }

        public Decimal TotalSupply { get; set; }
    }
}
