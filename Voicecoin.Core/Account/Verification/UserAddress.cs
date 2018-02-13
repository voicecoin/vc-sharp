using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class UserAddress : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [MaxLength(256)]
        public string AddressLine1 { get; set; }

        [MaxLength(256)]
        public string AddressLine2 { get; set; }

        [MaxLength(10)]
        public string Zipcode { get; set; }

        [MaxLength(64)]
        public string City { get; set; }

        [MaxLength(64)]
        public string County { get; set; }

        [MaxLength(64)]
        public string State { get; set; }

        [MaxLength(64)]
        public string Country { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
