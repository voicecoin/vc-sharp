using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Voicecoin.Core.Models;

namespace Voicecoin.Core
{
    /// <summary>
    /// Staging infomation
    /// </summary>
    public class PriceStage : DbRecord, IDbRecord
    {
        /// <summary>
        /// Target Currency
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 1 base = 10 Amount Symbol
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// Base currency
        /// </summary>
        [Required]
        [MaxLength(8)]
        public CurrencyType Symbol { get; set; }

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
