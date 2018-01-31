using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class VmUserCreate
    {
        [Required]
        public String FullName { get; set; }

        [Required]
        public String Email { get; set; }

        [Required]
        public String Password { get; set; }

        /// <summary>
        /// Invitation code
        /// </summary>
        public String InvitationCode { get; set; }
    }
}
