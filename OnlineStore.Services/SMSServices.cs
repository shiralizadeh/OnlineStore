using OnlineStore.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Services
{
    public static class SMSServices
    {
        public static void Register(string firstname, string lastname, string username, string password, string mobile, string userID)
        {
            var smsMessage = StaticContents.GetContentByName("RegisterSMS");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname)
                                       .Replace("{{Username}}", username)
                                       .Replace("{{Password}}", password);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void SuccessfullPayment(string firstname, string lastname, string saleReferenceID, string mobile, string userID)
        {
            var smsMessage = StaticContents.GetContentByName("SuccessfullPaymentSMS");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname)
                                       .Replace("{{SaleReferenceID}}", saleReferenceID);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void FinancialConfirmation(string firstname, string lastname, string mobile, string userID)
        {
            var smsMessage = StaticContents.GetContentByName("FinancialConfirmation");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void SendProduct(string firstname, string lastname, string mobile, string userID, string billNumber)
        {
            var smsMessage = StaticContents.GetContentByName("SendProduct");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname)
                                       .Replace("{{BillNumber}}", billNumber);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void CheckeProduct(string firstname, string lastname, string mobile, string userID)
        {
            var smsMessage = StaticContents.GetContentByName("CheckProduct");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void DliverProduct(string firstname, string lastname, string mobile, string userID)
        {
            var smsMessage = StaticContents.GetContentByName("DliverProduct");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", firstname + " " + lastname);

                SendSMS(mobile, smsMessage, userID);
            }
        }

        public static void ProductAvailable(string productName, string mobiles)
        {
            var smsMessage = StaticContents.GetContentByName("ProductAvailable");

            if (smsMessage != null && smsMessage != "نا مشخص")
            {
                smsMessage = smsMessage.Replace("{{Name}}", productName);

                var recievers = mobiles.Split(',');

                foreach (var item in recievers)
                {
                    SendSMS(item, smsMessage, null);
                }
            }
        }

        public static void SendSMS(string to, string message, string userID)
        {
            if (!String.IsNullOrWhiteSpace(to))
            {
                string result;

                if (to.Substring(0, 1) == "0")
                {
                    to = to.Remove(0, 1);
                }

                var sUTF8 = Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8,
                                                                Encoding.UTF8,
                                                                Encoding.UTF8.GetBytes(message)));

                Asnaf118.API_SMSServerPortTypeClient service = new Asnaf118.API_SMSServerPortTypeClient();

                try
                {
                    result = service.sendsms(StaticValues.AsnafSMSID,
                                                StaticValues.AsnafUser,
                                                StaticValues.AsnafPassword,
                                                sUTF8,
                                                to,
                                                "udh");
                }
                catch
                {
                    result = "-1";
                }

                #region Log

                var resultCode = Int64.Parse(result);

                var log = new SMSLog
                {
                    From = StaticValues.AsnafSMSID,
                    To = to,
                    Message = message,
                    UserID = userID,
                    ResultCode = resultCode,
                    IP = Utilities.GetIP(),
                    LastUpdate = DateTime.Now
                };

                SMSLogs.Insert(log);

                #endregion Log
            }
        }

        public static int SendTelegram(string to, string message)
        {
            var service = new EverySend.WebservicePortTypeClient();

            string type = "4",
                   from = "100012450",
                   username = StaticValues.EverySendUser,
                   password = StaticValues.EverySendPassword,
                   country = "98";

            var result = service.Sendsms(type, from, username, password, country, message, to, String.Empty);

            return Int32.Parse(result);
        }

    }
}
