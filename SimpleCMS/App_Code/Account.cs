using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Dynamic;
using System.Data.SqlClient;
using WebMatrix.Data;
using SimpleCMS.App_Code.Data;

namespace SimpleCMS.App_Code
{
    public class Account
    {
        public static WebPageRenderingBase Page
        {
            get { return WebPageContext.Current.Page; }
        }

        public static string Mode
        {
            get
            {
                if (Page.UrlData.Any())
                {
                    return Page.UrlData[0];
                }
                return string.Empty;
            }

        }
        public static string Username
        {
            get
            {
                if (Mode.ToLower() != "new")
                {
                    return Page.UrlData[1];
                }
                return string.Empty;
            }
        }

        public static dynamic Current
        {
            get
            {
                    var result = AccountRepository.Get(Username);
                    return result ?? CreateAccountObject();
               
            }
        }

        private static dynamic CreateAccountObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Id = 0;
            obj.Username = string.Empty;
            obj.Password= string.Empty;
            obj.Email = string.Empty;

            return obj;
        }
    
    }


}