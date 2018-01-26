using ContentFoundation.RestApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.RestApi
{
    [Route("v1/[controller]")]
    public class VerificationController : CoreController
    {
        [HttpGet]
        public Object GetProfile()
        {
            return new { };
        }

        [HttpPut]
        public IActionResult UpdateProfile()
        {
            return Ok();
        }
    }
}
