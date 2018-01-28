using Coinbase;
using Coinbase.Models;
using Coinbase.Wallet;
using EntityFrameworkCore.BootKit;
using Etherscan.NetSDK;
using Info.Blockchain.API.BlockExplorer;
using Info.Blockchain.API.Client;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core.Tables;

namespace Voicecoin.Core
{
    public class AccountReceivableStatusJob : ScheduleJobBase, IScheduleJob
    {
        public override Task Execute(IJobExecutionContext context)
        {
            UpdateContributionAmount4BTC();
            UpdateContributionAmount4ETH();

            return Task.CompletedTask;
        }

        private void UpdateContributionAmount4ETH()
        {
            var addresses = dc.Table<ContributorCurrencyAddress>()
                .Where(x => x.Currency == CurrencyType.ETH)
                .OrderByDescending(x => x.UpdatedTime)
                .Select(x => x.Address)
                .Take(10)
                .ToArray();

            var etherscan = new EtherscanClient(Database.Configuration.GetSection("Etherscan:ApiKey").Value);
            var transactions = etherscan.GetTransactions(addresses);

            dc.DbTran(() => {

                var conAddrs = dc.Table<ContributorCurrencyAddress>().Where(x => addresses.Contains(x.Address)).ToList();

                conAddrs.ForEach(conAddr => {
                    conAddr.Amount = transactions.Where(x => x.To.ToLower() == conAddr.Address.ToLower()).Sum(x => x.Amount);
                    conAddr.UpdatedTime = DateTime.UtcNow;
                });
                
            });
        }

        private void UpdateContributionAmount4BTC()
        {
            var addresses = dc.Table<ContributorCurrencyAddress>()
                .Where(x => x.Currency == CurrencyType.BTC)
                .OrderByDescending(x => x.UpdatedTime)
                .Select(x => x.Address)
                .Take(10)
                .ToList();

            dc.DbTran(() => {

                addresses.ForEach(conAddr => {
                    var addr = dc.Table<ContributorCurrencyAddress>().First(x => x.Address == conAddr);
                    var received = BitcoinHelper.GetReceivedValueByAddress(conAddr);
                    addr.Amount = received.Amount;
                    addr.UpdatedTime = DateTime.UtcNow;
                });

            });
        }
    }
}