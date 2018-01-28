using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core.Utility;

namespace Voicecoin.Core.Account
{
    public class AccountCore
    {
        private Database dc;
        private IConfiguration config;
        private static RazorLightEngine engine;

        public String Host { get; set; }

        public AccountCore(Database db, IConfiguration config)
        {
            dc = db;
            this.config = config;
        }

        public async Task CreateUser(User user)
        {
            user.Salt = PasswordHelper.GetSalt();
            user.Password = PasswordHelper.Hash(user.Password, user.Salt);
            user.ActivationCode = Guid.NewGuid().ToString("N");

            dc.Table<User>().Add(user);

            EmailRequestModel model = new EmailRequestModel();

            model.Subject = Database.Configuration.GetSection("UserActivationEmail:Subject").Value;
            model.ToAddresses = user.Email;
            model.Template = Database.Configuration.GetSection("UserActivationEmail:Template").Value;
            model.Bcc = Database.Configuration.GetSection("UserActivationEmail:Bcc").Value;
            model.Cc = Database.Configuration.GetSection("UserActivationEmail:Cc").Value;

            if (!String.IsNullOrEmpty(model.Template))
            {
                if (engine == null)
                {
                    engine = new RazorLightEngineBuilder()
                      .UseFilesystemProject(Database.ContentRootPath + "\\App_Data")
                      .UseMemoryCachingProvider()
                      .Build();
                }

                var cacheResult = engine.TemplateCache.RetrieveTemplate(model.Template);

                var emailModel = new { Host = Host, ActivationCode = user.ActivationCode };

                if (cacheResult.Success)
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }
                else
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }
            }

            var ses = new AwsSesHelper(Database.Configuration);
            string emailId = await ses.Send(model, config);

            $"Created user {user.Email}, user id: {user.Id}, sent email: {emailId}.".Log(CfLogLevel.INFO);
        }
    }
}
