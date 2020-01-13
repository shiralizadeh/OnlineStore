using System;

namespace OnlineStore.UserWorks
{
    internal class UserWorksResult
    {
        public UserWorksResult()
        {
        }

        public string Error { get; internal set; }
        public bool IsError { get; internal set; }
        public bool IsStart { get; internal set; }
        public DateTime Now { get; internal set; }
    }
}