using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Coupons;

namespace Voicecoin.RestApi
{
    public class CouponController : CoreController
    {
        [HttpGet("validate/{code}")]
        public IActionResult ValidateCouponCode([FromRoute] string code)
        {
            var couponCore = new CouponCore(dc, Database.Configuration);

            bool existed = couponCore.GetCouponByCode(code) != null;

            return Ok(existed);
        }

        /// <summary>
        /// Get current available coupons
        /// </summary>
        /// <returns></returns>
        [HttpGet("available")]
        public IActionResult GetAvailableCoupons()
        {
            var coupon = new CouponCore(dc, Database.Configuration);
            return Ok(coupon.GetAvailableCoupons());
        }

        /// <summary>
        /// Generate invitation temp code
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        [HttpGet("generate/{couponId}")]
        public IActionResult GenerateCouponLink([FromRoute] string couponId)
        {
            var coupon = new CouponCore(dc, Database.Configuration);
            string link = String.Empty;

            dc.DbTran(() => {
                link = coupon.GenerateCouponLink(CurrentUserId, couponId);
            });

            return Ok($"{link}");
        }
    }
}
