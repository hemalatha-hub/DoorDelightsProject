using DoorDelightsProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoorDelightsProject.Controllers.DoorDelightsController
{
    public class AdminController : Controller
    {
        LoginDbContext ldbcontext = new LoginDbContext();
        AdminRestaurantdbcontext ardbcontext = new AdminRestaurantdbcontext();
        DoorDelightsEntities1 db = new DoorDelightsEntities1();
        orderhistorydbcontext ohdbcontext = new orderhistorydbcontext();
        public ActionResult history()
        {
            List<OrderHistory> items = ohdbcontext.OrderHistories.ToList();
            return View(items);
        }
        // GET: Admin
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminLogin user)
        {
            var check = ldbcontext.AdminLogins.Where(u => u.Username.Equals(user.Username)
            && u.Password.Equals(user.Password)).FirstOrDefault();
            if (check != null)
            {
                Session["id"] = check.AdminId.ToString();
                Session["username"] = user.Username.ToString();
                Session["Name"] = check.Name.ToString();
                Session["userid"] = user.AdminId.ToString();
                ModelState.Clear();
                return RedirectToAction("Menu");
            }
            else
            {
                ModelState.AddModelError("", "Wrong Username or password");
                return View("Login");
            }

        }
        public ActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyEmail(AdminLogin user)
        {
            var check = ldbcontext.AdminLogins.Where(u => u.EmailAddress.Equals(user.EmailAddress)).FirstOrDefault();
            if (check != null)
            {
                Session["id"] = check.AdminId.ToString();
                Session["Email"] = user.EmailAddress.ToString();
                ModelState.Clear();
                return RedirectToAction("newPassword");
            }
            else
            {
                ModelState.AddModelError("", "Invaild EmailAddress");
                return View();
            }

        }
        public ActionResult newPassword(int? id)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("VerifyEmail");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult newPassword(AdminLogin user)
        {
            if (Session["Email"] != null)
            {
                user.EmailAddress = (string)Session["Email"];
                var password = ldbcontext.AdminLogins.Where(x => x.EmailAddress.Equals(user.EmailAddress)).FirstOrDefault();
                if (password != null)
                {
                    password.Password = user.Password;
                    ldbcontext.Entry(password).State = EntityState.Modified;
                    ldbcontext.SaveChanges();
                    ViewBag.Notification = "Reset Password is Successful";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Something Bad Happened");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("VerifyEmail");
            }
        }
        
        public ActionResult Menu()
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Login");

            }
            else
            {
                List<Restaurant> show = new List<Restaurant>();
                using (AdminRestaurantdbcontext db = new AdminRestaurantdbcontext())
                {
                    show = db.Restaurants.ToList();
                }

                return View(show);
            }

        }
        public ActionResult AddRestaurant()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRestaurant(Restaurant register)
        {
            List<Restaurant> lists = ardbcontext.Restaurants.ToList();
            Boolean isexists = false;
            foreach(var res in lists)
            {
                if((res.RestaurantName==register.RestaurantName)&&
                    res.RestaurantLocation==register.RestaurantLocation)
                {
                    ViewBag.Notification = "Restaurant already exists ";
                    isexists = true;
                    break;
                }
            }
            if (!isexists)
            {
                string filename = Path.GetFileNameWithoutExtension(register.Imagefile.FileName);
                string extension = Path.GetExtension(register.Imagefile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                register.RestaurantImage = "~/DoorDelightsImage/" + filename;
                filename = Path.Combine(Server.MapPath("~/DoorDelightsImage/"), filename);
                register.Imagefile.SaveAs(filename);
                using (AdminRestaurantdbcontext db = new AdminRestaurantdbcontext())
                {
                    db.Restaurants.Add(register);
                    db.SaveChanges();
                }
                ViewBag.Notification = "Restaurant Is successfully created";
                ModelState.Clear();
                return View();
            }
            return View();
        }
        public ActionResult Edit(short? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = ardbcontext.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Restaurant register, HttpPostedFileBase Imagefile)
        {
            if (Imagefile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(register.Imagefile.FileName);
                string extension = Path.GetExtension(register.Imagefile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                register.RestaurantImage = "~/DoorDelightsImage/" + filename;
                filename = Path.Combine(Server.MapPath("~/DoorDelightsImage/"), filename);
                register.Imagefile.SaveAs(filename);
                ardbcontext.Entry(register).State = EntityState.Modified;
                ardbcontext.SaveChanges();
                ViewBag.Notification = "Changes made successfully";
                ModelState.Clear();
               
                return View(register);
            }
            else
            {
                ardbcontext.Entry(register).State = EntityState.Modified;
                ardbcontext.SaveChanges();
                ViewBag.Notification = "Changes made successfully";
                return View(register);
            }
        }
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = ardbcontext.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = ardbcontext.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: crud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Restaurant restaurant = ardbcontext.Restaurants.Find(id);
            ardbcontext.Restaurants.Remove(restaurant);
            ardbcontext.SaveChanges();
            List<FoodItem> item = db.FoodItems.Where(x => x.RestaurantId.Equals(id)).ToList();
            foreach (var items in item)
            {
                db.FoodItems.Remove(items);
            }
            db.SaveChanges();
            ViewBag.Notification = "Deleted Restaurant successfully";
            return RedirectToAction("Menu");
        }

        public ActionResult AddFooditems()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFooditems(FoodItem register, short id)
        {
            List<FoodItem> itemlists = db.FoodItems.ToList();
            Boolean isexists = false;
            foreach(var item in itemlists)
            {
                if((item.RestaurantId==id)&& item.Item_name==register.Item_name)
                {
                    ViewBag.Notification = "Item already exists in the Restaurant ";
                    isexists = true;
                    break;
                }
            }
            if (!isexists)
            {
                string filename = Path.GetFileNameWithoutExtension(register.Imagefile.FileName);
                string extension = Path.GetExtension(register.Imagefile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                register.Item_Image = "~/DoorDelightsImage/" + filename;
                filename = Path.Combine(Server.MapPath("~/DoorDelightsImage/"), filename);
                register.Imagefile.SaveAs(filename);
                register.RestaurantId = id;
                using (DoorDelightsEntities1 db1 = new DoorDelightsEntities1())
                {
                    db1.FoodItems.Add(register);
                    db1.SaveChanges();
                }
                ViewBag.Notification = "Food Item is added successfully";
                ModelState.Clear();
                return View();
            }
            return View();
        }

        public ActionResult ViewFoodItems(short? id)
        {
            DoorDelightsEntities1 db = new DoorDelightsEntities1();
            AdminRestaurantdbcontext rdb = new AdminRestaurantdbcontext();
            List<Restaurant> restaurantsshow = rdb.Restaurants.ToList();
            List<FoodItem> show = db.FoodItems.ToList();
            List<FoodItem> show1 = new List<FoodItem>();

            foreach (FoodItem i1 in show)
            {
                if (i1.RestaurantId == id)
                {
                    show1.Add(i1);
                }
            }
            Session["restaurantId"] = id.ToString();
            return View(show1);
        }

        public ActionResult EditFooditems(short? id, short? resid)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem item = db.FoodItems.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFooditems(FoodItem register, HttpPostedFileBase Imagefile, short? resid)
        {
            if (Imagefile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(register.Imagefile.FileName);
                string extension = Path.GetExtension(register.Imagefile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                register.Item_Image = "~/DoorDelightsImage/" + filename;
                filename = Path.Combine(Server.MapPath("~/DoorDelightsImage/"), filename);
                register.Imagefile.SaveAs(filename);
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notification = "Changes made successfully";
                ModelState.Clear();
               
                return View(register);
            }
            else
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notification = "Changes made Successfully";
                ModelState.Clear();

                return View(register);
            }
        }

        public ActionResult FooditemDetails(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem item = db.FoodItems.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        public ActionResult DeleteFooditem(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem item = db.FoodItems.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFooditem(short id)
        {
            FoodItem item = db.FoodItems.Find(id);
            short resid = item.RestaurantId;
            db.FoodItems.Remove(item);
            db.SaveChanges();
            ViewBag.Notification = " Item Deleted";
            return RedirectToAction("ViewFoodItems",new { @id = id }) ;
        }
        public ActionResult Adminprofile(short? id)
        {
            AdminLogin login = ldbcontext.AdminLogins.Find(id);
            return View(login);
        }
        public ActionResult EditProfile(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminLogin item = ldbcontext.AdminLogins.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        [HttpPost]
        public ActionResult EditProfile(AdminLogin l1)
        {
            var mob = l1.Mobile_No.ToString("#.####");
            l1.Mobile_No = long.Parse(mob);
            ldbcontext.Entry(l1).State = EntityState.Modified;
           ldbcontext.SaveChanges();
            ViewBag.Notification = "Changes made successfully";
            ModelState.Clear();
            return View(l1);
        }
       public ActionResult changepassword(short? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult changepassword(AdminLogin l1,short?id)
        {
            var check = ldbcontext.AdminLogins.Find(id);
            check.Password = l1.Password;
            if (l1 != null)
            { 
                ldbcontext.Entry(check).State = EntityState.Modified;
                ldbcontext.SaveChanges();
                ViewBag.Notification = "Password is Successful changed";
                ModelState.Clear();
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Something Bad Happened");
                return View();
            }
            
        }
        public ActionResult Logout()
        {
            Session.Contents.RemoveAll();
            return RedirectToAction("Index", "start");
        }
    }
}