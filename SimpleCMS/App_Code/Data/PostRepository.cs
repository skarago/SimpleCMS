using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

namespace SimpleCMS.App_Code.Data
{
    public class PostRepository
    {
        private static readonly string _connectionString="CMSConnection";

        public static dynamic Get(int id)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Posts WHERE id=@0";

                return db.QuerySingle(sql, id);
            }
        }

        public static dynamic Get(string slug)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Posts WHERE slug=@0";

                return db.QuerySingle(sql, slug);
            }
        }

        public static IEnumerable<dynamic> GetAll(string orderBy=null)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Posts";

                if (!string.IsNullOrEmpty(orderBy))
                {
                    sql += " ORDER BY " + orderBy;
                }

                return db.Query(sql);
            }
        }

        public static void Add(string title, string content, string slug,DateTime? datePublished, int authorId)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "INSERT INTO Posts (Title,Content,DatePublished,AuthorId,Slug) " +
                        "VALUES (@0,@1,@2,@3,@4)";
                db.Execute(sql, title, content, datePublished,authorId, slug);
                
            }
        }
        public static void Edit(int id,string title, string content, string slug, DateTime? datePublished, int authorId)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "UPDATE Posts SET Title = @0,Content = @1,DatePublished = @2,AuthorId = @3,Slug = @4 " +
                        "WHERE id=@5";
                db.Execute(sql, title, content, datePublished, authorId, slug,id);

            }
        }
        public static void Remove(string slug)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "DELETE FROM Posts WHERE Slug=@0;";
                db.Execute(sql, slug);

            }
        }
    }
}