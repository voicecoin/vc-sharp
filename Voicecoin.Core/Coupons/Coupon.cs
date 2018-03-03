using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class Coupon : DbRecord, IDbRecord
    {
        [StringLength(6)]
        public String Code { get; set; }

        [MaxLength(128)]
        public String Description { get; set; }

        public Decimal PercentageOff { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public Decimal Amount { get; set; }

        public String Currency { get; set; }

        public BalanceModel Price
        {
            get
            {
                return new BalanceModel { Amount = Amount, Currency = Currency };
            }
        }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
