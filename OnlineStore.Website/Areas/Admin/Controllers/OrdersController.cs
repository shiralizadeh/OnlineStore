using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.User;
using OnlineStore.Models.Admin;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.DataLayer;
using System.Threading.Tasks;
using AutoMapper;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using OnlineStore.Services;
using OnlineStore.Identity;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class OrdersController : AdminController
    {
        protected OrderStatus _orderStatus;

        public OrdersController()
        {
            _orderStatus = OrderStatus.All;
            ViewBag.Controller = "Orders";
        }
        public ActionResult Index()
        {
            return View("/Areas/Admin/Views/Orders/Index.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex,
                                          int pageSize,
                                          string pageOrder,
                                          string userName,
                                          int? orderID,
                                          int? fromPrice,
                                          int? toPrice,
                                          string fromDate,
                                          string toDate,
                                          sbyte cartStatus,
                                          sbyte sendStatus,
                                          SendMethodType? sendMethodType,
                                          PaymentMethodType? paymentMethodType,
                                          string saleReferenceID)
        {

            DateTime? sDate = null,
                      eDate = null;

            SendStatus? sndStatus = null;
            CartStatus? crtStatus = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

            if (sendStatus != -1)
                sndStatus = (SendStatus)sendStatus;

            if (cartStatus != -1)
                crtStatus = (CartStatus)cartStatus;

            var list = Carts.Get(pageIndex,
                                 pageSize,
                                 pageOrder,
                                 orderID,
                                 fromPrice,
                                 toPrice,
                                 sDate,
                                 eDate,
                                 crtStatus,
                                 sndStatus,
                                 sendMethodType,
                                 paymentMethodType,
                                 saleReferenceID,
                                 _orderStatus);

            foreach (var item in list)
            {
                var user = (await UserManager.FindByIdAsync(item.UserID));
                if (user != null)
                {
                    item.UserName = user.UserName;
                    item.Firstname = user.Firstname;
                    item.Lastname = user.Lastname;
                }
            }

            int total = Carts.Count(orderID,
                                    fromPrice,
                                    toPrice,
                                    sDate,
                                    eDate,
                                    crtStatus,
                                    sndStatus,
                                    sendMethodType,
                                    paymentMethodType,
                                    saleReferenceID,
                                    _orderStatus);

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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var cart = Carts.GetByID(id);

            EditCart editCart = Mapper.Map<EditCart>(cart);
            editCart.SendMethodType = cart.SendMethodType;
            editCart.PaymentMethodType = cart.PaymentMethodType;
            editCart.Notes = OrderNotes.GetByCartID(id);
            editCart.OrderItems = CartItems.GetOrderItems(id);

            return View("/Areas/Admin/Views/Orders/Edit.cshtml", model: editCart);
        }

        [HttpPost]
        public ActionResult Edit(EditCart editCart, string notices)
        {
            try
            {
                var old = Carts.GetByID(editCart.ID);

                Cart cart = new Cart
                {
                    ID = editCart.ID,
                    CartStatus = editCart.CartStatus,
                    SendStatus = editCart.SendStatus,
                    SendDate = editCart.SendDate,
                    DeliveryDate = editCart.DeliveryDate,
                    ConfirmationStatus = editCart.ConfirmationStatus,
                    ConfirmationDate = editCart.ConfirmationDate,
                    BillNumber = editCart.BillNumber,
                    Description = editCart.Description,
                    LastUpdate = DateTime.Now,
                };

                SaveNotes(editCart, editCart.ID, UserID);

                ViewBag.Success = true;

                Carts.UpdateByAdmin(cart);

                #region User Info

                var user = OSUsers.GetByID(old.UserID);

                #endregion User Info

                if (notices == "on")
                {
                    #region Send Messages

                    // تاییدیه مالی
                    if (old.ConfirmationStatus != ConfirmationStatus.Approved &&
                        editCart.ConfirmationStatus == ConfirmationStatus.Approved)
                    {
                        SMSServices.FinancialConfirmation(user.Firstname, user.Lastname, user.Mobile, user.Id);
                        EmailServices.FinancialConfirmation(user.Firstname, user.Lastname, user.Email, user.Id);
                    }

                    // بررسی شده
                    if (old.SendStatus == SendStatus.NotChecked && cart.SendStatus == SendStatus.Checked)
                    {
                        SMSServices.CheckeProduct(user.Firstname, user.Lastname, user.Mobile, user.Id);
                        EmailServices.CheckeProduct(user.Firstname, user.Lastname, user.Email, user.Id);
                    }

                    // ارسال کالا
                    if (old.SendStatus == SendStatus.Checked && cart.SendStatus == SendStatus.Sent)
                    {
                        SMSServices.SendProduct(user.Firstname, user.Lastname, user.Mobile, user.Id, editCart.BillNumber);
                        EmailServices.SendProduct(user.Firstname, user.Lastname, user.Email, user.Id, editCart.BillNumber);
                    }

                    // تحویل کالا
                    if (old.SendStatus != SendStatus.Delivered && cart.SendStatus == SendStatus.Delivered)
                    {
                        SMSServices.DliverProduct(user.Firstname, user.Lastname, user.Mobile, user.Id);
                        EmailServices.DliverProduct(user.Firstname, user.Lastname, user.Email, user.Id);
                    }

                    #endregion Send Messages
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
        }

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var result = Carts.Archive(id);
                if (result == 1)
                    jsonSuccessResult.Success = true;
                else
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

        [HttpPost]
        public async Task<ActionResult> ExportToExcel(string userName,
                                          int? orderID,
                                          int? fromPrice,
                                          int? toPrice,
                                          string fromDate,
                                          string toDate,
                                          sbyte cartStatus,
                                          sbyte sendStatus,
                                          SendMethodType? sendMethodType,
                                          PaymentMethodType? paymentMethodType,
                                          string saleReferenceID)
        {
            DateTime? sDate = null,
                    eDate = null;

            SendStatus? sndStatus = null;
            CartStatus? crtStatus = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

            if (sendStatus != -1)
                sndStatus = (SendStatus)sendStatus;

            if (cartStatus != -1)
                crtStatus = (CartStatus)cartStatus;


            var dt = new System.Data.DataTable("Orders");

            var orders = Carts.Get(0,
                                -1,
                                "ID",
                                orderID,
                                fromPrice,
                                toPrice,
                                sDate,
                                eDate,
                                crtStatus,
                                sndStatus,
                                sendMethodType,
                                paymentMethodType,
                                saleReferenceID,
                                OrderStatus.All);

            dt.Columns.Add("کد سفارش", typeof(int));
            dt.Columns.Add("کد پیگیری", typeof(string));
            dt.Columns.Add("نام", typeof(string));
            dt.Columns.Add("نام خانوادگی", typeof(string));
            dt.Columns.Add("نام کاربری", typeof(string));
            dt.Columns.Add("روش ارسال", typeof(string));
            dt.Columns.Add("مبلغ کل", typeof(long));
            dt.Columns.Add("مبلغ قابل پرداخت", typeof(long));
            dt.Columns.Add("تاریخ سفارش", typeof(string));
            dt.Columns.Add("وضعیت سفارش", typeof(string));
            dt.Columns.Add("وضعیت ارسال", typeof(string));

            foreach (var item in orders)
            {
                var user = (await UserManager.FindByIdAsync(item.UserID));
                if (user != null)
                {
                    item.UserName = user.UserName;
                    item.Firstname = user.Firstname;
                    item.Lastname = user.Lastname;
                }

                dt.Rows.Add(item.ID,
                            item.SaleReferenceID,
                            item.Firstname,
                            item.Lastname,
                            item.UserName,
                            item.SendMethodType.GetEnumDisplay(),
                            item.Total,
                            item.ToPay,
                            Utilities.ToPersianDate(item.DateTime.Value),
                            item.CartStatus.GetEnumDisplay(),
                            item.SendStatus.GetEnumDisplay()
                            );
            }

            var grid = new GridView();
            grid.Font.Name = "tahoma";
            grid.Font.Size = new FontUnit(11, UnitType.Pixel);
            grid.DataSource = dt;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Prices.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }

        private static void SaveNotes(EditCart editcart, int cartID, string userID)
        {
            var curList = OrderNotes.GetByCartID(cartID);

            foreach (var note in editcart.Notes)
            {
                if (!curList.Any(item => item.ID == note.ID))
                {
                    var orderNote = Mapper.Map<OrderNote>(note);

                    orderNote.UserID = userID;
                    orderNote.CartID = cartID;
                    orderNote.LastUpdate = DateTime.Now;

                    OrderNotes.Insert(orderNote);
                }
            }
        }

    }
}