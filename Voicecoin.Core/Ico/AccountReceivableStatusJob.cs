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

namespace Voicecoin.Core
{
    public class AccountReceivableStatusJob : ScheduleJobBase, IScheduleJob
    {
        public override Task Execute(IJobExecutionContext context)
        {
            var pairs = new MarketCore(dc).GetPrices();

            UpdateContributionAmount4BTC(pairs.First(x => x.Base == CurrencyType.BTC && x.Currency == CurrencyType.VC));
            UpdateContributionAmount4ETH(pairs.First(x => x.Base == CurrencyType.ETH && x.Currency == CurrencyType.VC));

            return Task.CompletedTask;
        }

        private void UpdateContributionAmount4ETH(PricePairModel pricePair)
        {
            var addresses = dc.Table<IcoContribution>()
                .Where(x => x.Currency == CurrencyType.ETH)
                .OrderByDescending(x => x.UpdatedTime)
                .Select(x => x.Address)
                .Take(10)
                .ToArray();

            var etherscan = new EtherscanClient(Database.Configuration.GetSection("Etherscan:ApiKey").Value);
            var transactions = etherscan.GetTransactions(addresses);

            dc.DbTran(() => {

                var currency = dc.Table<Cryptocurrency>().First(x => x.Symbol == CurrencyType.VC);
                var conAddrs = dc.Table<IcoContribution>().Where(x => addresses.Contains(x.Address)).ToList();

                conAddrs.ForEach(conAddr => {
                    conAddr.Amount = transactions.Where(x => x.To.ToLower() == conAddr.Address.ToLower()).Sum(x => x.Amount);
                    conAddr.Token = conAddr.Amount * pricePair.Amount;
                    currency.AvailableSupply = currency.AvailableSupply - conAddr.Token;
                    conAddr.UpdatedTime = DateTime.UtcNow;
                });
                
            });
        }

        private void UpdateContributionAmount4BTC(PricePairModel pricePair)
        {
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
                    addr.Token = received.Amount * pricePair.Amount;
                    currency.AvailableSupply = currency.AvailableSupply - addr.Token;
                    addr.UpdatedTime = DateTime.UtcNow;
                });

            });
        }
    }
}