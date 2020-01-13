using Newtonsoft.Json;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class TelegramBotController : Controller
    {
        public ActionResult Index()
        {
            var sr = new StreamReader(Request.InputStream);
            var input = sr.ReadToEnd();
            sr.Close();

            var telegramUpdate = JsonConvert.DeserializeObject<TelegramUpdate>(input);

            SendMessage(telegramUpdate.message.chat.id, "به زودی منتظر ربات آنلاین استور باشید 😳");


            Logs.Alert(Utilities.GetIP(), "TelegramBot", input);
            return Content("result");
        }

        public static void SendMessage(long chat_id, string message)
        {
            WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + "248574492:AAHBKolpZw-0r3NGFOESyszXugovUbQgT0I" + "/sendMessage?chat_id=" + chat_id + "&text=" + message + "&parse_mode=Markdown");
            req.UseDefaultCredentials = true;

            var result = req.GetResponse();
            req.Abort();
        }
    }
}