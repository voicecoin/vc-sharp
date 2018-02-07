using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Voicecoin.Core.Ico;

namespace Voicecoin.Core
{
    public class ContributionTransaction : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        public CurrencyType Currency { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal UsdPrice { get; set; }

        [StringLength(36)]
        public String CouponId { get; set; }

        [MaxLength(128)]
        public String Transaction { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal TokenAmount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal TokenUsdPrice { get; set; }

        public ContributionStatus Status { get; set; }
    }
}
