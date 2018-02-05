using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core
{
    public class ContributionTransaction : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String ContributionId { get; set; }

        [MaxLength(128)]
        public String Transaction { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal UsdPrice { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal TokenAmount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal TokenUsdPrice { get; set; }

        [ForeignKey("ContributionId")]
        public IcoContribution IcoContribution { get; set; }
    }
}
