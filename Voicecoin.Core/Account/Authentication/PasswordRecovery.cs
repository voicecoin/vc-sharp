using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class PasswordRecovery : DbRecord, IDbRecord
    {
        [MaxLength(64)]
        public string Email { get; set; }

        /// <summary>
        /// One time security code
        /// </summary>
        [StringLength(32)]
        public string SecurityCode { get; set; }

        public DateTime ExpiredDate { get; set; }
    }
}
