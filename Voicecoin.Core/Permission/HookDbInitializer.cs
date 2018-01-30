using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Voicecoin.Core.Permission
{
    public class HookDbInitializer : IHookDbInitializer
    {
        public int Priority => 100;

        public void Load(IConfiguration config, Database dc)
        {
            InitRoles(config, dc);
        }

        private void InitRoles(IConfiguration config, Database dc)
        {
            if (dc.Table<Role>().Any()) return;

            string json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Voicecoin.Roles.json");
            var roles = JsonConvert.DeserializeObject<JsonRoles>(json);
            dc.Table<Role>().AddRange(roles.Roles);
        }

        private class JsonRoles
        {
            public List<Role> Roles { get; set; }
        }
    }

}
