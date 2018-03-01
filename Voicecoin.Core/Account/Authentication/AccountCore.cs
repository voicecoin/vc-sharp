using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core.Permission;

namespace Voicecoin.Core.Account
{
    public class AccountCore
    {
        private Database dc;
        private IConfiguration config;
        private static RazorLightEngine engine;

        public const String SALES_ROLE_ID = "150a431c-86b5-4a22-acc4-207a4a08cc01";
        public const String ADMIN_ROLE_ID = "9fb3536c-130c-48f4-93be-fd141ca8f29d";
        public const String AUTH_ROLE_ID = "095bbb96-a103-458c-a79f-e6832dc547bf";

        public AccountCore(Database db, IConfiguration config)
        {
            dc = db;
            this.config = config;
        }

        public async Task CreateUser(User user)
        {
            user.Authenticaiton.Salt = PasswordHelper.GetSalt();
            user.Authenticaiton.Password = PasswordHelper.Hash(user.Authenticaiton.Password, user.Authenticaiton.Salt);
            user.Authenticaiton.ActivationCode = Guid.NewGuid().ToString("N");

            dc.Table<User>().Add(user);

            dc.Table<RoleOfUser>().Add(new RoleOfUser
            {
                RoleId = AUTH_ROLE_ID,
                UserId = user.Id
            });

            dc.SaveChanges();

            EmailRequestModel model = new EmailRequestModel();

            model.Subject = config.GetSection("UserActivationEmail:Subject").Value;
            model.ToAddresses = user.Email;
            model.Template = config.GetSection("UserActivationEmail:Template").Value;
            model.Bcc = config.GetSection("UserActivationEmail:Bcc").Value;
            model.Cc = config.GetSection("UserActivationEmail:Cc").Value;

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

                var emailModel = new { Host = config.GetSection("clientHost").Value, ActivationCode = user.Authenticaiton.ActivationCode };

                if (cacheResult.Success)
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }
                else
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }
            }

            var ses = new AwsSesHelper(config);
            string emailId = await ses.Send(model, config);

            $"Created user {user.Email}, user id: {user.Id}, sent email: {emailId}.".Log(LogLevel.INFO);
        }

        public void RecoverPassword(string email)
        {
            var recovery = new PasswordRecovery
            {
                Email = email,
                ExpiredDate = DateTime.UtcNow.AddDays(1),
                SecurityCode = Guid.NewGuid().ToString("N")
            };

            dc.Table<PasswordRecovery>().Add(recovery);

            // send a email
        }
    }
}
