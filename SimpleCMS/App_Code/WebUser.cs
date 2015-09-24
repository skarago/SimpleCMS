﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.SessionState;
namespace SimpleCMS.App_Code
{
    public class WebUser
    {
        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public static bool HasRole(string roleName)
        {
            var roles = GetRolesForUser();
            return roles.Contains(roleName);
        }

        public static IEnumerable<string> GetRolesForUser()

        {
            return GetRolesForUser(WebUser.UserId);
        }

        public static IEnumerable<string> GetRolesForUser(int id)
        {
            return RoleRepository.GetRolesForUser(id)
                .Select(r=>(string)r.Name);

        }

        public static bool Authenticate(string username, string password)
        {
            var user = AccountRepository.Get(username);
            if (user == null)
            {
                return false;
            }
            return Crypto.VerifyHashedPassword((string)user.Password, password);

        }

        public static void Login(string username)
        {
            var user = AccountRepository.Get(username);
            if (user == null)
            {
                return;
            }

            SetupSession(user);

        }

        public static bool AuthenticateAndLogin(string username, string password)
        {
            var user = AccountRepository.Get(username);
            if (user == null)
            {
                return false;
            }
            var hashPassword = Crypto.HashPassword(password);

            var verified = Crypto.VerifyHashedPassword(hashPassword, password);
            
            if (!verified)
            {
                return false;
            }
            SetupSession(user);
            return true;

        }

        private static void SetupSession(dynamic user)
        {
            
            Session["userid"] = (int)user.Id;
            Session["username"] = (string)user.Username;
            Session["email"] = (string)user.Email;
        }

        public static int UserId
        {
            get
            {
                var value = Session["userId"];
                if (value == null)
                {
                    return -1;
                }
                return (int)value;
            }
        }
        public static string UserName
        {
            get
            {
                var value = Session["UserName"];
                if (value == null)
                {
                    return string.Empty;
                }
                return (string) value;
            }
        }

        public static string Email
        {
            get
            {
                var value = Session["Email"];
                if (value == null)
                {
                    return string.Empty;
                }
                return (string)value;
            }
        }
        public static bool IsAuthenticated
        {
            get
            {
                return !String.IsNullOrEmpty(UserName);
            }
        }
    }


}