using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace salesProject.Controllers
{
    public class OrderController : Controller
    {
        Models.OrderService OrderService = new Models.OrderService();
        /// <summary>
        /// 訂單管理首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.EmpCodeData = this.OrderService.GetEmp();
            return View();
        }

        /// <summary>
        /// 取得訂單查詢結果
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Index(Models.OrderSearchArg arg)
        {
            ViewBag.EmpCodeData = this.OrderService.GetEmp();
            Models.OrderService orderService = new Models.OrderService();
            ViewBag.SearchResult = orderService.GetOrderByCondtioin(arg);
            return View("Index");
        }

        /// <summary>
        /// 新增訂單頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult InsertOrder()
        {
            ViewBag.Emp = OrderService.GetEmp();
            ViewBag.Product = OrderService.GetProduct();
            ViewBag.Customer = OrderService.GetCustomer();
            return View(new Models.Order());
        }


        /// <summary>
        /// 新增訂單功能
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult InsertOrder(Models.Order order)
        {
            Models.OrderService orderService = new Models.OrderService();
            
            //檢查是否驗證成功
            if (ModelState.IsValid)
            {
                ViewBag.Suess = "新增成功";
                return RedirectToAction("Index");

            }
            return View(order);
          
        }

    }
}