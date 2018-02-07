using Coinbase;
using Coinbase.Models;
using Coinbase.Wallet;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public String GetAddress(CurrencyType currency)
        {
            // check if address exist
            //var accounts = coinbase.GetAccounts();
            //var account = accounts.Data.FirstOrDefault(x => x.Currency.Code == currency.ToString());
            // For Debug
            //var addr = coinbase.CreateAddress(account.Id);
            var newAddress = new ContributionTransaction
            {
                UserId = userId,
                Currency = currency,
                Address = "1Gt9VsPK3oQS6qmPLNNSamvRaQt9pMth94" //addr.Address
            };

            return newAddress.Address;
        }
    }
}
