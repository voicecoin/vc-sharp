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
    public class Transaction : DbRecord, IDbRecord
    {
        [MaxLength(128)]
        public String FromAddress { get; set; }

        [MaxLength(128)]
        public String ToAddress { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [StringLength(36)]
        public String CouponId { get; set; }

        [MaxLength(128)]
        public String TransactionHash { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal TokenAmount { get; set; }

        public ContributionStatus Status { get; set; }

        public String Error { get; set; }
    }
}
