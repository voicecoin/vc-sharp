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

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Nationality = model.Nationality;
            user.BirthDay = model.BirthDay;
            user.Address = model.Address;
        }
    }
}
