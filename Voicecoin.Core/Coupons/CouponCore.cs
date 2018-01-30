using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using shortid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class CouponCore
    {
        private Database dc;
        private IConfiguration config;
        private string userId;

        public CouponCore(string userId, Database db, IConfiguration config)
        {
            dc = db;
            this.config = config;
            this.userId = userId;
        }

        public string GenerateCouponLink(String couponId)
        {
            var refer = new ReferList
            {
                CouponId = couponId,
                ReferCode = ShortId.Generate(false, false, 6).ToUpper(),
                Referee = userId
            };

            dc.Table<ReferList>().Add(refer);

            string host = config.GetSection("clientHost").Value;

            return $"{host}/register?coupon={refer.ReferCode}";
        }
    }
}
