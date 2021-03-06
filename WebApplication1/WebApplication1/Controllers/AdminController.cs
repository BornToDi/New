using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Auth;
using WebApplication1.Models;
using WebApplication1.Classes;
using WebApplication1.Repository;
using System.IO;

namespace WebApplication1.Controllers
{
    [AdminAccess]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult UserList()
        {
            HomeRentEntities db = new HomeRentEntities();
            var user = from u in db.Users
                       select u;

            return View(user.ToList());
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                HomeRentEntities db = new HomeRentEntities();
                user.Image = "null";
                user.Phone = 0;
                user.Address = "null";

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View();
        }

        public ActionResult ViewDetails(int id)
        {
            HomeRentEntities db = new HomeRentEntities();

            var us = (from u in db.Users
                      where u.UserId == id
                      select u).First();
            return View(us);
        }

        public ActionResult Delete(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                User us = (from u in db.Users
                           where u.UserId == id
                           select u).FirstOrDefault();
                db.Users.Remove(us);
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                User us = (from u in db.Users
                           where u.UserId == id
                           select u).FirstOrDefault();
                return View(us);
            }
        }

        [HttpPost]
        public ActionResult Edit(User u, int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                User us = (from ur in db.Users
                           where ur.UserId == id
                           select ur).FirstOrDefault();
                us.Username = u.Username;
                us.Email = u.Email;
                us.Type = u.Type;
                us.active = u.active;

                db.SaveChanges();
                return RedirectToAction("UserList");
            }


        }

        public ActionResult Block(int id)
        {

            using (HomeRentEntities db = new HomeRentEntities())
            {
                User us = (from ur in db.Users
                           where ur.UserId == id
                           select ur).FirstOrDefault();

                us.active = 0;

                db.SaveChanges();
                return RedirectToAction("UserList");
            }
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            int id = Int32.Parse(Session["id"].ToString());

            HomeRentEntities db = new HomeRentEntities();
            var user = (from us in db.Users
                        where us.UserId == id
                        select us).First();
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(UserClass u)
        {
            int id = Int32.Parse(Session["id"].ToString());

            if (ModelState.IsValid)
            {
                var user = UserRepository.EditProfile(u, id);

                ViewBag.success = "profile Edit Successfullt";
                return RedirectToAction("EditProfile");
            }
            return View();
        }


        [HttpPost]
        public ActionResult ProfilePicture(User u)
        {
            int id = Int32.Parse(Session["id"].ToString());
            string fileName = Path.GetFileNameWithoutExtension(u.ImageFile.FileName);
            string extension = Path.GetExtension(u.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            u.Image = "~/Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
            u.ImageFile.SaveAs(fileName);

            using (HomeRentEntities db = new HomeRentEntities())
            {
                var us = (from ur in db.Users
                         where ur.UserId == id
                         select ur).FirstOrDefault();
                us.Image = u.Image;
                db.SaveChanges();
            }
            ModelState.Clear();
            return RedirectToAction("EditProfile");
        }

       

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string Password, string New_Password, string con_Password)
        {
            int id = Int32.Parse(Session["id"].ToString());
            if (ModelState.IsValid)
            {
                if (Session["password"].ToString() != Password)
                {
                    ViewBag.connPass = "Current Password Doesn't match!";
                    return View();
                }
                else if (New_Password != con_Password)
                {
                    ViewBag.newpass = "New password and Confirm password Doesn't match!";
                    return View();
                }
                else
                {
                    using (HomeRentEntities db = new HomeRentEntities())
                    {
                        var pass = (from pa in db.Users
                                    where pa.UserId == id
                                    select pa).FirstOrDefault();
                        pass.Password = New_Password;
                        db.SaveChanges();
                        Session["password"] = New_Password;
                        ViewBag.Message = "Password change Successfully";
                        return View();
                    }
                }
            }
            return View();
        }

        

        // Flat 
        public ActionResult HouseList()
        {
            HomeRentEntities db = new HomeRentEntities();
            var flat = db.Flats.ToList();
            return View(flat);
        }

        public ActionResult FlatDetails(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var flat = (from f in db.Flats
                            where f.FlatId == id
                            select f).First();
                return View(flat);
            }
        }

        public ActionResult FlatDelete(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var flat = (from f in db.Flats
                            where f.FlatId == id
                            select f).First();
                db.Flats.Remove(flat);
                db.SaveChanges();

                return RedirectToAction("HouseList");
            }
        }

        [HttpGet]
        public ActionResult FlatEdit(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var flat = (from f in db.Flats
                            where f.FlatId == id
                            select f).First();

                return View(flat);
            }
        }

        [HttpPost]
        public ActionResult FlatEdit(Flat f, int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var flat = (from ft in db.Flats
                            where ft.FlatId == id
                            select ft).FirstOrDefault();

                flat.FlatSize = f.FlatSize;
                flat.Location = f.Location;
                flat.Rent = f.Rent;
                flat.FlatDetails = f.FlatDetails;

                db.SaveChanges();

                return RedirectToAction("HouseList");
            }
        }

        public ActionResult BookingList()
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var booking = (from b in db.Bookings
                               select b);

                return View(booking.ToList());
            }
        }

        public ActionResult UpdateStatus(int id)
        {
            using (HomeRentEntities db = new HomeRentEntities())
            {
                var booking = (from b in db.Bookings
                               where b.BookigId == id
                               select b).FirstOrDefault();

                booking.Status = "Booked";
                db.SaveChanges();

                return RedirectToAction("BookingList");
            }
        }


    }
}