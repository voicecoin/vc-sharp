using Coinbase.Models;
using ContentFoundation.RestApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Voicecoin.RestApi
{
    /*[AllowAnonymous]
    [Produces("application/json")]
    [Route("coinbase/notification")]
    public class CoinbaseNotificationController : CoreController
    {
        [HttpPost]
        public IActionResult Received([FromBody] NotificationModel notification)
        {
            Console.WriteLine(JsonConvert.SerializeObject(notification));

            return Ok();
        }
    }*/
}
