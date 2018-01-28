using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Voicecoin.Core.Permission
{
    /// <summary>
    /// User Role
    /// </summary>
    public class Role : DbRecord, IDbRecord
    {
        [Required]
        [StringLength(36)]
        public String Name { get; set; }

        [MaxLength(128)]
        public String Description { get; set; }
    }
}
