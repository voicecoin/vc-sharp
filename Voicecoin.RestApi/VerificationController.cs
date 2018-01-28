using DotNetToolkit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicecoin.Core.Models;
using Voicecoin.Core.Tables;

namespace Voicecoin.RestApi
{
    [Route("v1/[controller]")]
    public class VerificationController : CoreController
    {
        [AllowAnonymous]
        [HttpGet("countries")]
        public PageResult<object> GetCountries()
        {
            var result = new PageResult<object>
            {
                Page = 1,
                Size = 1000
            };

            var query = dc.Table<Country>()
                .Select(x => new { Code = x.Code2, x.Name, x.Nationality });

            return result.LoadRecords(query);
        }

        [AllowAnonymous]
        [HttpGet("{country}/states")]
        public PageResult<object> GetCountries([FromRoute] string country = "US")
        {
            var result = new PageResult<object>
            {
                Page = 1,
                Size = 100
            };

            var query = dc.Table<State>().Where(x => x.CountryCode == country)
                .Select(x => new { x.Abbr, x.Name });

            return result.LoadRecords(query);
        }

        [HttpGet]
        public VmPersonalInfomation GetPersonalInformation()
        {
            return new VmPersonalInfomation { };
        }

        [HttpPut]
        public IActionResult UpdatePersonalInformation(VmPersonalInfomation vm)
        {
            string userId = GetCurrentUser().Id;



            return Ok();
        }
    }
}
