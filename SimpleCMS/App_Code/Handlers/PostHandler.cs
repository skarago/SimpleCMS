using SimpleCMS.App_Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebMatrix.Data;

namespace SimpleCMS.App_Code.Handlers
{
    public class PostHandler:IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var mode = context.Request.Form["mode"];
            var title = context.Request.Form["postTitle"];
            var content = context.Request.Form["postContent"];
            var slug = context.Request.Form["postSlug"];
            var datePublished = context.Request.Form["postDatePublished"];
            var id = context.Request.Form["postId"];

            if (string.IsNullOrWhiteSpace(slug))
            {
                CreateSlug(title);
            }

            if (mode == "edit")
            {
                EditPost(Convert.ToInt32(id) ,title, content, slug, datePublished, 1);
            }
            else if (mode == "new")
            {
                CreatePost(title, content, slug, datePublished, 1);
            }
            else if (mode=="delete")
            {
                DeletePost(slug);
            }


            var result = PostRepository.Get(slug);
            
             
             context.Response.Redirect("~/Admin/post/");
        }

        public static void CreatePost(string title,string content,string slug,string datePublished,int authorId )
        {
            var result = PostRepository.Get(slug);
            DateTime? published = null;
            if (result != null)
            {
                throw new HttpException(409, "Slug is already in use.");
            }
            if (!string.IsNullOrWhiteSpace(datePublished))
            {
                published = DateTime.Parse(datePublished);
            }
            PostRepository.Add(title, content,slug,published,authorId);
        }

        public static void DeletePost(string slug)
        {
            PostRepository.Remove(slug);
        }

        public static void EditPost(int id,string title, string content, string slug, string datePublished, int authorId)
        {
            var result = PostRepository.Get(id);
            DateTime? published = null;
            if (result == null)
            {
                throw new HttpException(404, "Post does not exist.");
            }
            if (!string.IsNullOrWhiteSpace(datePublished))
            {
                published = DateTime.Parse(datePublished);
            }
            //PostRepository.Add(title, content, slug, published, authorId);
            PostRepository.Edit(id, title, content, slug, published,authorId);
        }
        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ","-");
            title = Regex.Replace(title, @"[^0-9a-z-]",string.Empty);
            return title;
        }
    }
}