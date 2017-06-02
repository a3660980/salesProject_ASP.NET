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
        [HttpGet()]
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
        public JsonResult Order()
        {
            ViewBag.EmpCodeData = this.OrderService.GetEmp();
            Models.OrderService orderService = new Models.OrderService();
            return Json(orderService.GetOrderByCondtioin());
        }

        /// <summary>
        /// 新增訂單頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult InsertOrder()
        {
            ViewBag.Emp = this.OrderService.GetEmp();
            ViewBag.Product = this.OrderService.GetProduct();
            ViewBag.Customer = this.OrderService.GetCustomer();
            Models.Order order = new Models.Order();
            //載入小計
            ViewBag.total = new double[order.OrderDetails.Count];
            for (var i = 0; i < order.OrderDetails.Count; i++)
            {
                ViewBag.total[i] = Convert.ToDouble(order.OrderDetails[i].Qty) * order.OrderDetails[i].UnitPrice * (1 - order.OrderDetails[i].Discount);
            }
            //order.OrderDetails =new List<Models.OrderDetails>();
            return View(order);
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
                TempData["ok"] = "成功新增訂單";
                return RedirectToAction("Index", new { orderid = orderid });

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

        /// <summary>
        /// 更新訂單頁面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateOrder(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Emp = this.OrderService.GetEmp();
            ViewBag.Product = this.OrderService.GetProduct();
            ViewBag.Customer = this.OrderService.GetCustomer();
            Models.OrderService orderservice = new Models.OrderService();
            Models.Order model = orderservice.GetOrderById(id);
            //載入小計
            ViewBag.total = new double[model.OrderDetails.Count];
            for (var i = 0; i < model.OrderDetails.Count; i++)
            {
                ViewBag.total[i] = Convert.ToDouble(model.OrderDetails[i].Qty) * model.OrderDetails[i].UnitPrice * (1 - model.OrderDetails[i].Discount);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Models.Order order)
        {
            Models.OrderService orderService = new Models.OrderService();


            //檢查是否驗證成功
            if (ModelState.IsValid)
            {
                orderService.UpdateOrder(order);
                TempData["ok"] = "成功更新訂單";
                return RedirectToAction("Index");

            }

            ViewBag.Emp = this.OrderService.GetEmp();
            ViewBag.Product = this.OrderService.GetProduct();
            ViewBag.Customer = this.OrderService.GetCustomer();
            return View("UpdateOrder", order);
        }

    }

}