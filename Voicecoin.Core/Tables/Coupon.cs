using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Tables
{
    public class Coupon : DbRecord, IDbRecord
    {
        public String Code { get; set; }

        public String Description { get; set; }

        public Decimal PercentageOff { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public String Channel { get; set; }
    }
}
