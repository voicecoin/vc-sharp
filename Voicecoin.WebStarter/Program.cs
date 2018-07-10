using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Voicecoin.WebStarter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    
                    var env = hostingContext.HostingEnvironment;
                    var settings = Directory.GetFiles($"{env.ContentRootPath}{Path.DirectorySeparatorChar}Settings", "settings.*.json");
                    settings.ToList().ForEach(setting => {
                        config.AddJsonFile(setting, optional: false, reloadOnChange: true);
                    });
                })
                //.UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
#if VOICECHAIN
                .UseUrls("http://0.0.0.0:127")
#else
                .UseUrls("http://0.0.0.0:129")
#endif
                .Build();
    }
}
