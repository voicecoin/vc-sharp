using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Tables
{
    /// <summary>
    /// Receive money address
    /// </summary>
    public class ContributorAddress : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Total Amount
        /// </summary>
        public Decimal Amount { get; set; }
    }
}
