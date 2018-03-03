using Coinbase.Models;
using Info.Blockchain.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core
{
    public class BitcoinHelper
    {
        public static BalanceModel GetReceivedValueByAddress(String address)
        {
            var client = new RestClient("https://blockchain.info");

            var request = new RestRequest("rawaddr/{address}", Method.GET);
            request.AddUrlSegment("address", address);

            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<JObject>(response.Content);

            return new BalanceModel { Amount = result["total_received"].ToObject<Decimal>() / 100000000, Currency = "BTC" };
        }
    }}
