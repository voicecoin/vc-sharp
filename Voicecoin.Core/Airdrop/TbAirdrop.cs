using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core.Airdrop
{
    [Table("Airdrop")]
    public class TbAirdrop : DbRecord, IDbRecord
    {
        [MaxLength(64)]
        public String Email { get; set; }

        [MaxLength(16)]
        public String Symbol { get; set; }

        [MaxLength(128)]
        public String Address { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [MaxLength(10)]
        public String Code { get; set; }

        /// <summary>
        /// Refered by
        /// </summary>
        [MaxLength(10)]
        public String ReferCode { get; set; }

        public bool Activated { get; set; }

        [StringLength(8)]
        public String ActivationCode { get; set; }
    }
}
