using Coinbase;
using Coinbase.Currecny;
using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            ethUsd.Pair = "ETH-USD";

            var btcUsd = coinbase.GetBuyPrice("BTC-USD");
            btcUsd.Pair = "BTC-USD";

            var vcUsd = new PricePairModel
            {
                Base = ico.Symbol,
                Amount = ico.Price.Amount,
                Currency = ico.Price.Currency,
                Pair = $"{ico.Symbol}-{ico.Price.Currency}"
            };

            var pricePairs = new List<PricePairModel>();

            // VC - BASE UNIT
            pricePairs.Add(new PricePairModel
            {
                Base = ico.Symbol,
                Currency = ico.Price.Currency,
                Amount = ico.Price.Amount,
                Pair = $"{ico.Symbol}-{ico.Price.Currency}"
            });

            // BASE UNIT - VC
            pricePairs.Add(new PricePairModel
            {
                Base = ico.Price.Currency,
                Currency = ico.Symbol,
                Amount = 1 / ico.Price.Amount,
                Pair = $"{ico.Price.Currency}-{ico.Symbol}"
            });

            // VC - BTC
            pricePairs.Add(new PricePairModel
            {
                Base = ico.Symbol,
                Currency = CurrencyType.BTC,
                Amount = vcUsd.Amount / btcUsd.Amount,
                Pair = $"{ico.Symbol}-{CurrencyType.BTC.ToString()}"
            });

            // BTC - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.BTC,
                Currency = ico.Symbol,
                Amount = btcUsd.Amount / vcUsd.Amount,
                Pair = $"{CurrencyType.BTC.ToString()}-{ico.Symbol}"
            });

            // VC - ETH
            pricePairs.Add(new PricePairModel
            {
                Base = ico.Symbol,
                Currency = CurrencyType.ETH,
                Amount = vcUsd.Amount / ethUsd.Amount,
                Pair = $"{ico.Symbol}-{CurrencyType.ETH.ToString()}"
            });

            // ETH - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.ETH,
                Currency = ico.Symbol,
                Amount = ethUsd.Amount / vcUsd.Amount,
                Pair = $"{CurrencyType.ETH.ToString()}-{ico.Symbol}"
            });

            return pricePairs;
        }
    }
}
