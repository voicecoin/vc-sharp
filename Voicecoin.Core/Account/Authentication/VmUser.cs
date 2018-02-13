﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class VmUser
    {
        public String FullName
        {
            get
            {
                return $"{FirstName}";
            }
        }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Avatar { get; set; }
    }
}
