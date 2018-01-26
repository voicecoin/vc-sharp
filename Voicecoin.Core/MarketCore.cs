using Coinbase;
using Coinbase.Currecny;
using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Tables;

namespace Voicecoin.Core
{
    public class MarketCore
    {
        private Database dc;

        public MarketCore(Database db)
        {
            dc = db;
        }

        public List<PricePairModel> GetPrices()
        {
            // var info = dc.Table<PriceStage>().First();

            var coinbase = new Client("", "");

            var ethUsd = coinbase.GetBuyPrice("ETH-USD");
            var btcUsd = coinbase.GetBuyPrice("BTC-USD");
            //var ethVc = info.Amount > 0 ? Math.Round(1 / info.Amount, 2) : 0;

            return new List<PricePairModel>
            {
                // ETH-VC
                // new PricePairModel { Pair = $"{info.Symbol}-{info.Symbol}", Base = CurrencyType.ETH, Currency = CurrencyType.VC, Amount = ethVc },
                // BTC-VC
                // new PricePairModel { Pair = $"BTC-{info.Symbol}", Base = CurrencyType.BTC, Currency = CurrencyType.VC, Amount = btcUsd.Amount / ethUsd.Amount * ethVc},
                new PricePairModel { Pair = $"{ethUsd.Base}-{ethUsd.Currency}", Base = CurrencyType.ETH, Currency = CurrencyType.USD, Amount = ethUsd.Amount },
                new PricePairModel { Pair = $"{btcUsd.Base}-{ethUsd.Currency}", Base = CurrencyType.BTC, Currency = CurrencyType.USD, Amount = btcUsd.Amount }
            };
        }
    }
}
