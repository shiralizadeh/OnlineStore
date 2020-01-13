using OnlineStore.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace OnlineStore.Website.WebServices
{
    /// <summary>
    /// Summary description for UserWorksService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserWorksService : System.Web.Services.WebService
    {
        [WebMethod]
        public int StartTime(string username, string title)
        {
            UserWork userWork = new UserWork();

            userWork.Username = username;
            userWork.Title = title;
            userWork.StartTime = DateTime.Now;

            UserWorks.Insert(userWork);

            return userWork.ID;
        }

        [WebMethod]
        public void EndTime(int id)
        {
            var userWork = UserWorks.GetByID(id);

            userWork.EndTime = DateTime.Now;

            UserWorks.Update(userWork);
        }

        [WebMethod]
        public List<UserWork> GetLatestByUsername(string username)
        {
            return UserWorks.GetByUsername(username);
        }
    }
}
