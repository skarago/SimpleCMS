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
    public class Tag
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
        public static string UrlFriendlyName
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
                    var result = TagRepository.Get(UrlFriendlyName);
                    return result ?? CreateTagObject();
            }
        }

        private static dynamic CreateTagObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Id = 0;
            obj.Name = string.Empty;
            obj.UrlFriendlyName = string.Empty;
            return obj;
        }
    
    }


}