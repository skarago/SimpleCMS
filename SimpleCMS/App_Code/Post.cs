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
    public class Post    
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
        public static string Slug
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
                    var result = PostRepository.Get(Slug);
                    return result ?? CreatePostObject();
               
            }
        }

        private static dynamic CreatePostObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Id = 0;
            obj.Title = string.Empty;
            obj.Content = string.Empty;
            obj.DateCreated = DateTime.Now;
            obj.DatePublished = null;
            obj.Slug = string.Empty;
            obj.AuthorId = null;
            obj.Tags=new List<dynamic>();
            return obj;
        }
    
    }


}