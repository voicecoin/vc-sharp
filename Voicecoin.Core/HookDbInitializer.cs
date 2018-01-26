using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using CustomEntityFoundation;
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
        }
    }
}
