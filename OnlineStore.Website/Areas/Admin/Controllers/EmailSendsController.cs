using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class EmailSendsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = EmailSends.Get(pageIndex, pageSize, pageOrder);

            int total = EmailSends.Count();
            int totalPage = (int)Math.Ceiling((decimal)total / pageSize);

            if (pageSize > total)
                pageSize = total;

            if (list.Count < pageSize)
                pageSize = list.Count;

            JsonResult result = new JsonResult()
            {
                Data = new
                {
                    TotalPages = totalPage,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Rows = list
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                EmailSends.Delete(id);
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

        public ActionResult Edit(int? id)
        {
            EmailSend email;

            if (id.HasValue)
            {
                email = EmailSends.GetByID(id.Value);
                email.Text = HttpUtility.HtmlDecode(email.Text);
            }
            else
                email = new EmailSend();

            return View(email);
        }

        [HttpPost]
        public ActionResult Edit(EmailSend email, string emailsList)
        {
            try
            {
                email.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (email.ID == -1)
                {
                    var list = new List<EmailSend>();

                    var emails = emailsList.Split('\n');

                    foreach (var item in emails)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {

                            var emailSend = new EmailSend
                            {
                                FromID = email.FromID,
                                To = item,
                                EmailSendStatus = email.EmailSendStatus,
                                Priority = email.Priority,
                                LastUpdate = DateTime.Now,
                                Subject = email.Subject,
                                Text = email.Text
                            };

                            list.Add(emailSend);
                        }
                    }

                    EmailSends.InsertGroup(list);
                    email = new EmailSend();
                }
                else
                {
                    EmailSends.Update(email);
                    email.Text = HttpUtility.HtmlDecode(email.Text);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(email);
        }

    }
}