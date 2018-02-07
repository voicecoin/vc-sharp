using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Voicecoin.Core.Account;

namespace Voicecoin.RestApi
{
    [Authorize]
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class CoreController : ControllerBase
    {
        protected Database dc { get; set; }

        public CoreController()
        {
            dc = new DefaultDataContextLoader().GetDefaultDc();
        }

        protected String GetConfig(string path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;
            
            return Database.Configuration.GetSection(path).Value;
        }

        protected List<KeyValuePair<String, String>> GetSection(string path)
        {
            return Database.Configuration.GetSection(path).AsEnumerable().ToList();
        }

        protected String CurrentUserId
        {
            get
            {
                return this.User.Claims.FirstOrDefault(x => x.Type.Equals("UserId"))?.Value;
            }
        }

        protected User GetCurrentUser()
        {
            if (this.User != null)
            {
                return new User
                {
                    Id = this.User.Claims.FirstOrDefault(x => x.Type.Equals("UserId"))?.Value
                };
            }
            else
            {
                return new User
                {
                    Id = Guid.Empty.ToString()
                };
            }
        }
    }
}
