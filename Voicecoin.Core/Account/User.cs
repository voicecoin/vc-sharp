using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using EntityFrameworkCore.BootKit;
using System.ComponentModel.DataAnnotations.Schema;
using Voicecoin.Core.Permission;

namespace Voicecoin.Core.Account
{
    /// <summary>
    /// User profile
    /// </summary>
    public class User : DbRecord, IDbRecord
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [MaxLength(64)]
        public String Name { get; set; }

        [Required]
        [StringLength(128)]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required]
        [StringLength(64)]
        public String Salt { get; set; }

        [StringLength(32)]
        public String ActivationCode { get; set; }

        public Boolean IsActivated { get; set; }

        [Required]
        [StringLength(64)]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Required]
        [StringLength(32)]
        public String FirstName { get; set; }

        [Required]
        [StringLength(32)]
        public String LastName { get; set; }

        [MaxLength(256)]
        public String Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SignupDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [MaxLength(36)]
        public String Nationality { get; set; }

        [NotMapped]
        public String FullName { get { return $"{FirstName} {LastName}"; } }

        public List<RoleOfUser> Roles { get; set; }

        [ForeignKey("UserId")]
        public UserAddress Address { get; set; }

        public User()
        {
            SignupDate = DateTime.UtcNow;
        }
    }
}
