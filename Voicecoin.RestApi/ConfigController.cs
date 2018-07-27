using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityFrameworkCore.BootKit;
using Voicecoin.RestApi;

namespace Voicecoin.RestApi
{
    public class ConfigController : CoreController
    {
        [AllowAnonymous]
        [HttpGet("site")]
        public IActionResult GetSettings()
        {
            IEnumerable<IConfigurationSection> settings = Database.Configuration.GetSection("SiteSetting").GetChildren();

            JObject result = new JObject();
            settings.ToList().ForEach(setting => {
                result.Add(setting.Key, setting.Value);
            });

            return  Ok(result);
        }
    }
}
