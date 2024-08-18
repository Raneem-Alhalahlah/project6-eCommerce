using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using project_6_eCommerce.Models;

namespace project_6_eCommerce.Controllers
{
    public class login_registrationController : Controller
    {
        private project6_eCommerceEntities db = new project6_eCommerceEntities();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string Username, string Email, string Password)
        {
          
            var user = new User
            {
                Username = Username,
                Email = Email,
                Password = Password
            };

            db.Users.Add(user);
            db.SaveChanges();

           
            return RedirectToAction("Login");
        }


        public ActionResult login()
        {
            return View();
        }

        // POST:Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                // تسجيل الدخول بنجاح (يمكنك إضافة ملفات تعريف الارتباط، الجلسات، إلخ)
                // هنا نعيد توجيه المستخدم إلى الصفحة الرئيسية أو صفحة أخرى بعد تسجيل الدخول
                Session["UserEmail"] = Email;
                Session["UserID"] = user.UserId;
                return RedirectToAction("product", "Shop");
                //return RedirectToAction("ProfilePage", new { id = user.UserId });
            }
            else
            {
                // رسالة خطأ في حالة عدم تطابق بيانات تسجيل الدخول
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("category", "Categories");
        }


        public ActionResult ProfilePage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);

        }

        public ActionResult EditProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilePage", new { id = user.UserId });
            }
            return View(user);
        }

       
    }
}
