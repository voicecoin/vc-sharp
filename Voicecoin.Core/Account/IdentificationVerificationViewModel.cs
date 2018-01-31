using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class IdentificationVerificationViewModel
    {
        public IFormFile FrontSidePhotoId { get; set; }

        public IFormFile BackSidePhotoId { get; set; }

        public String DocumentTypeId { get; set; }

        public String DocumentNumber { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
