using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core.General
{
    public class TaxonomyTerm : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String TaxonomyId { get; set; }

        [ForeignKey("TaxonomyId")]
        public Taxonomy Taxonomy { get; set; }

        [MaxLength(64)]
        public String Term { get; set; }

        public Boolean IsActive { get; set; }
    }
}
