using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class UserCoupon : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        [StringLength(36)]
        public String CouponId { get; set; }
    }
}
