using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using project_6_eCommerce.Models;

namespace project_6_eCommerce.Controllers
{
    public class ShopController : Controller
    {
        private project6_eCommerceEntities db = new project6_eCommerceEntities();

        // GET: Shop/Index
        public ActionResult product(int? categoryId)
        {
            // استعلام للحصول على جميع الفئات لعرضها في القائمة المنسدلة
            var categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");

            // تعيين الفئة المحددة في ViewBag لعرضها بشكل صحيح في الـ View
            ViewBag.SelectedCategoryId = categoryId.HasValue ? categoryId.Value.ToString() : string.Empty;

            List<Product> products;

            if (categoryId.HasValue)
            {
                // عرض المنتجات الخاصة بالفئة المحددة
                products = db.Products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            else
            {
                // عرض كل المنتجات إذا لم يتم تحديد فئة
                products = db.Products.ToList();
            }

            return View(products);
        }



        public ActionResult DetailsOfProduct(int id)
        {
            // جلب تفاصيل المنتج باستخدام الـ id
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);

            // التحقق من وجود المنتج
            if (product == null)
            {
                return HttpNotFound();
            }

            // عرض تفاصيل المنتج
            return View(product);
        }




        public ActionResult cart()
        {
            var userID = (int)Session["UserID"];
            var AllItemInCart = db.CartItems.Where(b => b.UserId == userID).ToList();
            var AllProducts = AllItemInCart.Select(x => new CartViewModel
            {
                CartItem = x,
                Product = db.Products.Find(x.ProductId)
            }).ToList();    

            return View(AllProducts);

        }

        [HttpPost]
        public ActionResult AddAllTocart()
        {
            var userID = (int)Session["UserID"];
            var products = db.Products.ToList();
            foreach (var product in products)
            {
                var Quantity = Request.Form["quantity-" + product.ProductId];
                if (int.TryParse(Quantity, out int qty) && qty > 0)
                {
                    var cartItem = db.CartItems.FirstOrDefault(x => x.UserId == userID && x.ProductId == product.ProductId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity += qty;
                    }
                    else
                    {
                        cartItem = new CartItem
                        {
                            UserId = userID,
                            ProductId = product.ProductId,
                            Quantity = qty,
                        };
                        db.CartItems.Add(cartItem);
                    }
                }



            }
            db.SaveChanges();


            return RedirectToAction("cart");

        }


        public ActionResult AddToCart(FormCollection form)
        {
            var userID = (int)Session["UserID"];
            int productIdDetails = Int32.Parse(form["ProductID"]);
            var product = db.Products.Find(productIdDetails);
            var cart = db.CartItems.FirstOrDefault(x => x.UserId == userID && x.ProductId == productIdDetails);

            if (cart != null)
            {
                cart.Quantity = Int32.Parse(form["Quantity"]);
            }
            else
            {
                cart = new CartItem
                {
                    UserId = userID,
                    ProductId = product.ProductId,
                    Quantity = Int32.Parse(form["Quantity"]),
                };
                db.CartItems.Add(cart);
            }
            db.SaveChanges();
            var counter = db.CartItems.Where(x => x.UserId == userID).Sum(x => x.Quantity);
            ViewBag.CartItemCount = counter;
            return RedirectToAction("DetailsOfProduct", new { id = productIdDetails });

        }


        [HttpPost]
        public ActionResult UpdateQuantity(int id, string operation)
        {
            var userID = (int)Session["UserID"];

            var item = db.CartItems.FirstOrDefault(x => x.UserId == userID && x.ProductId == id);

            if (item != null)
            {
                if (operation == "increase")
                {
                    item.Quantity++;
                }
                else if (operation == "decrease" && item.Quantity > 1)
                {
                    item.Quantity--;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult DeleteItem(int id)
        {
            var userID = (int)Session["UserID"];

            var item = db.CartItems.FirstOrDefault(i => i.ProductId == id && i.UserId == userID);

            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Cart");
        }


















    }
}
