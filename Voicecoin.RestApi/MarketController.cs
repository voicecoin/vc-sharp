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
            /*result.Add(new
            {
                Name = "BTC",
                V2c = Math.Round(MarketCore.GetPricePair(IdConstants.TokenSymbol, "BTC", pairs).Amount, 8),
                C2v = Math.Round(MarketCore.GetPricePair("BTC", IdConstants.TokenSymbol, pairs).Amount, 8)
            });*/

            string tokenBaseCurrency = pairs.First(x => x.Base == IdConstants.TokenSymbol).Currency;

            if (tokenBaseCurrency == "USD")
            {
                result.Add(new
                {
                    Name = "ETH",
                    V2c = Math.Round(MarketCore.GetPricePair(IdConstants.TokenSymbol, "ETH", pairs).Amount, 8),
                    C2v = Math.Round(MarketCore.GetPricePair("ETH", IdConstants.TokenSymbol, pairs).Amount, 8)
                });
            }
            else if (tokenBaseCurrency == "ETH")
            {
                // var ico = new IcoCore(dc);
                // var stat = ico.GetIcoStat();

                var p = pairs.First(x => x.Base == IdConstants.TokenSymbol && x.Currency == "ETH");

                result.Add(new
                {
                    Name = "ETH",
                    V2c = Math.Round(p.Amount, 8),
                    C2v = Math.Round(1 / p.Amount, 8)
                });
            }


            return result;
        }
    }
}
