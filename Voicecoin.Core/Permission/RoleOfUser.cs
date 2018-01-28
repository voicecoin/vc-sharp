using EntityFrameworkCore.BootKit;
using System;
using System.ComponentModel.DataAnnotations;
using Voicecoin.Core.Account;

namespace Voicecoin.Core.Permission
{
    public class RoleOfUser : DbRecord, IDbRecord
    {
        [StringLength(36)]
        public String UserId { get; set; }

        public User User { get; set; }

        [StringLength(36)]
        public String RoleId { get; set; }

        public Role Role { get; set; }
    }
}
