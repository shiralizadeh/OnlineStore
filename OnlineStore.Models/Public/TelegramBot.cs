using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class TelegramUpdate
    {
        public long update_id { get; set; }
        public TelegramMessage message { get; set; }
    }

    public class TelegramMessage
    {
        public long message_id { get; set; }
        public TelegramUser from { get; set; }
        public TelegramChat chat { get; set; }
        public long date { get; set; }
        public string text { get; set; }
    }

    public class TelegramUser
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class TelegramChat
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }
}
