using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core.Airdrop;

namespace Voicecoin.RestApi.Airdrop
{
    [AllowAnonymous]
    public class AirdropController : CoreController
    {
        /// <summary>
        /// 申请参加活动，获取邀请码。
        /// </summary>
        /// <param name="data"></param>
        /// <returns>邀请码</returns>
        [HttpPost]
        public async Task<ActionResult<String>> Join(VmAirdrop data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var airdrop = new AirdropCore(dc);
            string code = await airdrop.JoinAsync(data.ToObject<TbAirdrop>());

            return Ok(code);
        }

        /// <summary>
        /// 确认参加活动，激活邀请码。
        /// </summary>
        /// <param name="activationCode">邀请激活码</param>
        /// <returns></returns>
        [HttpGet("Activate/{activationCode}")]
        public IActionResult Activate([FromRoute] string activationCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var airdrop = new AirdropCore(dc);
            var data = airdrop.Activate(activationCode);
            var host = Database.Configuration.GetSection("clientHost").Value;
            return Redirect($"{host}/candy/invite.html?code={data.Code}");
        }
    }
}
