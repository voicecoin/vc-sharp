﻿using DotNetToolkit;
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

        [HttpGet("PersonalInformation")]
        public VmPersonalInfomation GetPersonalInformation()
        {
            var personal = new UserPersonalCore(dc, GetCurrentUser().Id);
            var user = personal.GetPersonalInfo();

            return user.ToObject<VmPersonalInfomation>();
        }

        [HttpPost("PersonalInformation")]
        public IActionResult UploadPersonalInformation(VmPersonalInfomation vm)
        {
            var personal = new UserPersonalCore(dc, GetCurrentUser().Id);

            dc.DbTran(() =>
            {
                personal.UpdatePersonalInfo(vm.ToObject<User>());
            });

            return Ok();
        }

        [HttpGet("Declarations")]
        public VmUserDeclarations GetDeclarations()
        {
            var verify = new UserVerifyCore(dc, GetCurrentUser().Id);
            var declares = verify.GetDeclarations();

            return new VmUserDeclarations
            {
                Declaration1 = declares.First(x => x.Tag == TagConstants.UserDeclaration1).Declaration,
                Declaration2 = declares.First(x => x.Tag == TagConstants.UserDeclaration2).Declaration,
                Declaration3 = declares.First(x => x.Tag == TagConstants.UserDeclaration3).Declaration
            };
        }

        [HttpPost("Declarations")]
        public IActionResult UploadDeclarations(VmUserDeclarations declarations)
        {
            var verify = new UserVerifyCore(dc, GetCurrentUser().Id);

            dc.DbTran(() => {
                verify.UpdateDeclarations(TagConstants.UserDeclaration1, declarations.Declaration1);
                verify.UpdateDeclarations(TagConstants.UserDeclaration2, declarations.Declaration2);
                verify.UpdateDeclarations(TagConstants.UserDeclaration3, declarations.Declaration3);
            });

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("IdDocumentTypes")]
        public IEnumerable<Object> GetIdDocumentTypes()
        {
            return dc.Table<TaxonomyTerm>()
                .Where(x => x.TaxonomyId == IdConstants.IdDocumentType)
                .Select(x => x.ToObject<VmTaxonomyTerm>())
                .OrderBy(x => x.Term)
                .ToList();
        }

        [HttpGet("IdentificationVerification")]
        public VmIdentificationVerification GetIdentificationVerification()
        {
            var identification = dc.Table<UserIdentification>().FirstOrDefault(x => x.UserId == GetCurrentUser().Id);
            return new VmIdentificationVerification
            {
                DocumentNumber = identification.DocumentNumber,
                DocumentTypeId = identification.DocumentTypeId,
                ExpiryDate = identification.ExpiryDate,
                IssueDate = identification.IssueDate 
            };
        }

        [HttpPost("IdentificationVerification")]
        public async Task<IActionResult> UploadIdentificationVerification(VmIdentificationVerification model)
        {
            dc.DbTran(async () =>
            {
                var identification = dc.Table<UserIdentification>().FirstOrDefault(x => x.UserId == GetCurrentUser().Id);
                if (identification == null)
                {
                    dc.Table<UserIdentification>().Add(new UserIdentification
                    {
                        UserId = GetCurrentUser().Id,
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

                var storage = new FileStorageCore(dc, GetCurrentUser().Id);

                if (model.FrontSidePhotoId.Length > 0)
                {
                    var storageId = await storage.Save(model.FrontSidePhotoId);

                    var doc = new UserDocument
                    {
                        UserId = GetCurrentUser().Id,
                        Tag = "FrontSidePhotoId",
                        FileStorageId = storageId
                    };

                    dc.Table<UserDocument>().Add(doc);
                }

                if (model.BackSidePhotoId.Length > 0)
                {
                    var storageId = await storage.Save(model.BackSidePhotoId);

                    var doc = new UserDocument
                    {
                        UserId = GetCurrentUser().Id,
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

            dc.DbTran(async () =>
            {
                var storage = new FileStorageCore(dc, GetCurrentUser().Id);
                var storageId = await storage.Save(file);

                var doc = new UserDocument
                {
                    UserId = GetCurrentUser().Id,
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

            dc.DbTran(async () =>
            {
                var storage = new FileStorageCore(dc, GetCurrentUser().Id);
                var storageId = await storage.Save(file);

                var doc = new UserDocument
                {
                    UserId = GetCurrentUser().Id,
                    Tag = "DocumentSignature",
                    FileStorageId = storageId
                };

                dc.Table<UserDocument>().Add(doc);
            });

            return Ok();
        }
    }
}
