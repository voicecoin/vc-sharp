using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Voicecoin.Core.Utility;
using Voicecoin.RestApi;

namespace ContentFoundation.RestApi
{
    [AllowAnonymous]
    public class FileController : CoreController
    {
        [HttpPost]
        public string Upload()
        {
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            /*using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var gb2312 = Encoding.GetEncoding("GB2312");
            var content = System.IO.File.ReadAllText(filePath, gb2312);*/

            var fileName = DateTime.UtcNow.Ticks.ToString();
            var file = new AwsS3Helper(Database.Configuration);
            file.SaveFileToS3(filePath, "Temp", fileName);

            var presignedUrl = file.GeneratePreSignedURLFromS3("Temp", fileName);
            return presignedUrl;
        }
    }
}
