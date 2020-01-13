using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineStore.UserWorks
{
    public class Utilities
    {
        public static DateTime ToEnglishDate(string persianDate)
        {
            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            Regex objRegex = new Regex(@"^(\d{0,4})/(\d{1,2})/(\d{1,2})$");

            Match objMatch = objRegex.Match(persianDate);

            if (objMatch.Success)
            {
                int year = int.Parse(objMatch.Groups[1].Value);
                int month = int.Parse(objMatch.Groups[2].Value);
                int day = int.Parse(objMatch.Groups[3].Value);

                return objPersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0); ;
            }
            else
                throw new Exception("Invalid Input.");
        }

        public static string ToPersianDate(DateTime objDateTime)
        {
            string rValue;

            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            int Year = objPersianCalendar.GetYear(objDateTime);
            int Month = objPersianCalendar.GetMonth(objDateTime);
            int Day = objPersianCalendar.GetDayOfMonth(objDateTime);

            rValue = Year.ToString();

            if (Month < 10) rValue += "/0" + Month;
            else rValue += "/" + Month;


            if (Day < 10) rValue += "/0" + Day;
            else rValue += "/" + Day;

            return rValue;
        }
    }
}
