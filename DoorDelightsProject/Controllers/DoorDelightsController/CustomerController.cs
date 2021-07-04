using DoorDelightsProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoorDelightsProject.Controllers.DoorDelightsController
{
    public class CustomerController : Controller
    {
        AdminRestaurantdbcontext ardbcontext = new AdminRestaurantdbcontext();
        RegistrationDbcontext rdbcontext = new RegistrationDbcontext();
        DoorDelightsEntities1 db = new DoorDelightsEntities1();
        OrdersDbcontext ordbcontext = new OrdersDbcontext();
        orderhistorydbcontext oshdbcontext = new orderhistorydbcontext();
        
        // GET: Customer
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(Registration register)
        {
            if (ModelState.IsValid)
            {
                rdbcontext.Registrations.Add(register);
                Session["Name"] = register.Username;
                rdbcontext.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Some Error Occured!");
            }
            return View(register);

        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Registration user)
        {
            var check = rdbcontext.Registrations.Where(u => u.Username.Equals(user.Username)
            && u.Password.Equals(user.Password)).FirstOrDefault();
            if (check != null)
            {
                Session["id"] = check.CustomerId.ToString();
                Session["username"] = user.Username.ToString();
                Session["Name"] = check.Username.ToString();
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
        public ActionResult VerifyEmail(Registration user)
        {
            var check = rdbcontext.Registrations.Where(u => u.Email.Equals(user.Email)).FirstOrDefault();
            if (check != null)
            {
                Session["id"] = check.CustomerId.ToString();
                Session["Email"] = user.Email.ToString();
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
            return View();
        }
        [HttpPost]
        public ActionResult newPassword(Registration user)
        {
            if (Session["Email"] != null)
            {
                user.Email = (string)Session["Email"];
                var password = rdbcontext.Registrations.Where(x => x.Email.Equals(user.Email)).FirstOrDefault();
                if (password != null)
                {
                    password.Password = user.Password;
                    rdbcontext.Entry(password).State = EntityState.Modified;
                    rdbcontext.SaveChanges();
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
            if (Session["Name"] != null)
            {
                List<Restaurant> show = new List<Restaurant>();
                using (AdminRestaurantdbcontext db = new AdminRestaurantdbcontext())
                {
                    show = db.Restaurants.ToList();
                }
                return View(show);
            }
            else
            {
                return RedirectToAction("Registration");
            }
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
        public ActionResult Customerprofile(short? id)
        {
            Registration registration = rdbcontext.Registrations.Find(id);
            return View(registration);
        }
        public ActionResult EditCustomerprofile(short?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           Registration item = rdbcontext.Registrations.Find(id);
            item.Mobile.ToString("#.###");
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        [HttpPost]
        public ActionResult EditCustomerprofile(Registration l1)
        {
            var mob = l1.Mobile.ToString("#.####");
            l1.Mobile = long.Parse(mob);
            rdbcontext.Entry(l1).State = EntityState.Modified;
            rdbcontext.SaveChanges();
            ViewBag.Notification = "Changes made successfully";
            ModelState.Clear();
            return View(l1);
        }
        public ActionResult changepassword(short? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult changepassword(Registration l1, short? id)
        {
            var check = rdbcontext.Registrations.Find(id);
            check.Password = l1.Password;
            if (l1 != null)
            {
                rdbcontext.Entry(check).State = EntityState.Modified;
                rdbcontext.SaveChanges();
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

        
        public ActionResult Cart(short?id)
        {
            var custid= Session["id"].ToString();
            FoodItem f1 = db.FoodItems.Find(id);
            Restaurant r1 = ardbcontext.Restaurants.Find(f1.RestaurantId);
            Order o1 = new Order();
            o1.CustomerId = short.Parse(custid);
            o1.DeliveryAddress = "";
            o1.Restaurantname = r1.RestaurantName;
            o1.FoodId = f1.FoodId;
            o1.Itemname = f1.Item_name;
            o1.Price = f1.Price;
            o1.itemimage = f1.Item_Image;
            o1.Quantity = 1;
            ordbcontext.Orders.Add(o1);
            ordbcontext.SaveChanges();
            return RedirectToAction("ViewFoodItems", new { id = r1.RestaurantId });
        }
        public ActionResult cartlist()
        {
            List<Order> items = ordbcontext.Orders.ToList();
            return View(items);
        }
        [HttpPost]
        public ActionResult cartlist(List<Order>items)
        {
            foreach(var item in items)
            {
                ardbcontext.Entry(item).State = EntityState.Modified;
                ardbcontext.SaveChanges();
            }
            return View(items);
        }
        public ActionResult editquantity(short?id)
        {
            Order item = ordbcontext.Orders.Find(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult editquantity(Order obj)
        {
            obj.DeliveryAddress = " ";
            ordbcontext.Entry(obj).State = EntityState.Modified;
            ordbcontext.SaveChanges();
            return RedirectToAction("cartlist");
        }

        public ActionResult orders(IEnumerable<Order>id)
        {
            foreach (var item in id)
            {
                ardbcontext.Entry(item).State = EntityState.Modified;
                ardbcontext.SaveChanges();
            }
            return View(id);
        }
        public ActionResult DeliveryAddress(short?id)
        {
            Order item = ordbcontext.Orders.Find(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult DeliveryAddress(Order item)
        {
            List<Order> items = ordbcontext.Orders.ToList();
            List<Order> temp = new List<Order>();
            foreach(var x in items )
            {
                if(x.CustomerId==item.CustomerId)
                {
                    temp.Add(x);
                }
            }
            foreach(var y in temp)
            {
                y.DeliveryAddress = item.DeliveryAddress;
                ordbcontext.Entry(y).State = EntityState.Modified;
                ordbcontext.SaveChanges();
            }
            OrderHistory history = new OrderHistory();
            history.Isconfirmed = false;
            foreach(var x in items)
            {
                if(x.CustomerId==item.CustomerId)
                {
                    history.CustomerId = x.CustomerId;
                    history.DeliveryAddress = x.DeliveryAddress;
                    history.FoodId = x.FoodId;
                    history.itemimage = x.itemimage;
                    history.Itemname = x.Itemname;
                    history.OrderId = x.OrderId;
                    history.Price = x.Price;
                    history.Quantity = x.Quantity;
                    history.Restaurantname = x.Restaurantname;
                    oshdbcontext.OrderHistories.Add(history);
                    oshdbcontext.SaveChanges();
                }
            }
            foreach(var z in temp)
            {
                Order temp1 = ordbcontext.Orders.Find(z.OrderId);
                ordbcontext.Orders.Remove(temp1);
                ordbcontext.SaveChanges();
            }
            ViewBag.Notification = "Order Placed Successfully!! Check Items on History!";
            return View(item);
        }
        public ActionResult History()
        {
            var id = Session["id"].ToString();
            List<OrderHistory> items = oshdbcontext.OrderHistories.ToList();
            List<OrderHistory> list = new List<OrderHistory>();
            foreach(var item in items)
            {
                if(item.CustomerId==short.Parse(id))
                {
                    list.Add(item);
                }
            }
            return View(list);
        }
        public ActionResult Deletecart(short?id)
        {
            Order item = ordbcontext.Orders.Find(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult Deletecart(short id)
        {
            Order item = ordbcontext.Orders.Find(id);
            ordbcontext.Orders.Remove(item);
            ordbcontext.SaveChanges();
            ViewBag.Notification = "Deleted Successfully";
            return RedirectToAction("cartlist");
        }
    }
}