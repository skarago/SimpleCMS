using SimpleCMS.App_Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using WebMatrix.Data;


namespace SimpleCMS.App_Code.Handlers
{
    public class PostHandler : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (WebUser.IsAuthenticated)
            {
                throw new HttpException(401,"You must login to do this.");
            }
            if (!WebUser.HasRole(UserRoles.Admin) &&
                !WebUser.HasRole(UserRoles.Editor) &&
                !WebUser.HasRole(UserRoles.Author))
            {
                throw new HttpException(401, "You do not have permission to do this.");
            }

            var mode = context.Request.Form["mode"];
            var title = context.Request.Form["postTitle"];
            var content = context.Request.Form["postContent"];
            var slug = context.Request.Form["postSlug"];
            var datePublished = context.Request.Form["postDatePublished"];
            var id = context.Request.Form["postId"];
            var postTags = context.Request.Form["postTags"] ?? string.Empty;
            var authorId = context.Request.Form["postAuthorId"];
            var tags = postTags.Split(',').Select(v => Convert.ToInt32(v));

            if ((mode == "edit" || mode=="delete") && WebUser.HasRole(UserRoles.Author))
            {
                if (WebUser.UserId != Convert.ToInt32(authorId))
                {
                    throw new HttpException(401,"You do not have permission to do that.");
                }
            }


            if (string.IsNullOrWhiteSpace(slug))
            {
               slug= CreateSlug(title);
            }

            if (mode == "edit")
            {
                EditPost(Convert.ToInt32(id) ,title, content, slug, datePublished, Convert.ToInt32(authorId),tags);
            }
            else if (mode == "new")
            {
                CreatePost(title, content, slug, datePublished, WebUser.UserId, tags);
            }
            else if (mode=="delete")
            {
                DeletePost(slug);
            }


            var result = PostRepository.Get(slug);
            
             
             context.Response.Redirect("~/Admin/post/");
        }

        public static void CreatePost(string title,string content,string slug,string datePublished,int authorId ,IEnumerable<int>tags)
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
            else 
            {
                published = DateTime.Now;
            }
            PostRepository.Add(title, content,slug,published,authorId,tags);
        }

        public static void DeletePost(string slug)
        {
            PostRepository.Remove(slug);
        }

        public static void EditPost(int id, string title, string content, string slug, string datePublished, int authorId, IEnumerable<int> tags)
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
            //PostRepository.Add(title, content, slug, published, authorId,tags);
            PostRepository.Edit(id, title, content, slug, published,authorId,tags);
        }
        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ","-");
            title = Regex.Replace(title, @"[^0-9a-z-]",string.Empty);
            return title;
        }
    }
}