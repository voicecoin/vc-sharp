using Coinbase;
using Coinbase.Models;
using Coinbase.Wallet;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Ico;

namespace Voicecoin.Core
{
    public class ContributionCore
    {
        private Client coinbase;
        private Database dc;
        private String userId;

        public ContributionCore(String userId, Database dc, IConfiguration config)
        {
            coinbase = new Client(config.GetSection("Coinbase:Key").Value, config.GetSection("Coinbase:Secret").Value);
            this.dc = dc;
            this.userId = userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency">BTC</param>
        /// <returns></returns>
        public String GetAddress(String currency)
        {
            // check if address exist
            //var accounts = coinbase.GetAccounts();
            //var account = accounts.Data.FirstOrDefault(x => x.Currency.Code == currency.ToString());
            // For Debug
            //var addr = coinbase.CreateAddress(account.Id);
            /*var newAddress = new Transaction
            {
                FromAddress = "1Gt9VsPK3oQS6qmPLNNSamvRaQt9pMth94" //addr.Address
            };*/

            var wallet = dc.Table<WalletAddress>()
                .FirstOrDefault(x => x.UserId == userId && x.Currency == currency);

            return wallet?.Address;
        }
    }
}
