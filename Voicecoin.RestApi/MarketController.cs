using Coinbase;
using Coinbase.Currecny;
using Coinbase.Models;
using ContentFoundation.RestApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core;
using Voicecoin.Core.Tables;

namespace Voicecoin.RestApi
{
    public class MarketController : CoreController
    {
        [AllowAnonymous]
        [HttpGet("prices")]
        public List<Object> GetPricePairs()
        {
            var pairs = new MarketCore(dc).GetPrices();

            var result = new List<Object>();
            result.Add(new
            {
                Name = CurrencyType.BTC,
                V2c = pairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.BTC).Amount,
                C2v = pairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.VC).Amount
            });

            result.Add(new
            {
                Name = CurrencyType.ETH,
                V2c = pairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.ETH).Amount,
                C2v = pairs.First(x => x.Base == CurrencyType.ETH && x.Currency == CurrencyType.VC).Amount
            });

            return result;
        }
    }
}
