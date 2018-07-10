using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using RazorLight;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voicecoin.Core.Airdrop
{
    public class AirdropCore
    {
        private Database dc;
        private static RazorLightEngine engine;

        public AirdropCore(Database dc)
        {
            this.dc = dc;
        }

        public async Task<string> JoinAsync(TbAirdrop data)
        {
            data.Email = data.Email.ToLower().Trim();
            var existed = dc.Table<TbAirdrop>().FirstOrDefault(x => data.Email == x.Email);
            if (existed != null)
            {
                await SendNotification(existed);
                return existed.Code;
            }

            data.Symbol = "ETH";
            data.Code = "VC" + ShortId.Generate(true, false, 8).ToLower();
            data.ActivationCode = ShortId.Generate(true, false, 8);

            dc.DbTran(() =>
            {
                dc.Table<TbAirdrop>().Add(data);
            });

            await SendNotification(existed);

            return data.Code;
        }

        public async Task SendNotification(TbAirdrop data)
        {
            var config = Database.Configuration;
            EmailRequestModel model = new EmailRequestModel();

            model.Subject = config.GetSection("JoinCandyProgramChinese:Subject").Value;
            model.ToAddresses = data.Email;
            model.Template = config.GetSection("JoinCandyProgramChinese:Template").Value;

            if (!String.IsNullOrEmpty(model.Template))
            {
                if (engine == null)
                {
                    engine = new RazorLightEngineBuilder()
                      .UseFilesystemProject(Database.ContentRootPath + $"{Path.DirectorySeparatorChar}App_Data")
                      .UseMemoryCachingProvider()
                      .Build();
                }

                var cacheResult = engine.TemplateCache.RetrieveTemplate(model.Template);

                var emailModel = new { Host = config.GetSection("clientHost").Value, ActivationCode = data.ActivationCode };

                if (cacheResult.Success)
                {
                    model.Body = await engine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), emailModel);
                }
                else
                {
                    model.Body = await engine.CompileRenderAsync(model.Template, emailModel);
                }
            }

            var ses = new AwsSesHelper(config);
            string emailId = await ses.Send(model, config);

            $"join candy program user {data.Email}, user id: {data.Id}, sent email: {emailId}.".Log(LogLevel.INFO);
        }

        public TbAirdrop Activate(String code)
        {
            TbAirdrop airdrop = dc.Table<TbAirdrop>().FirstOrDefault(x => x.ActivationCode == code);

            if (airdrop == null) return new TbAirdrop();
            
            dc.DbTran(() => {

                airdrop = dc.Table<TbAirdrop>().FirstOrDefault(x => x.ActivationCode == code);

                airdrop.Activated = true;
                airdrop.UpdatedTime = DateTime.UtcNow;
            });

            return airdrop;
        }
    }
}
