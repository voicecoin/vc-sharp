﻿using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicecoin.Core.Coupons
{
    public class CouponCore
    {
        private Database dc;
        private IConfiguration config;

        public CouponCore(Database db, IConfiguration config)
        {
            dc = db;
            this.config = config;
        }

        public string GenerateCouponLink(String userId, String couponId)
        {
            var refer = new ReferList
            {
                CouponId = couponId,
                ReferCode = ShortId.Generate(true, false, 6).ToUpper(),
                Referee = userId
            };

            dc.Table<ReferList>().Add(refer);

            string host = config.GetSection("clientHost").Value;

            return $"{host}/invite/{refer.ReferCode}";
        }

        public Coupon GetCouponByCode(string code)
        {
            var coupon = dc.Table<Coupon>().FirstOrDefault(x => x.StartDate <= DateTime.UtcNow
                && x.EndDate >= DateTime.UtcNow && x.Code == code);

            return coupon;
        }

        public List<Coupon> GetAvailableCoupons()
        {
            var coupons = dc.Table<Coupon>().Where(x => x.StartDate <= DateTime.UtcNow
                && x.EndDate >= DateTime.UtcNow)
                .Select(x => x);

            return coupons.ToList();
        }
    }
}
