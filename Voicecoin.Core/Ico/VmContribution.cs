using Coinbase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core
{
    public class VmContribution
    {
        public string Currency { get; set; }

        public decimal TokenAmount { get; set; }

        public string CouponCode { get; set; }

        public string FromAddress { get; set; }
    }
}
