using EntityFrameworkCore.BootKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class UserPersonalCore
    {
        private Database dc;
        private string userId;

        public UserPersonalCore(Database dc, string userId)
        {
            this.dc = dc;
            this.userId = userId;
        }

        public User GetPersonalInfo()
        {
            return dc.Table<User>().Include(x => x.Address).First(x => x.Id == userId);
        }

        public void UpdatePersonalInfo(User model)
        {
            var user = dc.Table<User>().Find(userId);
            var address = dc.Table<UserAddress>().FirstOrDefault(x => x.UserId == userId);

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

            dc.SaveChanges();
        }
    }
}
