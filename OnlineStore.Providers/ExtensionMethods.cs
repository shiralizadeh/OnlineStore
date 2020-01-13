using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using OnlineStore.Identity;
using System.Security.Principal;
using System.Data;
using System.Reflection;

namespace OnlineStore.Providers
{
    public static class ExtensionMethods
    {
        public const bool IsRial = false;

        public static bool IsSuccess(this WebViewPage page)
        {
            if (page.ViewData["Success"] != null)
            {
                return (bool)page.ViewData["Success"];
            }
            else
                return false;
        }

        public static int NormalizePrice(this int number)
        {
            if (!IsRial)
                number = number / 10;

            return number;
        }

        public static string ToPrice(this int number, string template = "{0} {1}")
        {
            number = NormalizePrice(number);

            return String.Format(template, (number == 0 ? "0" : number.ToString("#,#")), (IsRial ? "ریال" : "تومان"));
        }

        public static List<string> GetErrors(this WebViewPage page)
        {
            if (page.ViewData["Errors"] != null)
            {
                return (List<string>)page.ViewData["Errors"];
            }
            else
                return new List<string>();
        }

        public static string NormalizeForUrl(this string str)
        {
            if (str != null)
            {
                str = str.Trim();
                str = str.Replace(' ', '-');
                str = str.Replace(',', '-');
                str = str.Replace('،', '-');
                str = str.Replace('/', '-');
                str = str.Replace('\\', '-');
                str = str.Replace('+', '-');
                str = str.Replace('.', '-');
                str = str.Replace(':', '-');
                str = str.Replace('&', '-');

                while (str.IndexOf("--") > 0)
                {
                    str = str.Replace("--", "-");
                }

                str = str.Trim('-');
            }

            return str;
        }

        public static string NormalizeUrl(this string str)
        {
            if (str != null)
            {
                str = str.Trim('/');
            }

            return str;
        }

        public static string DeNormalizeForUrl(this string str)
        {
            str = str.Replace('-', ' ');

            return str;
        }

        public static string HtmlEncode(this string str)
        {
            str = HttpUtility.HtmlEncode(str);
            return str;
        }

        public static List<E> Shuffle<E>(this List<E> list)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (list.Count > 0)
            {
                randomIndex = r.Next(0, list.Count);
                randomList.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }

            return randomList;
        }
    }
}
