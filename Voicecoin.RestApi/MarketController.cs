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
    [Route("v1/[controller]")]
    public class MarketController : CoreController
    {
        [AllowAnonymous]
        [HttpGet("prices")]
        public List<PricePairModel> GetPricePairs()
        {
            return new MarketCore(dc).GetPrices();
        }
    }
}
