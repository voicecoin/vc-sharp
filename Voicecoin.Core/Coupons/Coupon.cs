using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class Coupon : DbRecord, IDbRecord
    {
        public String Code { get; set; }

        public String Description { get; set; }

        public Decimal PercentageOff { get; set; }

        public Decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        public BalanceModel Price
        {
            get
            {
                return new BalanceModel { Amount = Amount, Currency = Currency };
            }
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
