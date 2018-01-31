using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class UserIdentification : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        /// <summary>
        /// Refer to taxonomy term
        /// </summary>
        [StringLength(36)]
        public String DocumentTypeId { get; set; }

        public String DocumentNumber { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
