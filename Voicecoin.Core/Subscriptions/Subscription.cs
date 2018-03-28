using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Subscriptions
{
    public class Subscription : DbRecord, IDbRecord
    {
        [StringLength(64)]
        public String Email { get; set; }

        public Boolean IsActive { get; set; }
    }
}
