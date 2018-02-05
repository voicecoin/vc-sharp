using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core
{
    /// <summary>
    /// Receive money address
    /// </summary>
    public class IcoContribution : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Total Amount
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Total Tokens
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Decimal Token { get; set; }

        [ForeignKey("ContributionId")]
        public List<ContributionTransaction> Transactions { get; set; }
    }
}
