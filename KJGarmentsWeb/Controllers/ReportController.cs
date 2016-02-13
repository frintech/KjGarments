using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.Models;
namespace KJGarmentsWeb.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult PrintRepert()
           {
               return View();

   
          }

    }
}
