using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core;
using Voicecoin.Core.Subscriptions;

namespace Voicecoin.RestApi
{
    [AllowAnonymous]
    public class SubscriptionController : CoreController
    {
        private static RazorLightEngine engine;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] VmSubscription sub)
        {
            if (!dc.Table<Subscription>().Any(x => x.Email == sub.Email.ToLower()))
            {
                dc.DbTran(() =>
                {
                    dc.Table<Subscription>().Add(new Subscription()
                    {
                        Email = sub.Email.ToLower(),
                        IsActive = true
                    });
                });

                EmailRequestModel model = new EmailRequestModel();

                model.Subject = Database.Configuration.GetSection("UserSubscriptionEmail:Subject").Value;
                model.ToAddresses = sub.Email;
                model.Template = Database.Configuration.GetSection("UserSubscriptionEmail:Template").Value;

                if (engine == null)
                {
                    engine = new RazorLightEngineBuilder()
                      .UseFilesystemProject(Database.ContentRootPath + "\\App_Data")
                      .UseMemoryCachingProvider()
                      .Build();
                }

                var cacheResult = engine.TemplateCache.RetrieveTemplate(model.Template);

                var emailModel = new { Host = Database.Configuration.GetSection("clientHost").Value, Email = sub.Email };

                if (cacheResult.Success)
                {
                    model.Body = await engine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), emailModel);
                }
                else
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }

                var ses = new AwsSesHelper(Database.Configuration);
                string emailId = await ses.Send(model, Database.Configuration);
            }

            return Ok();
        }

        [HttpGet("/unsubscribe")]
        public IActionResult Delete([FromQuery] string email)
        {
            if (dc.Table<Subscription>().Any(x => x.Email == email.ToLower()))
            {
                dc.DbTran(() =>
                {
                    var subscription = dc.Table<Subscription>().First(x => x.Email == email.ToLower());
                    dc.Table<Subscription>().Remove(subscription);
                });
            }

            return Ok();
        }
    }
}
