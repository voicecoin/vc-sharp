using System;
using Voicecoin.Core.Account;

namespace Voicecoin.Core.Models
{
    public class VmPersonalInfomation
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Nationality { get; set; }

        public DateTime? BirthDay { get; set; }

        public VmUserAddress Address { get; set; }
    }
}
