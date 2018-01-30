using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Account;
using Voicecoin.Core.Coupons;
using Voicecoin.Core.Tables;

namespace Voicecoin.RestApi
{
    public class CouponController : CoreController
    {
        [HttpPut("apply/{code}")]
        public IActionResult ApplyCouponCode([FromRoute] string code)
        {
            var coupon = dc.Table<Coupon>().FirstOrDefault(x => x.Code == code &&
                x.StartDate <= DateTime.UtcNow
                && x.EndDate >= DateTime.UtcNow);

            if (coupon != null)
            {
                string userId = GetCurrentUser().Id;

                dc.DbTran(() => {
                    var user = dc.Table<User>().Find(userId);
                    user.CouponId = coupon.Id;
                });

                return Ok($"Coupon code applied successfully.");
            }
            else
            {
                return BadRequest($"Coupon code is invalide");
            }
        }

        [HttpGet("available")]
        public IActionResult GetAvaliableCoupons()
        {
            var coupons = dc.Table<Coupon>().Where(x => x.StartDate <= DateTime.UtcNow
                && x.EndDate >= DateTime.UtcNow)
                .Select(x => new { x.Id, x.Code, x.Description, x.EndDate });

            return Ok(coupons);
        }

        [HttpGet("generate/{couponId}")]
        public IActionResult GenerateCouponLink([FromRoute] string couponId)
        {
            var coupon = new CouponCore(GetCurrentUser().Id, dc, Database.Configuration);
            string link = String.Empty;

            dc.DbTran(() => {
                link = coupon.GenerateCouponLink(couponId);
            });

            return Ok($"{link}");
        }
    }
}
