using EntityFrameworkCore.BootKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Voicecoin.Core.Account;

namespace Voicecoin.RestApi.Account
{
    public sealed class JwtToken
    {
        private JwtSecurityToken token;

        internal JwtToken(JwtSecurityToken token)
        {
            this.token = token;
        }

        public DateTime ValidTo => token.ValidTo;
        public string Value => new JwtSecurityTokenHandler().WriteToken(this.token);

        public static string GenerateToken(User user)
        {
            var Configuration = Database.Configuration;
            var authConfig = Configuration.GetSection("TokenAuthentication");

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(authConfig["SecretKey"]))
                .AddSubject(authConfig["Subject"])
                .AddIssuer(authConfig["Issuer"])
                .AddAudience(authConfig["Audience"])
                .AddClaim("FirstName", user.FirstName)
                .AddClaim("LastName", user.LastName)
                .AddClaim("Email", user.Email)
                .AddClaim("UserName", user.Name)
                .AddClaim("UserId", user.Id)
                .AddExpiry(480)
                .Build();

            return token.Value;
        }
    }
}
