using Coinbase;
using Coinbase.Models;
using Coinbase.Wallet;
using EntityFrameworkCore.BootKit;
using Etherscan.NetSDK;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core.Coupons;

namespace Voicecoin.Core
{
    public class ContributionTransactionStatusJob : ScheduleJobBase, IScheduleJob
    {
        public override Task Execute(IJobExecutionContext context)
        {
            var marketCore = new MarketCore(dc, Database.Configuration);
            var pairs = marketCore.GetUsdPrices();

            UpdateContributionAmount4ETH(marketCore, pairs);
            // UpdateContributionAmount4BTC(pairs);

            // Update available tokens
            dc.DbTran(() => {
                var currency = dc.Table<Cryptocurrency>().First(x => x.Symbol == CurrencyType.VC);
                decimal soldTokenAmount = dc.Table<IcoContribution>().Sum(x => x.Token);
                currency.AvailableSupply = currency.TotalSupply - soldTokenAmount;
                currency.UpdatedTime = DateTime.UtcNow;
            });

            return Task.CompletedTask;
        }

        private void UpdateContributionAmount4ETH(MarketCore marketCore, List<PricePairModel> pricePairs)
        {
            var addresses = dc.Table<IcoContribution>()
                .Where(x => x.Currency == CurrencyType.ETH)
                .OrderByDescending(x => x.UpdatedTime)
                .Select(x => x.Address)
                .Take(10)
                .ToArray();

            // Sample https://etherscan.io/tx/0x3cbe0ae1cdd73bc674557514110e3d6f57598a371629b29fe4b2cbf26028ed4e
            var etherscan = new EtherscanClient(Database.Configuration.GetSection("Etherscan:ApiKey").Value);
            var transactions = etherscan.GetTransactions(addresses);

            var couponCore = new CouponCore(dc, Database.Configuration);

            dc.DbTran(() => {
                var conAddrs = dc.Table<IcoContribution>().Where(x => addresses.Contains(x.Address)).ToList();

                // Loop every receive address
                conAddrs.ForEach(conAddr => {

                    // check coupon
                    var coupon = couponCore.GetLastedCouponByUser(conAddr.UserId);
                    if(coupon != null)
                    {
                        pricePairs = marketCore.ApplyCoupon(pricePairs, coupon.Code);
                    }

                    // loop every transaction
                    transactions.Where(x => x.To.ToLower() == conAddr.Address.ToLower())
                        .ToList()
                        .ForEach(tx =>
                        {
                            AddTransaction(conAddr.Id, tx.Amount, tx.Hash, CurrencyType.ETH, pricePairs);
                        });

                    dc.SaveChanges();

                    var coin2Token = MarketCore.GetPricePair(CurrencyType.ETH, CurrencyType.VC, pricePairs);
                    conAddr.Amount = dc.Table<ContributionTransaction>().Where(x => x.ContributionId == conAddr.Id).Sum(x => x.Amount);
                    conAddr.Token = conAddr.Amount * coin2Token.Amount;
                    conAddr.UpdatedTime = DateTime.UtcNow;
                });
                
            });
        }

        private void UpdateContributionAmount4BTC(MarketCore marketCore, List<PricePairModel> pricePairs)
        {
            var coin2Token = pricePairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.VC);
            var coin2Usd = pricePairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.USD);
            var token2Usd = pricePairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.USD);

            var addresses = dc.Table<IcoContribution>()
                .Where(x => x.Currency == CurrencyType.BTC)
                .OrderByDescending(x => x.UpdatedTime)
                .Select(x => x.Address)
                .Take(10)
                .ToList();

            dc.DbTran(() => {
                var currency = dc.Table<Cryptocurrency>().First(x => x.Symbol == CurrencyType.VC);

                addresses.ForEach(conAddr => {
                    var addr = dc.Table<IcoContribution>().First(x => x.Address == conAddr);
                    var received = BitcoinHelper.GetReceivedValueByAddress(conAddr);
                    addr.Amount = received.Amount;
                    addr.Token = received.Amount * coin2Token.Amount;
                    currency.AvailableSupply = currency.AvailableSupply - addr.Token;
                    addr.UpdatedTime = DateTime.UtcNow;
                });

            });
        }

        private void AddTransaction(string contributionId, decimal amount, string transaction, CurrencyType currency, List<PricePairModel> pricePairs)
        {
            var coin2Token = MarketCore.GetPricePair(currency, CurrencyType.VC, pricePairs);
            var coin2Usd = pricePairs.First(x => x.Base == currency && x.Currency == CurrencyType.USD);
            var token2Usd = pricePairs.First(x => x.Base == CurrencyType.VC && x.Currency == CurrencyType.USD);

            if (!dc.Table<ContributionTransaction>().Any(x => x.Transaction == transaction))
            {
                dc.Table<ContributionTransaction>().Add(new ContributionTransaction
                {
                    Amount = amount,
                    TokenAmount = amount * coin2Token.Amount,
                    TokenUsdPrice = token2Usd.Amount,
                    ContributionId = contributionId,
                    Transaction = transaction,
                    UsdPrice = coin2Usd.Amount
                });
            }
        }
    }
}