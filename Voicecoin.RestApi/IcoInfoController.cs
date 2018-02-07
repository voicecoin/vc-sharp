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
using Voicecoin.Core.Coupons;
using Voicecoin.Core.Ico;
using Voicecoin.Core.Models;

namespace Voicecoin.RestApi
{
    /// <summary>
    /// ICO releated information
    /// </summary>
    public class IcoInfoController : CoreController
    {
        /// <summary>
        /// ICO summary
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public Object GetInfo()
        {
            var ico = new IcoCore(dc).GetIcoStat();

            return new 
            {
                Sold = (ico.TotalSupply - ico.AvailableSupply).ToString("N0"),
                Available = ico.AvailableSupply.ToString("N0"),
                Total = ico.TotalSupply.ToString("N0"),
                Percent = (ico.TotalSupply - ico.AvailableSupply) / ico.TotalSupply * 100 + "%",
                StartDate = ico.StartDate
            };
        }

        /// <summary>
        /// Get supported currency types
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("currencies")]
        public List<Object> GetSupportedCurrencies()
        {
            return new List<Object>
            {
                new { Symbol = CurrencyType.BTC, Name = "Bitcoin" },
                new { Symbol = CurrencyType.ETH, Name = "Ethereum" }
            };
        }

        [HttpGet("ContributionStat")]
        public Object GetContributionStat()
        {
            string userId = GetCurrentUser().Id;
            var contribution = new ContributionCore(GetCurrentUser().Id, dc, Database.Configuration);
            var addresses = dc.Table<ContributionTransaction>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc, Database.Configuration).GetUsdPrices();

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
            var contribution = new ContributionCore(GetCurrentUser().Id, dc, Database.Configuration);
            var addresses = dc.Table<ContributionTransaction>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc, Database.Configuration).GetUsdPrices();

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

        [HttpPost("purchase/{currency}")]
        public ContributionTransaction Purchase([FromRoute] CurrencyType currency, [FromBody] VmContribution contribution)
        {
            // check wether coupon applied
            var couponCore = new CouponCore(dc, Database.Configuration);
            var coupon = couponCore.GetCouponByCode(contribution.CouponCode);

            var contributionCore = new ContributionCore(GetCurrentUser().Id, dc, Database.Configuration);
            String address = contributionCore.GetAddress(currency);

            var marketCore = new MarketCore(dc, Database.Configuration);
            var pairs = marketCore.GetUsdPrices();
            pairs = marketCore.ApplyCoupon(pairs, contribution.CouponCode);
            var pricePair = MarketCore.GetPricePair(CurrencyType.VC, contribution.Currency, pairs);

            var transaction = new ContributionTransaction
            {
                Address = address,
                TokenAmount = contribution.TokenAmount,
                Amount = contribution.TokenAmount * pricePair.Amount,
                UsdPrice = pairs.Find(x => x.Base == contribution.Currency && x.Currency == CurrencyType.USD).Amount,
                CouponId = coupon?.Id,
                Currency = contribution.Currency,
                UserId = GetCurrentUser().Id,
                TokenUsdPrice = pairs.Find(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.USD).Amount,
                Status = ContributionStatus.Unfullfilled
            };

            dc.DbTran(() => dc.Table<ContributionTransaction>().Add(transaction));

            return transaction;
        }

        [HttpGet("purchases")]
        public IEnumerable<ContributionTransaction> Purchases()
        {
            return dc.Table<ContributionTransaction>()
                .Where(x => x.UserId == GetCurrentUser().Id)
                .OrderByDescending(x => x.UpdatedTime)
                .Take(100);
        }
    }
}
