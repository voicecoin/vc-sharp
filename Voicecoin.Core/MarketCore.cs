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
            var ico = new IcoCore(dc).GetIcoStat();

            var coinbase = new Client("", "");

            var ethUsd = coinbase.GetBuyPrice("ETH-USD");
            var btcUsd = coinbase.GetBuyPrice("BTC-USD");
            //var ethVc = info.Amount > 0 ? Math.Round(1 / info.Amount, 2) : 0;

            var pricePairs = new List<PricePairModel>
            {
                // ETH-VC
                // new PricePairModel { Pair = $"{info.Symbol}-{info.Symbol}", Base = CurrencyType.ETH, Currency = CurrencyType.VC, Amount = ethVc },
                // BTC-VC
                // new PricePairModel { Pair = $"BTC-{info.Symbol}", Base = CurrencyType.BTC, Currency = CurrencyType.VC, Amount = btcUsd.Amount / ethUsd.Amount * ethVc},
                new PricePairModel { Pair = $"{ethUsd.Base}-{ethUsd.Currency}", Base = CurrencyType.ETH, Currency = CurrencyType.USD, Amount = ethUsd.Amount },
                new PricePairModel { Pair = $"{btcUsd.Base}-{ethUsd.Currency}", Base = CurrencyType.BTC, Currency = CurrencyType.USD, Amount = btcUsd.Amount }
            };

            // BASE UNIT - VC
            pricePairs.Add(new PricePairModel
            {
                Base = Enum.Parse<CurrencyType>(ico.Base),
                Currency = Enum.Parse<CurrencyType>(ico.Symbol),
                Amount = ico.Amount,
                Pair = $"{ico.Base}-{ico.Symbol}"
            });

            // VC - BASE UNIT
            pricePairs.Add(new PricePairModel
            {
                Base = Enum.Parse<CurrencyType>(ico.Symbol),
                Currency = Enum.Parse<CurrencyType>(ico.Base),
                Amount = 1 / ico.Amount,
                Pair = $"{ico.Symbol}-{ico.Base}"
            });

            var vcUsd = pricePairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.USD).Amount;

            // BTC - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.BTC,
                Currency = Enum.Parse<CurrencyType>(ico.Symbol),
                Amount = btcUsd.Amount / vcUsd,
                Pair = $"{CurrencyType.BTC.ToString()}-{ico.Symbol}"
            });

            // ETH - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.ETH,
                Currency = Enum.Parse<CurrencyType>(ico.Symbol),
                Amount = ethUsd.Amount / vcUsd,
                Pair = $"{CurrencyType.ETH.ToString()}-{ico.Symbol}"
            });

            return pricePairs;
        }
    }
}
