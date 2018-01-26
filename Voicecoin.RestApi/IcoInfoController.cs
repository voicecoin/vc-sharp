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
using Voicecoin.Core.Models;
using Voicecoin.Core.Tables;

namespace Voicecoin.RestApi
{
    [Route("v1/[controller]")]
    public class IcoInfoController : CoreController
    {
        [AllowAnonymous]
        [HttpGet]
        public IcoStatModel GetInfo()
        {
            return new IcoCore(dc).GetIcoStat();
        }

        [HttpGet("ContributionStat")]
        public Object GetContributionStat()
        {
            string userId = GetCurrentUser().Id;
            var contribution = new Contribution(GetCurrentUser().Id, dc, Database.Configuration);
            var addresses = dc.Table<ContributorAddress>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc).GetPrices();

            var btcAmount = addresses.FirstOrDefault(x => x.Currency == CurrencyType.BTC)?.Amount;
            var ethAmount = addresses.FirstOrDefault(x => x.Currency == CurrencyType.ETH)?.Amount;

            var btcAmountUsd = btcAmount * pairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.USD).Amount;
            var ethAmountUsd = ethAmount * pairs.First(x => x.Base == CurrencyType.ETH && x.Currency == CurrencyType.USD).Amount;

            return new
            {
                Items = new List<Object>
                {
                    new
                    {
                        Currency = CurrencyType.BTC,
                        Amount = btcAmount,
                        AmountUsd = btcAmountUsd
                    },

                    new
                    {
                        Currency = CurrencyType.ETH,
                        Amount = ethAmount,
                        AmountUsd = ethAmountUsd
                    }
                },

                Total = btcAmountUsd + ethAmountUsd
            };
        }

        [HttpGet("addresses")]
        public Object GetPayableAddresses()
        {
            string userId = GetCurrentUser().Id;
            var contribution = new Contribution(GetCurrentUser().Id, dc, Database.Configuration);
            var addresses = dc.Table<ContributorAddress>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc).GetPrices();

            return new List<Object>
            {
                new
                {
                    Currency = CurrencyType.BTC.ToString(),
                    addresses.FirstOrDefault(x => x.Currency == CurrencyType.BTC)?.Address,
                    Rate = pairs.First(x => x.Base == CurrencyType.BTC).Amount
                },
                new
                {
                    Currency = CurrencyType.ETH.ToString(),
                    addresses.FirstOrDefault(x => x.Currency == CurrencyType.ETH)?.Address,
                    Rate = pairs.First(x => x.Base == CurrencyType.ETH).Amount
                }
            };
        }

        [HttpGet("address/{currency}")]
        public String GetReceiveAddress([FromRoute] CurrencyType currency)
        {
            String address = String.Empty;

            dc.DbTran(() => {
                var contribution = new Contribution(GetCurrentUser().Id, dc, Database.Configuration);
                address = contribution.GetAddress(currency, true);
            });


            return address;
        }
    }
}
