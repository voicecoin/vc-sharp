using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.BootKit;
using Voicecoin.Core.Tables;

namespace Voicecoin.Core
{
    public class HookDbInitializer : IHookDbInitializer
    {
        public int Priority => 1000;

        public void Load(IConfiguration config, Database dc)
        {
            if (dc.Table<Cryptocurrency>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Voicecoin.Cryptocurrency.json");
            var cryptocurrency = JsonConvert.DeserializeObject<Cryptocurrency>(json);
            dc.Table<Cryptocurrency>().Add(cryptocurrency);

            if (dc.Table<PriceStage>().Any()) return;

            json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Voicecoin.PriceStage.json");
            var icoinfos = JsonConvert.DeserializeObject<List<PriceStage>>(json);
            dc.Table<PriceStage>().AddRange(icoinfos);

            /*json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Voicecoin.Coupon.json");
            var coupnons = JsonConvert.DeserializeObject<List<Coupon>>(json);
            dc.Table<Coupon>().AddRange(coupnons);*/

            InitCommonDataCountry(config, dc);
            InitCommonDataUsStates(config, dc);
        }

        /// <summary>
        /// https://github.com/OpenBookPrices/country-data/tree/master/data
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dc"></param>
        private void InitCommonDataCountry(IConfiguration config, Database dc)
        {
            if (dc.Table<Country>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Common.Countries.json");
            var countries = JsonConvert.DeserializeObject<List<JObject>>(json);

            countries.ForEach(country => {
                dc.Table<Country>().Add(new Country
                {
                    Name = country["name"].ToString(),
                    Code2 = country["alpha_2_code"].ToString(),
                    Code3 = country["alpha_3_code"].ToString(),
                    Nationality = country["nationality"].ToString()
                });
            });
        }

        private void InitCommonDataUsStates(IConfiguration config, Database dc)
        {
            if (dc.Table<State>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Common.States-US.json");
            var states = JsonConvert.DeserializeObject<List<JObject>>(json);

            states.ForEach(state => {
                dc.Table<State>().Add(new State
                {
                    Name = state["name"].ToString(),
                    Abbr = state["abbr"].ToString(),
                    CountryCode = "US"
                });
            });
        }
    }
}
