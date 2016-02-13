using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;

namespace KJGarmentsWeb.Models
{
    public class Helper
    {
    }
    public class CustomProviderProvider : SimpleMembershipProvider
    {
        private MughapuEntities db = new MughapuEntities();
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return base.GetUser(username, userIsOnline);
        }

        public override bool ValidateUser(string username, string password)
        {
            var query = from c in db.User_Info
                        where c.E_mail_id == username && c.Usr_pwd == SHA1.Encode(password)
                        select c;
            if (query.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }
    }

    public class SHA1
    {
        public static string Encode(string value)
        {
            var hash = System.Security.Cryptography.SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
  
}