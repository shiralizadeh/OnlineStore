using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineStore.Providers;
using OnlineStore.Models;
using OnlineStore.Providers.Controllers;
using OnlineStore.EntityFramework;

namespace OnlineStore.Website.Controllers
{
    public class UserWishesController : PublicController
    {
        public ActionResult Add(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (UserWishes.Exists(UserID, productID))
                    {
                        jsonSuccessResult.Data = new { Login = true, Exists = true };
                    }
                    else
                    {
                        jsonSuccessResult.Data = new { Login = true, Exists = false };

                        UserWishe userWishe = new UserWishe
                        {
                            UserID = UserID,
                            ProductID = productID,
                            LastUpdate = DateTime.Now
                        };

                        UserWishes.Insert(userWishe);
                    }
                }
                else
                    jsonSuccessResult.Data = new { Login = false, Exists = false };

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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