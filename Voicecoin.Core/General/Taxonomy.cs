using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core.General
{
    public class Taxonomy : DbRecord, IDbRecord
    {
        [MaxLength(32)]
        public String Name { get; set; }

        [MaxLength(256)]
        public String Description { get; set; }

        public Boolean IsActive { get; set; }

        [ForeignKey("TaxonomyId")]
        public List<TaxonomyTerm> Terms { get; set; }
    }
}
