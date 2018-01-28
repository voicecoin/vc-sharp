using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Tables
{
    public class Country : DbRecord, IDbRecord
    {
        [StringLength(2)]
        public String Code2 { get; set; }

        [StringLength(3)]
        public String Code3 { get; set; }

        [MaxLength(64)]
        public String Name { get; set; }

        [MaxLength(64)]
        public String Nationality { get; set; }
    }
}
