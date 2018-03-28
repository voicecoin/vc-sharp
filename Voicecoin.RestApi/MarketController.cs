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
            var prices = dc.Table<TokenPrice>().Where(x => x.StartDate >= DateTime.UtcNow)
                .ToList();

            var pairs = new List<PricePairModel>();

            prices.ForEach(price => {

                pairs.Add(new PricePairModel
                {
                    Pair = $"{price.Currency}-{price.Symbol}",
                    Base = price.Currency,
                    Currency = price.Symbol,
                    Amount = Math.Round(price.Amount, 8)
                });

                pairs.Add(new PricePairModel
                {
                    Pair = $"{price.Symbol}-{price.Currency}",
                    Base = price.Symbol,
                    Currency = price.Currency,
                    Amount = Math.Round(1 / price.Amount, 8)
                });
            });

            var result = new List<Object>();

            prices.ForEach(price => {
                result.Add(new
                {
                    Name = price.Currency,
                    V2c = pairs.First(x => x.Base == IdConstants.TokenSymbol && x.Currency == price.Currency).Amount,
                    C2v = pairs.First(x => x.Base == price.Currency && x.Currency == IdConstants.TokenSymbol).Amount
                });
            });

            return result;
        }
    }
}
