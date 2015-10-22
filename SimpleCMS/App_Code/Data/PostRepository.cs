﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

namespace SimpleCMS.App_Code.Data
{
    public class PostRepository
    {
        private static readonly string _connectionString="CMSConnection";

        public static IEnumerable<dynamic> GetPublishedPosts()
        {
            var posts = new List<dynamic>();
            var sql = "SELECT  p.*,t.id as TagId, t.name as TagName,t.UrlFriendlyName as TagUrlFriendlyName, u.UserName " +
                    "FROM Posts p " +
                    "LEFT JOIN PostsTagsMap m ON p.Id=m.PostId " +
                    "LEFT JOIN Tags t ON t.Id=m.TagId " +
                    "INNER JOIN Users u ON u.Id=p.AuthorId " +
                    "WHERE DatePublished IS NOT NULL AND DatePublished < getdate() ";

                return DoGet(sql);
        }


        public static dynamic Get(int id)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT  p.*,t.id as TagId, t.name as TagName,t.UrlFriendlyName as TagUrlFriendlyName " +
                        "FROM Posts p " +
                        "LEFT JOIN PostsTagsMap m ON p.Id=m.PostId " +
                        "LEFT JOIN Tags t ON t.Id=m.TagId WHERE p.id=@0 ";
                
                var results= DoGet(sql,id);
                return results.Any() ? results.First(): null;
            }
        }

        public static dynamic Get(string slug)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT  p.*,t.id as TagId, t.name as TagName,t.UrlFriendlyName as TagUrlFriendlyName " +
                        "FROM Posts p " +
                        "LEFT JOIN PostsTagsMap m ON p.Id=m.PostId " +
                        "LEFT JOIN Tags t ON t.Id=m.TagId WHERE slug=@0 ";
                
                var results= DoGet(sql,slug);
                return results.Any() ? results.First(): null;
            }
        }

        public static IEnumerable<dynamic> GetAll(string orderBy=null)
        {
            var posts = new List<dynamic>();
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT  p.*,t.id as TagId, t.name as TagName,t.UrlFriendlyName as TagUrlFriendlyName " +
                        "FROM Posts p " +
                        "LEFT JOIN PostsTagsMap m ON p.Id=m.PostId " +
                        "LEFT JOIN Tags t ON t.Id=m.TagId ";

                if (!string.IsNullOrEmpty(orderBy))
                {
                    sql += " ORDER BY " + orderBy;
                }

                return DoGet(sql);

            }
        }

        public static void Add(string title, string content, string slug, DateTime? datePublished, int authorId, IEnumerable<int> tags)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "INSERT INTO Posts (Title,Content,DatePublished,AuthorId,Slug) " +
                        "VALUES (@0,@1,@2,@3,@4)";
                db.Execute(sql, title, content, datePublished,authorId, slug);

                var post=db.QuerySingle("SELECT * FROM Posts WHERE Slug=@0",slug);
;
                AddTags(post.Id,tags, db);
            }
        }
        public static void Edit(int id, string title, string content, string slug, DateTime? datePublished, int authorId, IEnumerable<int> tags)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "UPDATE Posts SET Title = @0,Content = @1,DatePublished = @2,AuthorId = @3,Slug = @4 WHERE id=@5";
                db.Execute(sql, title, content, datePublished, authorId, slug,id);
                DeleteTags(id, db);
                AddTags(id,tags, db);
            }
        }
        public static void Remove(string slug)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Posts WHERE Slug =@0 ";
                var post = db.QuerySingle(sql, slug);
                if (post == null)
                {
                    return;
                }

                DeleteTags(post.Id, db);
                sql = "DELETE FROM Posts WHERE Id=@0;";
                db.Execute(sql, post.Id);
            }
        }

        private static void AddTags(int postId,IEnumerable<int> tags, Database db)
        {
            if (!tags.Any())
            {
                return;
            }

            var sql = "INSERT INTO PostsTagsMap (PostId,TagId) VALUES (@0,@1) ";

            foreach (var tag in tags)
            {
                db.Execute(sql, postId, tag);
            }

        }

        private static void DeleteTags(int id, Database db)
        {
            var sql = "DELETE FROM PostsTagsMap WHERE PostId=@0 ";
            db.Execute(sql, id);

        }

        private static IEnumerable<dynamic> DoGet(string sql,params object  [] values)
        {
            using (var db = Database.Open(_connectionString))
            {

                var posts =new List<dynamic>();
                var results= db.Query(sql,values);

                foreach (var result in results)
                {
                    dynamic post = posts.SingleOrDefault(p => p.Id == result.Id);
                    if (post == null)
                    {
                        post = CreatePostObject(result);
                        posts.Add(post);
                    }
                    if (result.TagId == null)
                    {
                        continue;
                    }
                    dynamic tag = new ExpandoObject();
                    tag.Id = result.TagId;
                    tag.Name = result.TagName;
                    tag.UrlFriendlyName = result.TagUrlFriendlyName;

                    post.Tags.Add(tag);
                    
                }
                return posts.ToArray();
            }
            
        }



        private static dynamic CreatePostObject(dynamic obj)
        {
            dynamic post = new ExpandoObject();
            post.Id = obj.Id;
            post.Title = obj.Title;
            post.Content = obj.Content;
            post.DateCreated = obj.DateCreated;
            post.DatePublished = obj.DatePublished;
            post.AuthorId = obj.AuthorId;
            post.Slug = obj.Slug;
            post.Tags = new List<dynamic>();
            post.UserName = obj.UserName;
            return post;
        }
    }
}