using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private WebsiteBanHangDbContext db = new WebsiteBanHangDbContext();
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Users.Where(m => m.Status == 0).Count();
            var result = from u in db.Users
                         join g in db.GroupUsers on u.GroupId equals g.ID
                         where u.Status != 0 && u.Access == 0
                         select new UserViewModel()
                         {
                             ID = u.ID,
                             Fullname = u.Fullname,
                             Email = u.Email,
                             Phone = u.Phone,
                             Address = u.Address,
                             Access = u.Access,
                             PayTotal = u.PayTotal,
                             GroupName = g.Name,
                             Status = u.Status
                         };
            return View(result.ToList());
        }
        public ActionResult IndexAdmin()
        {
            ViewBag.countTrash = db.Users.Where(m => m.Status == 0).Count();
            var result = db.Users.Where(x => x.Access != 0 && x.Status != 0).Select(u => new UserViewModel()
            {
                ID = u.ID,
                Fullname = u.Fullname,
                Email = u.Email,
                Phone = u.Phone,
                Address = u.Address,
                Status = u.Status,
                Access = u.Access
            });
            return View(result.ToList());
        }
        public ActionResult Trash()
        {
            var result = db.Users.Where(x => x.Status == 0).Select(u => new UserViewModel()
            {
                ID = u.ID,
                Fullname = u.Fullname,
                Email = u.Email,
                Status = u.Status
            });
            return View(result.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MUser mUser)
        {
            if (ModelState.IsValid)
            {
                mUser.GroupId = 7;
                mUser.Password = XString.ToMD5(mUser.Password);
                mUser.Created_at = DateTime.Now;
                mUser.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mUser.Updated_at = DateTime.Now;
                mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                mUser.Gender = 1;
                mUser.Image = "Image";

                db.Users.Add(mUser);
                db.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }

            return View(mUser);
        }


        public ActionResult DelTrash(int id)
        {
            MUser mUser = db.Users.Find(id);
            if (mUser == null)
            {
                Notification.set_flash("Không tồn tại User trên!", "warning");
                return RedirectToAction("IndexAdmin");
            }

            mUser.Status = 0;

            mUser.Created_at = DateTime.Now;
            mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã xóa vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("IndexAdmin");
        }

        // Delete to trash
        public ActionResult ReTrash(int? id)
        {
            MUser mUser = db.Users.Find(id);
            if (mUser == null)
            {
                Notification.set_flash("Không tồn tại User!", "warning");
                return RedirectToAction("Trash", "User");
            }
            mUser.Status = 2;

            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "User");
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.Users.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.Users.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MUser mUser)
        {
            if (ModelState.IsValid)
            {
                MUser us = db.Users.FirstOrDefault(x => x.ID == mUser.ID);
                mUser.Password = us.Password;
                mUser.Created_at = DateTime.Now;
                mUser.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mUser.Updated_at = DateTime.Now;
                mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                mUser.Image = "Image";
                mUser.Gender = 1;
                mUser.GroupId = 3;
                mUser.PayTotal = 0;
                db.Entry(mUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }
            return View(mUser);
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUser mUser = db.Users.Find(id);
            if (mUser == null)
            {
                return HttpNotFound();
            }
            return View(mUser);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MUser mUser = db.Users.Find(id);
            db.Users.Remove(mUser);
            db.SaveChanges();
            Notification.set_flash("Đã xóa hoàn User!", "success");
            return RedirectToAction("Trash");
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            MUser mUser = db.Users.Find(id);
            mUser.Status = (mUser.Status == 1) ? 2 : 1;

            mUser.Updated_at = DateTime.Now;
            mUser.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mUser).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = mUser.Status
            });
        }
    }
}
