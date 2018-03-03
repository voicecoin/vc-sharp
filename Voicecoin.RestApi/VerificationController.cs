using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shortid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voicecoin.Core;
using Voicecoin.Core.Account;
using Voicecoin.Core.General;
using Voicecoin.Core.Models;

namespace Voicecoin.RestApi
{
    /// <summary>
    /// User profile and documents
    /// </summary>
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
                .OrderBy(x => x.Name)
                .Select(x => new { Code = x.Code2, x.Name, x.Nationality });

            return result.LoadRecords(query);
        }

        [AllowAnonymous]
        [HttpGet("{country}/states")]
        public PageResult<object> GetCountryStates([FromRoute] string country = "US")
        {
            var result = new PageResult<object>
            {
                Page = 1,
                Size = 100
            };

            var query = dc.Table<State>()
                .Where(x => x.CountryCode == country)
                .OrderBy(x => x.Name)
                .Select(x => new { x.Abbr, x.Name });

            return result.LoadRecords(query);
        }

        [HttpGet("PersonalInformation")]
        public VmPersonalInfomation GetPersonalInformation()
        {
            var personal = new UserPersonalCore(dc, CurrentUserId);
            var user = personal.GetPersonalInfo();

            var pi = new VmPersonalInfomation
            {
                Birthday = user.Birthday,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nationality = user.Nationality
            };

            if (user.Address != null)
            {
                pi.Address = new VmUserAddress
                {
                    AddressLine1 = user.Address.AddressLine1,
                    AddressLine2 = user.Address.AddressLine2,
                    City = user.Address.City,
                    Country = user.Address.Country,
                    County = user.Address.County,
                    State = user.Address.State,
                    Zipcode = user.Address.Zipcode
                };
            }
            else
            {

                pi.Address = new VmUserAddress();
            }

            return pi;
        }

        [HttpPost("PersonalInformation")]
        public IActionResult UploadPersonalInformation([FromBody] VmPersonalInfomation vm)
        {
            var personal = new UserPersonalCore(dc, CurrentUserId);

            dc.DbTran(() =>
            {
                var user = vm.ToObject<User>();
                personal.UpdatePersonalInfo(user);
            });

            return Ok();
        }

        [HttpGet("Declarations")]
        public VmUserDeclarations GetDeclarations()
        {
            var verify = new UserVerifyCore(dc, CurrentUserId);
            var declares = verify.GetDeclarations();

            return new VmUserDeclarations
            {
                Declaration1 = declares.FirstOrDefault(x => x.Tag == TagConstants.UserDeclaration1)?.Declaration,
                Declaration2 = declares.FirstOrDefault(x => x.Tag == TagConstants.UserDeclaration2)?.Declaration,
                Declaration3 = declares.FirstOrDefault(x => x.Tag == TagConstants.UserDeclaration3)?.Declaration
            };
        }

        [HttpPost("Declarations")]
        public IActionResult UploadDeclarations([FromBody] VmUserDeclarations declarations)
        {
            var verify = new UserVerifyCore(dc, CurrentUserId);

            dc.DbTran(() => {
                verify.UpdateDeclarations(TagConstants.UserDeclaration1, declarations.Declaration1);
                verify.UpdateDeclarations(TagConstants.UserDeclaration2, declarations.Declaration2);
                verify.UpdateDeclarations(TagConstants.UserDeclaration3, declarations.Declaration3);
            });

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("IdDocumentTypes")]
        public PageResult<Object> GetIdDocumentTypes()
        {
            var result = new PageResult<object>
            {
                Page = 1,
                Size = 100
            };

            var query = dc.Table<TaxonomyTerm>()
                .Where(x => x.TaxonomyId == IdConstants.IdDocumentType)
                .Select(x => x.ToObject<VmTaxonomyTerm>())
                .OrderBy(x => x.Term);

            return result.LoadRecords(query);
        }

        [HttpGet("IdentificationVerification")]
        public VmIdentificationVerification GetIdentificationVerification()
        {
            var identification = dc.Table<UserIdentification>().FirstOrDefault(x => x.UserId == CurrentUserId);
            return new VmIdentificationVerification
            {
                DocumentNumber = identification?.DocumentNumber,
                DocumentTypeId = identification?.DocumentTypeId,
                ExpiryDate = identification?.ExpiryDate,
                IssueDate = identification?.IssueDate 
            };
        }

        [HttpPost("IdentificationVerification")]
        public IActionResult UploadIdentificationVerification(VmIdentificationVerification model)
        {
            var userId = CurrentUserId;

            dc.DbTran(async () =>
            {
                var identification = dc.Table<UserIdentification>().FirstOrDefault(x => x.UserId == CurrentUserId);
                if (identification == null)
                {
                    dc.Table<UserIdentification>().Add(new UserIdentification
                    {
                        UserId = CurrentUserId,
                        DocumentTypeId = model.DocumentTypeId,
                        DocumentNumber = model.DocumentNumber,
                        IssueDate = model.IssueDate,
                        ExpiryDate = model.ExpiryDate
                    });
                }
                else
                {
                    identification.DocumentTypeId = model.DocumentTypeId;
                    identification.DocumentNumber = model.DocumentNumber;
                    identification.IssueDate = model.IssueDate;
                    identification.ExpiryDate = model.ExpiryDate;
                }

                if (model.FrontSidePhoto != null && model.FrontSidePhoto.Length > 0)
                {
                    var storage = new FileStorageCore(dc, userId);
                    var storageId = await storage.Save(model.FrontSidePhoto);

                    var doc = new UserDocument
                    {
                        UserId = userId,
                        Tag = "FrontSidePhotoId",
                        FileStorageId = storageId
                    };

                    dc.Table<UserDocument>().Add(doc);
                }

                if (model.BackSidePhoto != null && model.BackSidePhoto.Length > 0)
                {
                    var storage = new FileStorageCore(dc, userId);
                    var storageId = await storage.Save(model.BackSidePhoto);

                    var doc = new UserDocument
                    {
                        UserId = userId,
                        Tag = "BackSidePhotoId",
                        FileStorageId = storageId
                    };

                    dc.Table<UserDocument>().Add(doc);
                }
            });

            return Ok();
        }

        [HttpGet("ResidenceVerification")]
        public IActionResult GetResidenceVerification()
        {
            return Ok();
        }

        [HttpPost("ResidenceVerification")]
        public async Task<IActionResult> UploadResidenceVerification(IFormFile file)
        {
            if (file.Length == 0) return BadRequest("File size is zero.");
            var userId = CurrentUserId;

            dc.DbTran(async () =>
            {
                var storage = new FileStorageCore(dc, userId);
                var storageId = await storage.Save(file);

                var doc = new UserDocument
                {
                    UserId = userId,
                    Tag = "ResidenceVerification",
                    FileStorageId = storageId
                };

                dc.Table<UserDocument>().Add(doc);
            });

            return Ok();
        }

        [HttpGet("DocumentSignature")]
        public IActionResult GetDocumentSignature()
        {
            return Ok();
        }

        /// <summary>
        /// Upload Simple Agreement for Future Tokens (SAFT)
        /// </summary>
        /// <returns></returns>
        [HttpPost("DocumentSignature")]
        public async Task<IActionResult> UploadDocumentSignature(IFormFile file)
        {
            if (file.Length == 0) return BadRequest("File size is zero.");
            var userId = CurrentUserId;

            dc.DbTran(async () =>
            {
                var storage = new FileStorageCore(dc, userId);
                var storageId = await storage.Save(file);

                var doc = new UserDocument
                {
                    UserId = userId,
                    Tag = "DocumentSignature",
                    FileStorageId = storageId
                };

                dc.Table<UserDocument>().Add(doc);
            });

            return Ok();
        }
    }
}
