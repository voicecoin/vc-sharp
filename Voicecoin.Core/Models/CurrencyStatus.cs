using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.Core.Models
{
    public enum CurrencyStatus
    {
        UpcomingPreIco = 2,
        
        ActivePreIco = 8,

        ConcludedPreIco = 9,

        UpcomingIco = 12,

        ActiveIco = 18,

        ConcludedIco = 19,
    }
}
