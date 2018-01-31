using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicecoin.Core.Account
{
    public class UserVerifyCore
    {
        private Database dc;
        private string userId;

        public UserVerifyCore(Database dc, string userId)
        {
            this.dc = dc;
            this.userId = userId;
        }

        public List<UserDeclaration> GetDeclarations()
        {
            var declares = dc.Table<UserDeclaration>()
                .Where(x => x.UserId == userId)
                .ToList();

            return declares;
        }

        public void UpdateDeclarations(string tag, string declaration)
        {
            var declare = dc.Table<UserDeclaration>()
                    .FirstOrDefault(x => x.UserId == userId &&
                    x.Tag == tag);

            if (declare == null)
            {
                dc.Table<UserDeclaration>().Add(new UserDeclaration
                {
                    UserId = userId,
                    Declaration = declaration,
                    Tag = tag
                });
            }
            else
            {
                declare.Declaration = declaration;
            }
        }
    }
}
