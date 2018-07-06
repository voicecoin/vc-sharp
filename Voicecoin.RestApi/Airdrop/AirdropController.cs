using DotNetToolkit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Voicecoin.Core.Airdrop;

namespace Voicecoin.RestApi.Airdrop
{
    [AllowAnonymous]
    public class AirdropController : CoreController
    {
        [HttpPost]
        public IActionResult Join(VmAirdrop data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var airdrop = new AirdropCore(dc);
            airdrop.Join(data.ToObject<TbAirdrop>());

            return Ok();
        }
    }
}
