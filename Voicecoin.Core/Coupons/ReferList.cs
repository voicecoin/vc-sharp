using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class ReferList : DbRecord, IDbRecord
    {
        /// <summary>
        /// UserId
        /// </summary>
        [StringLength(36)]
        public String Referee { get; set; }

        [StringLength(32)]
        public String ReferCode { get; set; }

        [StringLength(36)]
        public String CouponId { get; set; }

        public DateTime? ActivatedTime { get; set; }
    }
}
