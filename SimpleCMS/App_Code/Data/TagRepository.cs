using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

namespace SimpleCMS.App_Code.Data
{
    public class TagRepository
    {
        private static readonly string _connectionString="CMSConnection";

        public static dynamic Get(int id)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Tags WHERE id=@0 ";

                return db.QuerySingle(sql, id);
            }
        }

        public static dynamic Get(string friendlyName)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Tags WHERE UrlFriendlyName=@0 ";

                return db.QuerySingle(sql, friendlyName);
            }
        }

        public static IEnumerable<dynamic> GetAll(string orderBy=null,string where=null)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "SELECT * FROM Tags";

                if (!string.IsNullOrEmpty(where))
                {
                    sql += "WHERE " + where;
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    sql += " ORDER BY " + orderBy;
                }

                return db.Query(sql);
            }
        }

        public static void Add(string title, string frieldnyName )
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "INSERT INTO Tags (name,urlfriendlyname) " +
                        "VALUES (@0,@1)";
                db.Execute(sql, title, frieldnyName);
                
            }
        }
        public static void Edit(int id,string name,string friendlyName)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "UPDATE Tags SET Name = @0,UrlFriendlyName =@1 " +
                        "WHERE id=@2";
                db.Execute(sql, name, friendlyName,id);

            }
        }
        public static void Remove(string friendlyName)
        {
            using (var db = Database.Open(_connectionString))
            {
                var sql = "DELETE FROM Tags WHERE UrlFriendlyName=@0 ";
                db.Execute(sql, friendlyName);

            }
        }
    }
}