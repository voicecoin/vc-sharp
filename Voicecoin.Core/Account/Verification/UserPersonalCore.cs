using EntityFrameworkCore.BootKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voiceweb.Auth.Core.DbTables;

namespace Voicecoin.Core.Account
{
    public class UserPersonalCore
    {
        private Database dc;
        private Database authDc;
        private string userId;

        public UserPersonalCore(string userId)
        {
            dc = new DefaultDataContextLoader().GetDefaultDc();
            authDc = new DefaultDataContextLoader().GetDefaultDc2<IAuthDbRecord>("VoicewebAuth");
            this.userId = userId;
        }

        public TbUser GetPersonalInfo()
        {
           return authDc.Table<TbUser>().Include(x => x.Address).FirstOrDefault(x => x.Id == userId);
        }

        public void UpdatePersonalInfo(TbUser model)
        {
            authDc.Transaction<IAuthDbRecord>(() => {
                var user = authDc.Table<TbUser>().Find(userId);
                var address = authDc.Table<TbUserAddress>().FirstOrDefault(x => x.UserId == userId);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Nationality = model.Nationality;
                user.Birthday = model.Birthday;
                user.UpdatedTime = DateTime.UtcNow;

                if (address == null)
                {
                    user.Address = model.Address;
                    user.Address.UserId = user.Id;
                    user.Address.UpdatedTime = DateTime.UtcNow;
                }
                else
                {
                    address.AddressLine1 = model.Address.AddressLine1;
                    address.AddressLine2 = model.Address.AddressLine2;
                    address.Country = model.Address.Country;
                    address.County = model.Address.County;
                    address.State = model.Address.State;
                    address.City = model.Address.City;
                    address.Zipcode = model.Address.Zipcode;
                    address.UpdatedTime = DateTime.UtcNow;
                }
            });
        }
    }
}
