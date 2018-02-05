using Coinbase;
using Coinbase.Currecny;
using Coinbase.Models;
using ContentFoundation.RestApi;
using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core;

namespace Voicecoin.RestApi
{
    public class MarketController : CoreController
    {
        [AllowAnonymous]
        [HttpGet("prices")]
        public List<Object> GetPricePairs([FromQuery] string coupon)
        {
            var marketCore = new MarketCore(dc, Database.Configuration);
            var pairs = marketCore.GetUsdPrices();

            if (!String.IsNullOrEmpty(coupon))
            {
                pairs = marketCore.ApplyCoupon(pairs, coupon);
            }

            var result = new List<Object>();
            result.Add(new
            {
                Name = CurrencyType.BTC,
                V2c = Math.Round(MarketCore.GetPricePair(CurrencyType.VC, CurrencyType.BTC, pairs).Amount, 8),
                C2v = Math.Round(MarketCore.GetPricePair(CurrencyType.BTC, CurrencyType.VC, pairs).Amount, 8)
            });

            result.Add(new
            {
                Name = CurrencyType.ETH,
                V2c = Math.Round(MarketCore.GetPricePair(CurrencyType.VC, CurrencyType.ETH, pairs).Amount, 8),
                C2v = Math.Round(MarketCore.GetPricePair(CurrencyType.ETH, CurrencyType.VC, pairs).Amount, 8)
            });

            return result;
        }
    }
}
