using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Tables
{
    public class State : DbRecord, IDbRecord
    {
        [MaxLength(8)]
        public String Abbr { get; set; }

        [MaxLength(64)]
        public String Name { get; set; }

        [MaxLength(3)]
        public String CountryCode { get; set; }
    }
}
