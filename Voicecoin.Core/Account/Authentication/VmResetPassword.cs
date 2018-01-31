using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Account
{
    /// <summary>
    /// Use either SecurityCode or OriginalPassword to reset password
    /// </summary>
    public class VmResetPassword
    {
        public String Email { get; set; }

        /// <summary>
        /// Refer to PasswordRecovery.SecurityCode
        /// </summary>
        public String SecurityCode { get; set; }

        public String OriginalPassword { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        public String Password { get; set; }
    }
}
