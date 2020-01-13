using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Providers
{
    public static class Utilities
    {
        static Utilities()
        {
            reverseChars.Add('q', 'ض');
            reverseChars.Add('w', 'ص');
            reverseChars.Add('e', 'ث');
            reverseChars.Add('r', 'ق');
            reverseChars.Add('t', 'ف');
            reverseChars.Add('y', 'غ');
            reverseChars.Add('u', 'ع');
            reverseChars.Add('i', 'ه');
            reverseChars.Add('o', 'خ');
            reverseChars.Add('p', 'ح');
            reverseChars.Add('[', 'ج');
            reverseChars.Add(']', 'چ');
            reverseChars.Add('a', 'ش');
            reverseChars.Add('s', 'س');
            reverseChars.Add('d', 'ی');
            reverseChars.Add('f', 'ب');
            reverseChars.Add('g', 'ل');
            reverseChars.Add('h', 'ا');
            reverseChars.Add('j', 'ت');
            reverseChars.Add('k', 'ن');
            reverseChars.Add('l', 'م');
            reverseChars.Add(';', 'ک');
            reverseChars.Add('\'', 'گ');
            reverseChars.Add('z', 'ظ');
            reverseChars.Add('x', 'ط');
            reverseChars.Add('c', 'ز');
            reverseChars.Add('v', 'ر');
            reverseChars.Add('b', 'ذ');
            reverseChars.Add('n', 'د');
            reverseChars.Add('m', 'ئ');
            reverseChars.Add(',', 'و');
        }

        private static char[] invalidFileChars = new char[] { '\\', '/', ':', '*', '\'', '?', '"', '<', '>', '|', '[', ']', '+', '%' };

        public class FileUploadSettings
        {
            public string Title { get; set; }
            public string FileUpload { get; set; }
        }

        public static List<FileUploadSettings> SaveFiles(HttpFileCollectionBase files, string filename, string path)
        {
            var tmpFilename = filename;
            var tmpPath = path;

            Random rnd = new Random();
            var list = new List<FileUploadSettings>();

            foreach (string item in files)
            {
                var file = files[item];

                string ext = Path.GetExtension(file.FileName);
                filename = filename + "_" + rnd.Next(1000, 9999) + ext;

                path = System.Web.Hosting.HostingEnvironment.MapPath(path) + filename;

                if (file.ContentLength > 0)
                {
                    file.SaveAs(path);
                    list.Add(new FileUploadSettings()
                    {
                        Title = filename,
                        FileUpload = item,
                    });
                }

                filename = tmpFilename;
                path = tmpPath;

            }

            return list;
        }

        public static string GetNormalFileName(string fileName)
        {
            foreach (char ch in invalidFileChars)
            {
                fileName = fileName.Replace(ch, '-');
            }

            fileName = fileName.Replace(' ', '-');

            while (fileName.IndexOf("--") != -1)
            {
                fileName = fileName.Replace("--", "-");
            }

            return fileName;
        }

        public static string ReadFileContent(string url)
        {
            string html = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(url));
            return html;
        }

        public static string ToPersianDate(DateTime objDateTime)
        {
            string rValue;

            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            int Year = objPersianCalendar.GetYear(objDateTime);
            int Month = objPersianCalendar.GetMonth(objDateTime);
            int Day = objPersianCalendar.GetDayOfMonth(objDateTime);
            int Hour = objPersianCalendar.GetHour(objDateTime);
            int Minute = objPersianCalendar.GetMinute(objDateTime);
            int Second = objPersianCalendar.GetSecond(objDateTime);

            rValue = Year.ToString();

            if (Month < 10) rValue += "/0" + Month;
            else rValue += "/" + Month;


            if (Day < 10) rValue += "/0" + Day;
            else rValue += "/" + Day;

            if (Hour > 0)
            {
                rValue += " ";
                if (Hour < 10) rValue += "0" + Hour;
                else rValue += Hour;

                if (Minute < 10) rValue += ":0" + Minute;
                else rValue += ":" + Minute;

                if (Second < 10) rValue += ":0" + Second;
                else rValue += ":" + Second;

            }

            return rValue;
        }

        public static DateTime ToEnglishDate(string persianDate)
        {
            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            Regex objRegex = new Regex(@"^(\d{0,4})/(\d{1,2})/(\d{1,2})(?:\s+(\d\d):(\d\d):(\d\d))?$");

            Match objMatch = objRegex.Match(persianDate);

            if (objMatch.Success)
            {
                int year = int.Parse(objMatch.Groups[1].Value);
                int month = int.Parse(objMatch.Groups[2].Value);
                int day = int.Parse(objMatch.Groups[3].Value);

                if (persianDate.Length > 10)
                {
                    int hour = int.Parse(objMatch.Groups[4].Value);
                    int min = int.Parse(objMatch.Groups[5].Value);
                    int sec = int.Parse(objMatch.Groups[6].Value);

                    return objPersianCalendar.ToDateTime(year, month, day, hour, min, sec, 0);
                }
                else
                {
                    return objPersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0); ;
                }

            }
            else
                throw new Exception("Invalid Input.");
        }

        public static string ToPersianDateString(DateTime date, bool includeTime = false)
        {
            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            DateTime objDateTime = date;

            int Year = objPersianCalendar.GetYear(objDateTime);
            int Month = objPersianCalendar.GetMonth(objDateTime);
            string strMonth = "";
            DayOfWeek WeekDay = objPersianCalendar.GetDayOfWeek(objDateTime);
            string strWeekDay = "";
            int Day = objPersianCalendar.GetDayOfMonth(objDateTime);

            switch (Month)
            {
                case 1:
                    strMonth = "فروردین";
                    break;
                case 2:
                    strMonth = "اردیبهشت";
                    break;
                case 3:
                    strMonth = "خرداد";
                    break;
                case 4:
                    strMonth = "تیر";
                    break;
                case 5:
                    strMonth = "مرداد";
                    break;
                case 6:
                    strMonth = "شهریور";
                    break;
                case 7:
                    strMonth = "مهر";
                    break;
                case 8:
                    strMonth = "آبان";
                    break;
                case 9:
                    strMonth = "آذر";
                    break;
                case 10:
                    strMonth = "دی";
                    break;
                case 11:
                    strMonth = "بهمن";
                    break;
                case 12:
                    strMonth = "اسفند";
                    break;
                default:
                    break;
            }

            switch (WeekDay)
            {
                case DayOfWeek.Friday:
                    strWeekDay = "جمعه";
                    break;
                case DayOfWeek.Monday:
                    strWeekDay = "دوشنبه";
                    break;
                case DayOfWeek.Saturday:
                    strWeekDay = "شنبه";
                    break;
                case DayOfWeek.Sunday:
                    strWeekDay = "یک شنبه";
                    break;
                case DayOfWeek.Thursday:
                    strWeekDay = "پنج شنبه";
                    break;
                case DayOfWeek.Tuesday:
                    strWeekDay = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    strWeekDay = "چهار شنبه";
                    break;
                default:
                    break;
            }

            return strWeekDay + " " + Day + " " + strMonth + " " + Year + (includeTime ? " ساعت " + date.ToString("HH:mm") : "");
        }

        public static string ToPersianDateTime(DateTime objDateTime)
        {
            string rValue;

            System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();

            int Year = objPersianCalendar.GetYear(objDateTime);
            int Month = objPersianCalendar.GetMonth(objDateTime);
            int Day = objPersianCalendar.GetDayOfMonth(objDateTime);
            int Hour = objPersianCalendar.GetHour(objDateTime);

            rValue = Year.ToString();

            if (Month < 10) rValue += "/0" + Month;
            else rValue += "/" + Month;


            if (Day < 10) rValue += "/0" + Day;
            else rValue += "/" + Day;

            if (Hour >= 12)
            {
                rValue += " عصر";
            }
            else
            {
                rValue += " صبح";
            }

            return rValue;
        }

        public static List<int> MakePaging(int totalPages, int pageIndex)
        {
            var paging = new List<int>();
            int i, j, k;

            i = pageIndex;
            j = pageIndex - 1;
            k = pageIndex + 1;

            while (j != 0 && j != i - 6)
            {
                paging.Add(j);
                j--;
            }

            paging.Reverse();

            paging.Add(i);

            for (; k < totalPages + 1 && k < i + 6; k++)
            {
                paging.Add(k);
            }

            return paging;
        }

        public static int DiscountPrice(float percent, int price)
        {
            return price - (int)((percent / 100.0) * price);
        }

        public static float CalcRaty(float sum, int count, float adminRate)
        {
            if (count == 0 && adminRate == 0)
            {
                return 0;
            }

            float result = (sum + adminRate) / (count + 1);

            if (result > 5)
            {
                result = 5;
            }

            return result;
        }

        public static string GetEnumDisplay(this Enum en)
        {
            var type = en.GetType();

            var display = ((DisplayAttribute)type.GetMember(en.ToString()).First().GetCustomAttributes(false).First());

            return display.Name;
        }

        public static string GetIP()
        {
            return (HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0");
        }

        #region MellatBank Methods

        public static string MellatBankResult(string resCode)
        {
            var result = String.Empty;
            var errorMessage = "عملیات با خطا مواجه شد";

            switch (resCode)
            {
                case "0": result = "با تشکر از خرید شما، خرید شما با موفقیت انجام شد."; break;
                case "11": result = errorMessage + "، شماره کارت نامعتبر است"; break;
                case "12": result = errorMessage + "، موجودی کافی نیست."; break;
                case "13": result = errorMessage + "، رمز نادرست است."; break;
                case "14": result = errorMessage + "، تعداد دفعات وارد کردن رمز بیش از حد مجاز است."; break;
                case "15": result = errorMessage + "، کارت نامعتبر است."; break;
                case "16": result = errorMessage + "، دفعات برداشت وجه بیش از حد مجاز است."; break;
                case "17": result = "مشتری گرامی شما از انجام تراکنش منصرف شده اید."; break;
                case "18": result = errorMessage + "، تاریخ انقضای کارت گذشته است."; break;
                case "19": result = errorMessage + "، مبلغ برداشت وجه بیش از حد مجاز است."; break;
                case "51": result = errorMessage + "، تراکنش تکراری است."; break;
                case "55": result = errorMessage + "، تراکنش نامعتبر است."; break;
                default: result = errorMessage; break;
            }

            return result;
        }

        #endregion MellatBank Methods


        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }

        private static Dictionary<char, char> reverseChars = new Dictionary<char, char>();
        public static string GetReversed(this string key, bool isFa)
        {
            string result = key;

            foreach (var item in reverseChars)
            {
                if (!isFa)
                    result = result.Replace(item.Key, item.Value);
                else
                    result = result.Replace(item.Value, item.Key);
            }

            return result;
        }

        public static Size GetImageSize(string path)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(path));

                return new Size(img.Width, img.Height);
            }
            catch (Exception ex)
            {
                return new Size(64, 64);
            }
        }

        public static string GetColorByOptionID(int optionID)
        {
            var result = String.Empty;

            switch (optionID)
            {
                case 21: // سفید
                    result = "#fff";
                    break;
                case 22: // مشکی
                    result = "#303031";
                    break;
                case 23: // نوک مدادی
                    result = "#555555";
                    break;
                case 53: // طلایی
                    result = "#ffb400";
                    break;
                case 64: // رزگلد
                    result = "#ffbdbd";
                    break;
                case 65: // قرمز
                    result = "#ff3636";
                    break;
                case 66: // نارنجی
                    result = "#ff6a00";
                    break;
                case 67: // زرشکی
                    result = "#bb4242";
                    break;
                case 68: // نقره ای
                    result = "#a9a9a9";
                    break;
                case 69: // سورمه ای
                    result = "#001578";
                    break;
                case 70: // صورتی
                    result = "#bf0bc1";
                    break;
                case 93: // آبی
                    result = "#0094ff";
                    break;
                case 97: // Gold on Gold
                    result = "#ffd800";
                    break;
                case 123: // Rose Gold
                    result = "#ffaea2";
                    break;
                case 124: // Gold Sepia
                    result = "#ffd300";
                    break;
                case 335: // زرد
                    result = "#ffea00";
                    break;
                case 336: // نارنجی
                    result = "#ff6a00";
                    break;
                case 1352: // Gold Silver
                    result = "#e6df88";
                    break;
                case 1353: // سفید دور قرمز
                    result = "#ededed";
                    break;
                case 1355: // Gray
                    result = "#cccccc";
                    break;
                default:
                    result = "#fff";
                    break;
            }

            return result;
        }

        public static string ToLinks(this string text)
        {
            string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);

            return r.Replace(text, "<a href=\"$1\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
        }

    }
}
