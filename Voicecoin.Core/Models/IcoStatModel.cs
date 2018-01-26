using Coinbase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Models
{
    public class IcoStatModel
    {
        public String Total { get; set; }

        public String Available { get; set; }

        public String Sold { get; set; }

        public String Percent { get; set; }

        public DateTime StartDate { get; set; }

        public List<PricePairModel> Prices { get; set; }
    }
}
