using OnlineStore.DataLayer;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using OnlineStore.Models.Public;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace OnlineStore.Services
{
    public static class EmailServices
    {
        #region Email Address

        public static ViewEmail InfoMail;
        public static ViewEmail SaleMail;
        public static ViewEmail RegisterMail;
        public static ViewEmail NewsletterMail;

        #endregion Email Address

        public static void Register(string firstname, string lastname, string username, string password, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("RegisterEmail");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname)
                                           .Replace("{{Username}}", username)
                                           .Replace("{{Password}}", password);

                SendEmail(RegisterMail, email, "به آنلاین استور خوش آمدید", emailMessage, userID);
            }
        }

        public static void SuccessfullPayment(string firstname, string lastname, string saleReferenceID, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("SuccessfullPaymentEmail");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname)
                                           .Replace("{{SaleReferenceID}}", saleReferenceID);

                SendEmail(SaleMail, email, "از خرید شما متشکریم", emailMessage, userID);
            }
        }

        public static void SendToFriend(string fullName,
                                        string productLink,
                                        string productTitle,
                                        string productOtherTitle,
                                        string productImage,
                                        string friendEmail,
                                        string message,
                                        string userID,
                                        string priceBox)
        {
            var emailMessage = Utilities.ReadFileContent("~/Templates/SendToFriend.html");

            if (emailMessage != null)
            {
                emailMessage = emailMessage.Replace("{{Name}}", fullName)
                                           .Replace("{{DisplayTitle}}", productTitle)
                                           .Replace("{{OtherTitle}}", productOtherTitle)
                                           .Replace("{{Image}}", StaticValues.WebsiteUrl + productImage)
                                           .Replace("{{ProductLink}}", StaticValues.WebsiteUrl + productLink)
                                           .Replace("{{PriceBox}}", priceBox);

                if (!String.IsNullOrWhiteSpace(message))
                {
                    emailMessage = emailMessage.Replace("{{Message}}", "<div style='padding:10px;background-color:#ccc;color:#fff'>" + message + "</div>");
                }
                else
                {
                    emailMessage = emailMessage.Replace("{{Message}}", String.Empty);
                }

                emailMessage = HttpUtility.HtmlDecode(emailMessage);
                try
                {
                    SendEmail(InfoMail, friendEmail, "از طرف دوست شما!", emailMessage, userID);
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex.Message);
                }
            }
        }

        public static void ForgotPassword(string firstname, string lastname, string callbackUrl, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("ForgotPassword");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                callbackUrl = "<a href=" + callbackUrl + ">لینک بازیابی کلمه عبور</a>";

                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname)
                                           .Replace("{{CallBackUrl}}", callbackUrl);

                SendEmail(InfoMail, email, "بازیابی کلمه عبور", emailMessage, userID);
            }
        }

        public static void SendEmail(string to, string subject, string textMessage, string userID)
        {
            SendEmail(InfoMail, to, subject, textMessage, userID);
        }

        public static EmailStatus SendEmail(ViewEmail from, string toEmail, string subject, string textMessage, string userID)
        {
            if (String.IsNullOrWhiteSpace(from.Title))
            {
                from.Title = "آنلاین استور";
            }

            string body = renderEmailBody(textMessage);
            var status = EmailStatus.Delivered;

            try
            {
                var message = new MailMessage();

                message.From = new MailAddress(from.EmailAddress, from.Title, System.Text.UTF8Encoding.UTF8);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = from.EmailAddress.Trim(),
                        Password = from.Password.Trim()
                    };

                    smtp.Credentials = credential;
                    smtp.Host = "mail.online-store.com";
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                status = EmailStatus.Failed;
                textMessage += "\n Error:" + ex.Message;
            }

            var log = new EmailLog
            {
                From = StaticValues.InfoEmail,
                To = toEmail,
                Message = textMessage,
                IP = Utilities.GetIP(),
                LastUpdate = DateTime.Now,
                UserID = userID,
                Status = status,
                Key = Guid.NewGuid().ToString()
            };

            EmailLogs.Insert(log);

            return status;
        }

        private static string renderEmailBody(string textMessage)
        {
            if (!textMessage.Contains("<!DOCTYPE html>"))
            {
                textMessage = textMessage.Replace("\r\n", "<br/>");
            }

            textMessage = HttpUtility.HtmlDecode(textMessage);

            var tmp = Utilities.ReadFileContent("~/Templates/Email.html");

            string body = tmp.Replace("{{Body}}", textMessage)
                             .Replace("{{Logo}}", StaticValues.WebsiteUrl + StaticValues.Logo)
                             .Replace("{{WebsiteUrl}}", StaticValues.WebsiteUrl)
                             .Replace("{{WebsiteTitle}}", StaticValues.WebsiteTitle)
                             .Replace("{{Address}}", StaticValues.Address)
                             .Replace("{{Phone}}", StaticValues.Phone)
                             .Replace("{{Email}}", StaticValues.InfoEmail);

            return body;
        }

        public static void NotifyAdminsByEmail(AdminEmailType emailType, string message, string userID)
        {
            var subject = emailType.GetEnumDisplay();

            SendEmail("mohamad.shir@gmail.com", subject, message, userID);
            //SendEmail("s.noroozi2007@gmail.com", subject, message, userID);
        }

        public static void ProductAvailable(string productName, string[] emails)
        {
            var emailMessage = StaticContents.GetContentByName("ProductAvailable");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", productName);

                List<EmailSend> list = new List<EmailSend>();

                foreach (var item in emails)
                {
                    var emailSend = new EmailSend
                    {
                        EmailSendStatus = EmailSendStatus.NotChecked,
                        FromID = SaleMail.ID,
                        LastUpdate = DateTime.Now,
                        Priority = Priority.Medium,
                        Subject = "دعوت به خرید",
                        Text = emailMessage,
                        To = item
                    };

                    list.Add(emailSend);
                }

                EmailSends.InsertGroup(list);
            }
        }

        public static void FinancialConfirmation(string firstname, string lastname, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("FinancialConfirmation");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendEmail(SaleMail, email, "تایید مالی", emailMessage, userID);
            }
        }

        public static void SendProduct(string firstname, string lastname, string email, string userID, string billNumber)
        {
            var emailMessage = StaticContents.GetContentByName("SendProduct");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname)
                                       .Replace("{{BillNumber}}", billNumber);

                SendEmail(SaleMail, email, "ارسال کالا", emailMessage, userID);
            }
        }

        public static void CheckeProduct(string firstname, string lastname, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("CheckProduct");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendEmail(SaleMail, email, "بررسی کالا", emailMessage, userID);
            }
        }

        public static void DliverProduct(string firstname, string lastname, string email, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("DliverProduct");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendEmail(SaleMail, email, "تحویل کالا", emailMessage, userID);
            }
        }

        public static void ContactUsMessage(string email, string subject, string message, string answer, string userID)
        {
            var emailMessage = String.Format("پاسخ به: <span style='color:#546e7a;diaplay:inline-block'>{0}</span><br/>{1}", message, answer);

            EmailServices.SendEmail(email, "پاسخ به: " + subject, emailMessage, userID);

        }

        public static void DeliveryContactUsMessage(string email, string name, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("DeliveryContactUsMessage");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", name);

                SendEmail(SaleMail, email, "پیام تماس با ما", emailMessage, userID);
            }
        }

        public static void DeliveryColleagueRequest(string email, string name, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("DeliveryColleagueRequest");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", name);

                SendEmail(SaleMail, email, "دریافت اطلاعات همکاری", emailMessage, userID);
            }
        }

        public static void DeliveryEmploymentInfo(string email, string name, string userID)
        {
            var emailMessage = StaticContents.GetContentByName("DeliveryEmploymentInfo");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{Name}}", name);

                SendEmail(InfoMail, email, "پیام استخدام", emailMessage, userID);
            }
        }

        public static void ProductRequest(string email, string userID, string productName, string productLink)
        {
            var emailMessage = StaticContents.GetContentByName("ProductRequest");

            if (emailMessage != null && emailMessage != "نا مشخص")
            {
                emailMessage = emailMessage.Replace("{{ProductLink}}", String.Format("<a href='{0}' target='_blank'>{1}</a>", productLink, productName));

                SendEmail(SaleMail, email, "درخواست محصول", emailMessage, userID);
            }
        }

        public static void SendEmails(int count)
        {
            var emailSends = EmailSends.GetEmailsList(count);

            foreach (var item in emailSends)
            {
                #region Before Send

                var notCheckeds = new EmailSend();
                notCheckeds.ID = item.ID;
                notCheckeds.LastUpdate = DateTime.Now;
                notCheckeds.EmailSendStatus = EmailSendStatus.Sending;
                EmailSends.UpdateStatus(notCheckeds);

                #endregion Before Send

                var status = SendEmail(item.From, item.To, item.Subject, item.Text, null);

                #region After Send

                var afterSend = new EmailSend();
                afterSend.ID = item.ID;
                afterSend.LastUpdate = DateTime.Now;
                if (status == EmailStatus.Failed)
                {
                    afterSend.EmailSendStatus = EmailSendStatus.Failed;
                }
                else
                {
                    afterSend.EmailSendStatus = EmailSendStatus.Sent;
                }
                EmailSends.UpdateStatus(afterSend);

                #endregion After Send
            }
        }

    }
}
