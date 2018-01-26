using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;
using Voicecoin.Core.Tables;
using System.Linq;
using Voicecoin.Core.Models;
using Coinbase.Models;

namespace Voicecoin.Core
{
    public class IcoCore
    {
        private Database dc;

        public IcoCore(Database db)
        {
            dc = db;
        }

        public IcoStatModel GetIcoStat()
        {
            var ico = (from currency in dc.Table<Cryptocurrency>()
                       join stage in dc.Table<PriceStage>() on new { currency.Symbol, currency.Status } equals new { stage.Symbol, stage.Status }
                       select new
                       {
                           currency.TotalSupply,
                           currency.AvailableSupply,
                           stage.StartDate,
                           stage.Base,
                           stage.Amount,
                           stage.Symbol
                       }).First();

            var pricePairs = new MarketCore(dc).GetPrices();

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
            var btcUsd = pricePairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.USD).Amount;
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.BTC,
                Currency = Enum.Parse<CurrencyType>(ico.Symbol),
                Amount = btcUsd / vcUsd,
                Pair = $"{CurrencyType.BTC.ToString()}-{ico.Symbol}"
            });

            // ETH - VC
            var ethUsd = pricePairs.First(x => x.Base == CurrencyType.ETH && x.Currency == CurrencyType.USD).Amount;
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.ETH,
                Currency = Enum.Parse<CurrencyType>(ico.Symbol),
                Amount = ethUsd / vcUsd,
                Pair = $"{CurrencyType.ETH.ToString()}-{ico.Symbol}"
            });

            return new IcoStatModel
            {
                Sold = (ico.TotalSupply - ico.AvailableSupply).ToString("N0"),
                Available = ico.AvailableSupply.ToString("N0"),
                Total = ico.TotalSupply.ToString("N0"),
                Percent = (ico.TotalSupply - ico.AvailableSupply) / ico.TotalSupply * 100 + "%",
                StartDate = ico.StartDate,
                Prices = pricePairs
            };
        }
    }
}
