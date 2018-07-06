using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;
using EntityFrameworkCore.BootKit;
using DotNetToolkit;
using Voicecoin.Core.Account;
using DotNetToolkit.JwtHelper;

namespace Voicecoin.RestApi
{
    public class AccountController : CoreController
    {
        /// <summary>
        /// Sign up a new account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Obsolete]
        public IActionResult CreateUser([FromBody] VmUserCreate account)
        {
            var existedUser = dc.Table<User>().Any(x => x.Email.ToLower() == account.Email.ToLower() ||
                x.UserName.ToLower() == account.Email.ToLower());

            if (existedUser) return BadRequest("Account already existed");

            var user = new User
            {
                Authenticaiton = new UserAuth { Password = account.Password },
                Email = account.Email,
                UserName = account.Email,
                FirstName = account.FullName.Split(' ').First(),
                LastName = account.FullName.Split(' ').Last()
            };

            dc.DbTran(async delegate {
                var userCore = new AccountCore(dc, Database.Configuration);
                await userCore.CreateUser(user);
            });

            return Ok("Register successfully. Please active your account through email.");
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public VmUser GetUser()
        {
            var user = dc.Table<User>().Find(CurrentUserId);
            return user.ToObject<VmUser>();
        }

        /// <summary>
        /// Check if user already exists
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("exist")]
        public Boolean UserExist([FromQuery] String email)
        {
            var user = dc.Table<User>().Any(x => x.Email == email);

            return user;
        }
        
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        [HttpPut("changepassword")]
        public bool ChangePassword(string oldpassword, string newpassword)
        {
            var authId = (from usr in dc.Table<User>()
                        join auth in dc.Table<UserAuth>() on usr.Id equals auth.UserId
                        where usr.Id == CurrentUserId && auth.Password == oldpassword
                        select auth.Id).FirstOrDefault();

            if (!String.IsNullOrEmpty(authId))
            {
                dc.DbTran(() => {
                    var user = dc.Table<UserAuth>().Find(authId);
                    user.Password = newpassword;
                });
            }


            return true;
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpdateUser([FromRoute] String id, User accountModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction("UpdateUser", new { id = accountModel.Id }, accountModel.Id);
        }

        [AllowAnonymous]
        [HttpGet("activate/{token}")]
        public IActionResult ActivateAccount([FromRoute] String token)
        {
            dc.DbTran(() =>
            {
                var user = (from usr in dc.Table<User>()
                            join auth in dc.Table<UserAuth>() on usr.Id equals auth.UserId
                            where auth.ActivationCode == token
                            select auth).FirstOrDefault();

                if(user != null)
                {
                    user.ActivationCode = String.Empty;
                    user.IsActivated = true;
                }
            });

            return Ok("Account activated");
        }

        /// <summary>
        /// Find back password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("RecoverPassword")]
        public IActionResult RecoverPassword([FromQuery] String email)
        {
            dc.DbTran(async delegate {
                var userCore = new AccountCore(dc, Database.Configuration);
                userCore.RecoverPassword(email);
            });
            return Ok("Please follow the instruction sent to your email to reset password.");
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] VmResetPassword model)
        {
            return Ok("Please follow the instruction sent to your email to reset password.");
        }
    }
}