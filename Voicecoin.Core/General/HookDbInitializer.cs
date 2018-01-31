using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Voicecoin.Core.General
{
    public class HookDbInitializer : IHookDbInitializer
    {
        public int Priority => 2000;

        public void Load(IConfiguration config, Database dc)
        {
            InitTaxonomy(config, dc);
        }

        private void InitTaxonomy(IConfiguration config, Database dc)
        {
            if (dc.Table<Taxonomy>().Any(x => x.Id == "ee3ccd84-cff1-4701-8c61-9f9d88a6e03e")) return;

            string json = File.ReadAllText(Database.ContentRootPath + "\\App_Data\\DbInitializer\\Common.IdDocumentType.json");
            var taxonomy = JsonConvert.DeserializeObject<TaxonomyLoadModel>(json);

            dc.Table<Taxonomy>().Add(new Taxonomy
            {
                Id = taxonomy.TaxonomyId,
                Name = taxonomy.TaxonomyName,
                IsActive = true
            });

            dc.Table<TaxonomyTerm>()
                .AddRange(taxonomy.Terms.Select(x => new TaxonomyTerm
                {
                    TaxonomyId = taxonomy.TaxonomyId,
                    Term = x,
                    IsActive = true
                }));
        }

        private class TaxonomyLoadModel
        {
            public String TaxonomyId { get; set; }

            public String TaxonomyName { get; set; }

            public List<String> Terms { get; set; }
        }
    }
}
