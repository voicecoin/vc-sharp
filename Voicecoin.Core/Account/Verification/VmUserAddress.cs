using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class VmUserAddress
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Address
        {
            get
            {
                return $"{AddressLine1} {AddressLine2}, {City}, {State} {Zipcode}, {Country}";
            }
        }
    }
}
