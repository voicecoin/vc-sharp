using Coinbase.Models;
using EntityFrameworkCore.BootKit;
using IP2Region;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Voicecoin.Core;
using Voicecoin.Core.Ico;

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

            DbSearcher dbSearcher = new DbSearcher(new DbConfig
            {

            }, $"{Database.ContentRootPath}{Path.DirectorySeparatorChar}App_Data{Path.DirectorySeparatorChar}ip2region.db");
            var ipRegion = dbSearcher.BtreeSearch(Request.Host.Host);

            return new 
            {
                Sold = (ico.TotalSupply - ico.AvailableSupply).ToString("N0"),
                Available = ico.AvailableSupply.ToString("N0"),
                Total = ico.TotalSupply.ToString("N0"),
                Percent = (ico.TotalSupply - ico.AvailableSupply) / ico.TotalSupply * 100 + "%",
                StartDate = ico.StartDate,
                IpRegion = ipRegion.GetRegion()
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
                new { Symbol = "BTC", Name = "Bitcoin" },
                new { Symbol = "ETH", Name = "Ethereum" }
            };
        }

        [HttpGet("ContributionStat")]
        public Object GetContributionStat()
        {
            string userId = CurrentUserId;
            var contribution = new ContributionCore(CurrentUserId, dc, Database.Configuration);
            var addresses = dc.Table<TokenWalletAddress>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc, Database.Configuration).GetUsdPrices();

            var btcAmount = 0; // addresses.FirstOrDefault(x => x.Currency == "BTC")?.Amount;
            var ethAmount = 0; // addresses.FirstOrDefault(x => x.Currency == "ETH")?.Amount;

            var btcAmountUsd = btcAmount * pairs.First(x => x.Base == "BTC" && x.Currency == "USD").Amount;
            var ethAmountUsd = ethAmount * pairs.First(x => x.Base == "ETH" && x.Currency == "USD").Amount;

            return new
            {
                Items = new List<Object>
                {
                    new
                    {
                        Currency = "BTC",
                        Amount = btcAmount,
                        AmountUsd = btcAmountUsd
                    },

                    new
                    {
                        Currency = "ETH",
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
            string userId = CurrentUserId;
            var contribution = new ContributionCore(CurrentUserId, dc, Database.Configuration);
            var addresses = dc.Table<TokenWalletAddress>().Where(x => x.UserId == userId).ToList();

            var pairs = new MarketCore(dc, Database.Configuration).GetUsdPrices();

            return new List<Object>
            {
                new
                {
                    Currency = "BTC",
                    addresses.FirstOrDefault(x => x.Currency == "BTC")?.Address,
                    Rate = pairs.First(x => x.Base == "BTC").Amount
                },
                new
                {
                    Currency = "ETH",
                    addresses.FirstOrDefault(x => x.Currency == "ETH")?.Address,
                    Rate = pairs.First(x => x.Base == "ETH").Amount
                }
            };
        }

        [HttpPost("purchase/{currency}")]
        public Transaction Purchase([FromRoute] String currency, [FromBody] VmContribution contribution)
        {
            // save user's wallet address
            if (!dc.Table<UserWalletAddress>().Any(x => x.UserId == CurrentUserId && 
                x.Currency == contribution.Currency &&
                x.Address == contribution.FromAddress))
            {
                dc.DbTran(() =>
                {
                    dc.Table<UserWalletAddress>().Add(new UserWalletAddress
                    {
                        UserId = CurrentUserId,
                        Currency = contribution.Currency,
                        Address = contribution.FromAddress
                    });
                });
            };

            // check wether coupon applied
            // var couponCore = new CouponCore(dc, Database.Configuration);
            // var coupon = couponCore.GetCouponByCode(contribution.CouponCode);

            //var contributionCore = new ContributionCore(CurrentUserId, dc, Database.Configuration);
            //String address = contributionCore.GetAddress(currency);

            //var marketCore = new MarketCore(dc, Database.Configuration);
            //var pairs = marketCore.GetUsdPrices();
            // pairs = marketCore.ApplyCoupon(pairs, contribution.CouponCode);
            //var pricePair = MarketCore.GetPricePair(IdConstants.TokenSymbol, contribution.Currency, pairs);

            string address = dc.Table<TokenWalletAddress>()
                .FirstOrDefault(x => x.Currency == contribution.Currency)
                .Address;

            var transaction = new Transaction
            {
                ToAddress = address,
                TokenAmount = contribution.TokenAmount,
                //Amount = contribution.TokenAmount * pricePair.Amount,
                // CouponId = coupon?.Id,
                Status = ContributionStatus.Unfullfilled
            };

            // dc.DbTran(() => dc.Table<Transaction>().Add(transaction));

            return transaction;
        }

        [HttpGet("purchases")]
        public IEnumerable<Transaction> Purchases()
        {
            var query = from wa in dc.Table<TokenWalletAddress>()
                        join t in dc.Table<Transaction>() on wa.Address equals t.ToAddress
                        select t;

            return query.ToList();
        }
    }
}
