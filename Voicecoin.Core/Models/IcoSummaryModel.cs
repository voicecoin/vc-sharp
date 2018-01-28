using Coinbase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Models
{
    /// <summary>
    /// ICO summary
    /// </summary>
    public class IcoSummaryModel
    {
        /// <summary>
        /// Total Supply tokens
        /// </summary>
        public decimal TotalSupply { get; set; }

        /// <summary>
        /// Total Available tokens
        /// </summary>
        public decimal AvailableSupply { get; set; }

        public String Symbol { get; set; }

        public BalanceModel Price { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Base currency
        /// </summary>
        public String Base { get; set; }

        /// <summary>
        /// Sold
        /// </summary>
        public decimal SoldTokens { get; set; }

        /// <summary>
        /// Sold percent
        /// </summary>
        public decimal Percent { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}
