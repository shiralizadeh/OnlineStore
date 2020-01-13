using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public static class Global
    {
        private static string connectionString;
        public static string ConnectionString
        {
            get
            {
                if (String.IsNullOrWhiteSpace(connectionString))
                    return System.Configuration.ConfigurationManager.ConnectionStrings["OnlineStore"].ConnectionString;
                else
                    return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }
    }
}
