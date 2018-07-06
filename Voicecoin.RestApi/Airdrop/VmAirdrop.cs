using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.RestApi.Airdrop
{
    public class VmAirdrop
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        public String Address { get; set; }

        public String ReferCode { get; set; }
    }
}
