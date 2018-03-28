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
        /// Get a valid token after login
        /// </summary>
        /// <param name="username">User Email</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(typeof(String), 200)]
        public IActionResult Token([FromBody] VmUserLogin userModel)
        {
            if (String.IsNullOrEmpty(userModel.UserName) || String.IsNullOrEmpty(userModel.Password))
            {
                return new BadRequestObjectResult("Username and password should not be empty.");
            }

            // validate from local
            var user = (from usr in dc.Table<User>()
                       join auth in dc.Table<UserAuth>() on usr.Id equals auth.UserId
                       where usr.UserName == userModel.UserName
                       select auth).FirstOrDefault();

            if (user != null)
            {
                if (!user.IsActivated)
                {
                    return BadRequest("Account hasn't been activated, please check your email to activate it.");
                }
                else
                {
                    // validate password
                    string hash = PasswordHelper.Hash(userModel.Password, user.Salt);
                    if (user.Password == hash)
                    {
                        return Ok(JwtToken.GenerateToken(Database.Configuration, user.UserId));
                    }
                    else
                    {
                        return BadRequest("Authorization Failed.");
                    }
                }
            }
            else
            {
                return BadRequest("Account doesn't exist");
            }
        }

        /*[HttpGet("list")]
        public PageResult<UserListViewModel> GetUsers(string name, [FromQuery] int page = 1)
        {
            var query = dc.Table<User>().AsQueryable().Select(x => new UserListViewModel
            {
                Email = x.Email,
                Name = x.Name
            });

            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            var total = query.Count();
            int size = 20;
            var items = query.Skip((page - 1) * size).Take(size).ToList();

            return new PageResult<UserListViewModel> { Total = total, Page = page, Size = size, Items = items };
        }*/

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