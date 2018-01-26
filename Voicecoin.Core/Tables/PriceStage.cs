using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Voicecoin.Core.Models;

namespace Voicecoin.Core.Tables
{
    /// <summary>
    /// Staging infomation
    /// </summary>
    public class PriceStage : DbRecord, IDbRecord
    {
        /// <summary>
        /// base Currency
        /// </summary>
        public String Base { get; set; }

        /// <summary>
        /// 1 base = 10 Amount Symbol
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// target currency
        /// </summary>
        [Required]
        [MaxLength(8)]
        public String Symbol { get; set; }

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
