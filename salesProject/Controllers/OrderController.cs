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
            int orderid = orderService.InsertOrder(order);
          
                //檢查是否驗證成功
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", new {orderid=orderid });

                }
            
            
            return View("InsertOrder", order);
          
        }

        /// <summary>
        /// 刪除訂單功能
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>json true or false</returns>
        [HttpPost]
        public JsonResult DeleteOrder(string orderId)
        {
            try
            {
                Models.OrderService orderService = new Models.OrderService();
                orderService.DeleteOrderById(orderId);
                return this.Json(true);//成功回傳TRUE
            }
            catch (Exception)
            {

                return this.Json(false);//失敗回傳FALSE
            }

        }

        [HttpGet]
        public ActionResult UpdateOrder(string orderId)
        {
            Models.OrderService orderservice = new Models.OrderService();
           // Models.Order order = orderservice.
            return View(new Models.Order());
        }

    }

}