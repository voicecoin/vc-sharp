using EntityFrameworkCore.BootKit;
using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicecoin.Core.Airdrop
{
    public class AirdropCore
    {
        private Database dc;

        public AirdropCore(Database dc)
        {
            this.dc = dc;
        }

        public string Join(TbAirdrop data)
        {
            data.Email = data.Email.ToLower().Trim();
            var existed = dc.Table<TbAirdrop>().FirstOrDefault(x => data.Email == x.Email);
            if (existed != null) return existed.Code;

            data.Symbol = "ETH";
            data.Code = "VC" + ShortId.Generate(true, false, 8).ToLower();

            dc.DbTran(() =>
            {
                dc.Table<TbAirdrop>().Add(data);
            });

            return data.Code;
        }
    }
}
