using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private WebsiteBanHangDbContext db = new WebsiteBanHangDbContext();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.CountOrderSuccess = db.Orders.Where(m => m.Status == 3).Count();
            ViewBag.CountOrderCancel = db.Orders.Where(m => m.Status == 1).Count();
            ViewBag.CountContactDoneReply = db.Contacts.Where(m => m.Flag == 0).Count();
            var list = db.Orders.Where(x => x.Status == 3).Select(x => x.ID).ToList();
            ViewBag.Total = db.Orderdetails.Where(x => list.Contains(x.OrderID)).Sum(m => m.Amount);
            ViewBag.CountUser = db.Products.Where(m => m.Status != 0).Count();
            return View();
        }
    }
}