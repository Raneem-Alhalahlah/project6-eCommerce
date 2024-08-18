using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project_6_eCommerce.Models;

namespace project_6_eCommerce.Controllers
{
    public class CategoriesController : Controller
    {
        private project6_eCommerceEntities db = new project6_eCommerceEntities();

        // GET: Home
        public ActionResult category()
        {
            // الحصول على جميع الفئات من قاعدة البيانات
            var categories = db.Categories.ToList();
            return View(categories);
        }
    }
}
