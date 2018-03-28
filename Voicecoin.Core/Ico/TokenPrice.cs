using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Voicecoin.Core.Models;

namespace Voicecoin.Core
{
    /// <summary>
    /// Currency rate. Format: N Token/ BTC
    /// </summary>
    public class TokenPrice : DbRecord, IDbRecord
    {
        /// <summary>
        /// Base Currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// N Symbol/ Currency
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Token symbol
        /// </summary>
        [Required]
        [MaxLength(8)]
        public string Symbol { get; set; }

        /// <summary>
        /// Start date of the Status
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Status of the stage
        /// </summary>
        public CurrencyStatus Status { get; set; }
    }
}
