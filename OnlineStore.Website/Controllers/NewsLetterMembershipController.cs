using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.EntityFramework;

namespace OnlineStore.Website.Controllers
{
    public class NewsLetterMembershipController : Controller
    {
        [HttpPost]
        public JsonResult AddMember(string email)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                NewsLetterMember member = new NewsLetterMember
                {
                    Email = email
                };

                NewsLetterMembers.Insert(member);

                jsonSuccessResult.Success = true;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

    }
}