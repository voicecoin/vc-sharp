using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.RestApi.Airdrop
{
    public class VmAirdrop
    {
        /// <summary>
        /// 参与者的邮件地址
        /// </summary>
        [Required]
        [EmailAddress]
        public String Email { get; set; }

        /// <summary>
        /// 接受糖果的数字货币地址
        /// </summary>
        [Required]
        public String Address { get; set; }

        /// <summary>
        /// 推荐人的原始邀请码
        /// </summary>
        public String ReferCode { get; set; }
    }
}
