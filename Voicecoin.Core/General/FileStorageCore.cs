using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Http;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voicecoin.Core.General
{
    public class FileStorageCore
    {
        private Database dc;
        private String userId;

        public FileStorageCore(Database dc, string userId)
        {
            this.dc = dc;
            this.userId = userId;
        }

        public async Task<string> Save(IFormFile file, string dir = "Documents")
        {
            if (file == null || file.Length == 0)
            {
                return String.Empty;
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileType = file.ContentType.Split('/').Last();
            var fileName = ShortId.Generate(true, false, 8) + "." + fileType;
            var s3file = new AwsS3Helper(Database.Configuration);
            s3file.SaveFileToS3(filePath, dir, fileName);

            var presignedUrl = s3file.GeneratePreSignedURLFromS3(dir, fileName);

            // read file to double check file is uploaded to S3
            if(!await s3file.ValidateFileSizeFromS3(dir, fileName, file.Length))
            {
                throw new Exception("Upload failed, file size exception");
            }

            var storage = new FileStorage
            {
                OriginalFileName = file.FileName,
                ConvertedFileName = fileName,
                Size = file.Length,
                ContentType = file.ContentType,
                Directory = dir,
                UserId = userId
            };

            dc.DbTran(() => {
                dc.Table<FileStorage>().Add(storage);
            });

            return storage.Id;
        }
    }
}
