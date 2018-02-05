using Coinbase;
using Coinbase.Currecny;
using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Coupons;

namespace Voicecoin.Core
{
    public class MarketCore
    {
        private Database dc;
        private IConfiguration config;

        public MarketCore(Database db, IConfiguration config)
        {
            dc = db;
            this.config = config;
        }

        public List<PricePairModel> GetUsdPrices()
        {
            var pricePairs = new List<PricePairModel>();

            var coinbase = new Client("", "");

            var ethUsd = coinbase.GetBuyPrice("ETH-USD");
            ethUsd.Pair = "ETH-USD";

            var btcUsd = coinbase.GetBuyPrice("BTC-USD");
            btcUsd.Pair = "BTC-USD";

            // BTC - USD
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.BTC,
                Currency = CurrencyType.USD,
                Amount = btcUsd.Amount,
                Pair = $"{CurrencyType.BTC}-{CurrencyType.USD}"
            });

            // ETH - USD
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.ETH,
                Currency = CurrencyType.USD,
                Amount = ethUsd.Amount,
                Pair = $"{CurrencyType.ETH}-{CurrencyType.USD}"
            });


            // VC - USD
            var ico = new IcoCore(dc).GetIcoStat();
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.VC,
                Amount = ico.Price.Amount,
                Currency = ico.Price.Currency,
                Pair = $"{CurrencyType.VC}-{ico.Price.Currency}"
            });

            return pricePairs;
        }

        public List<PricePairModel> ApplyCoupon(List<PricePairModel> pricePairs, string couponCode)
        {
            var couponCore = new CouponCore(dc, config);
            var coupon = couponCore.GetCouponByCode(couponCode);
            if(coupon != null)
            {
                pricePairs.First(x => x.Base == CurrencyType.VC).Amount = coupon.Amount;
            }

            return pricePairs;
        }

        public static PricePairModel GetPricePair(CurrencyType baseCurrency, CurrencyType targetCurrency, List<PricePairModel> usdPrices)
        {
            var bc = usdPrices.First(x => x.Base == baseCurrency);
            var tc = usdPrices.First(x => x.Base == targetCurrency);

            return new PricePairModel
            {
                Base = baseCurrency,
                Currency = targetCurrency,
                Amount = bc.Amount / tc.Amount,
                Pair = $"{baseCurrency}-{targetCurrency}"
            };
        }

        /*
        public void MapMoreCurrencies(List<PricePairModel> pricePairs)
        {
            var btcUsd = pricePairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.USD);
            var ethUsd = pricePairs.First(x => x.Base == CurrencyType.ETH && x.Currency == CurrencyType.USD);
            var vcUsd = pricePairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.USD);

            // VC - BTC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.VC,
                Currency = CurrencyType.BTC,
                Amount = vcUsd.Amount / btcUsd.Amount,
                Pair = $"{CurrencyType.VC}-{CurrencyType.BTC}"
            });

            // BTC - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.BTC,
                Currency = CurrencyType.VC,
                Amount = btcUsd.Amount / vcUsd.Amount,
                Pair = $"{CurrencyType.BTC}-{CurrencyType.VC}"
            });

            // VC - ETH
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.VC,
                Currency = CurrencyType.ETH,
                Amount = vcUsd.Amount / ethUsd.Amount,
                Pair = $"{CurrencyType.VC}-{CurrencyType.ETH}"
            });

            // ETH - VC
            pricePairs.Add(new PricePairModel
            {
                Base = CurrencyType.ETH,
                Currency = CurrencyType.VC,
                Amount = ethUsd.Amount / vcUsd.Amount,
                Pair = $"{CurrencyType.ETH}-{CurrencyType.VC}"
            });

            // approximately
            pricePairs.ForEach(x => {
                x.Amount = Math.Round(x.Amount, 8);
            });
        }*/
    }
}
