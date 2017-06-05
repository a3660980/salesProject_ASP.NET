using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;


namespace salesProject.Models
{
    public class OrderService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        /// <summary>
        /// 取得員工資料
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetEmp()
        {
            DataTable dt = new DataTable();
            string sql = @"Select EmployeeID As CodeId,Lastname +'-'+ Firstname As CodeName FROM HR.Employees";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);

                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);
        }

        /// <summary>
        /// 取得產品資料
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetProduct()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT Production.Products.ProductID AS CodeId,Production.Products.ProductName AS CodeName FROM Production.Products;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);

        }

        /// <summary>
        /// 取得客戶資料
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCustomer()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT Sales.Customers.CustomerID AS CodeId,Sales.Customers.CompanyName AS CodeName FROM Sales.Customers;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);

        }



        /// <summary>
        /// Maping 代碼資料
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapCodeData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();


            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["CodeName"].ToString(),
                    Value = row["CodeId"].ToString()
                });
            }
            return result;
        }

        /// <summary>
        /// 用orderid取得資料
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>order</returns>
        public Models.Order GetOrderById(string OrderId)
        {
            DataTable dt = new DataTable();
            DataTable dtDetails = new DataTable();
            string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CustName,
					A.EmployeeID,C.lastname+ C.firstname As EmpName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.shipperid=D.shipperid
					Where  A.OrderId=@OrderId";
            string details = @"SELECT * FROM Sales.OrderDetails WHERE OrderID = @OrderID;";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderId", OrderId == null ? string.Empty : OrderId));
                    sqlAdapter = new SqlDataAdapter(cmd);
                    sqlAdapter.Fill(dt);
                }

                using (SqlCommand cmd = new SqlCommand(details, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderId", OrderId == null ? string.Empty : OrderId));
                    sqlAdapter = new SqlDataAdapter(cmd);
                    sqlAdapter.Fill(dtDetails);
                }


                conn.Close();
            }


            return this.MapOrderDataToList(dt, dtDetails).FirstOrDefault();
        }


        /// <summary>
		/// 依照條件取得訂單資料
		/// </summary>
		/// <returns></returns>
		public List<Models.Order> GetOrderByCondtioin(Models.OrderSearchArg arg)
        {

            DataTable dt = new DataTable();
              string sql = @"SELECT 
                      A.OrderId,A.CustomerID,B.Companyname As CustName,
                      A.EmployeeID,C.lastname+ C.firstname As EmpName,
                      A.Orderdate,A.RequireDdate,A.ShippedDate,
                      A.ShipperId,D.companyname As ShipperName,A.Freight,
                      A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
                      From Sales.Orders As A 
                      INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
                      INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
                      inner JOIN Sales.Shippers As D ON A.shipperid=D.shipperid
                      Where (A.OrderId Like '%' + @OrderId + '%' Or @OrderId='' ) And  (B.Companyname Like '%' + @CustName + '%' Or @CustName='') And 
                            (A.Orderdate=@Orderdate Or @Orderdate='') And (A.EmployeeID=@EmployeeID Or @EmployeeID='')  ";
              
           /* string sql = @"SELECT 
					A.OrderId,A.CustomerID,B.Companyname As CustName,
					A.EmployeeID,C.lastname+ C.firstname As EmpName,
					A.Orderdate,A.RequireDdate,A.ShippedDate,
					A.ShipperId,D.companyname As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.shipperid=D.shipperid;";
                */

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", arg.EmployeeID == null ? string.Empty : arg.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@OrderId", arg.OrderId == null ? string.Empty : arg.OrderId));
                cmd.Parameters.Add(new SqlParameter("@CustName", arg.CustName == null ? string.Empty : arg.CustName));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", arg.OrderDate == null ? string.Empty : arg.OrderDate));
                
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }


            return this.MapOrderDataToList(dt, null);
        }


        private List<Models.Order> MapOrderDataToList(DataTable orderData, DataTable dtDetails)
        {
            List<Models.Order> result = new List<Order>();
            List<Models.OrderDetails> Details = new List<OrderDetails>();
            if (dtDetails != null)
            {
                foreach (DataRow row in dtDetails.Rows)
                {
                    Details.Add(new OrderDetails()
                    {
                        OrderId = Convert.ToInt32(row["OrderID"]),
                        ProductId = Convert.ToString(row["ProductID"]),
                        UnitPrice = Convert.ToInt32(row["UnitPrice"]),
                        Qty = Convert.ToDecimal(row["Qty"]),
                        Discount = Convert.ToDouble(row["Discount"])
                    });
                }
            }

            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new Order()
                {
                    CustomerID = row["CustomerID"].ToString(),
                    CustName = row["CustName"].ToString(),
                    EmployeeID = (int)row["EmployeeID"],
                    EmpName = row["EmpName"].ToString(),
                    Freight = (decimal)row["Freight"],
                    Orderdate = row["Orderdate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["Orderdate"],
                    OrderId = (int)row["OrderId"],
                    RequireDdate = row["RequireDdate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["RequireDdate"],
                    ShipAddress = row["ShipAddress"].ToString(),
                    ShipCity = row["ShipCity"].ToString(),
                    ShipCountry = row["ShipCountry"].ToString(),
                    ShipName = row["ShipName"].ToString(),
                    ShippedDate = row["ShippedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["ShippedDate"],
                    ShipperId = (int)row["ShipperId"],
                    ShipperName = row["ShipperName"].ToString(),
                    ShipPostalCode = row["ShipPostalCode"].ToString(),
                    ShipRegion = row["ShipRegion"].ToString(),
                    OrderDetails = Details
                });
            }

            return result;
        }


        /// <summary>
        /// 新增訂單處理
        /// </summary>
        /// <param name=""></param>
        /// <returns>訂單編號</returns>
        public int InsertOrder(Models.Order order)
        {

            string sql = @" Insert INTO Sales.Orders
						 (
							CustomerID,EmployeeID,orderdate,requireddate,shippeddate,shipperid,freight,
							shipname,shipaddress,shipcity,shipregion,shippostalcode,shipcountry
						)
						VALUES
						(
							@CustomerID,@EmployeeID,@orderdate,@requireddate,@shippeddate,@shipperid,@freight,
							@shipname,@shipaddress,@shipcity,@shipregion,@shippostalcode,@shipcountry
						)
					    select @@IDENTITY
						";

            string orderDetialSQL = @"Insert INTO Sales.OrderDetails
                                      (
                                        OrderID,ProductID,UnitPrice,Qty,Discount   
                                      )
                                    VALUES
                                    (
                                        @OrderID,@ProductID,@UnitPrice,@Qty,@Discount
                                    )";
            int orderId;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))

            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction("InsertOrder");
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Transaction = transaction;
                        cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                        cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                        cmd.Parameters.Add(new SqlParameter("@orderdate", order.Orderdate));
                        cmd.Parameters.Add(new SqlParameter("@requireddate", order.RequireDdate));
                        cmd.Parameters.Add(new SqlParameter("@shippeddate", order.ShippedDate));
                        cmd.Parameters.Add(new SqlParameter("@shipperid", order.ShipperId));
                        cmd.Parameters.Add(new SqlParameter("@freight", order.Freight));
                        cmd.Parameters.Add(new SqlParameter("@shipname", order.ShipperName));
                        cmd.Parameters.Add(new SqlParameter("@shipaddress", order.ShipAddress));
                        cmd.Parameters.Add(new SqlParameter("@shipcity", order.ShipCity));
                        cmd.Parameters.Add(new SqlParameter("@shipregion", order.ShipRegion));
                        cmd.Parameters.Add(new SqlParameter("@shippostalcode", order.ShipPostalCode));
                        cmd.Parameters.Add(new SqlParameter("@shipcountry", order.ShipCountry));

                        orderId = Convert.ToInt32(cmd.ExecuteScalar()); ///回傳新增訂單的orderId並使用Convert轉型
                    }

                    foreach (var OrderDetail in order.OrderDetails)
                    {
                        using (SqlCommand cmd = new SqlCommand(orderDetialSQL, conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));
                            cmd.Parameters.Add(new SqlParameter("@ProductID", OrderDetail.ProductId));
                            cmd.Parameters.Add(new SqlParameter("@UnitPrice", OrderDetail.UnitPrice));
                            cmd.Parameters.Add(new SqlParameter("@Qty", OrderDetail.Qty));
                            cmd.Parameters.Add(new SqlParameter("@Discount", OrderDetail.Discount));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception Exception)
                {
                    transaction.Rollback();
                    throw Exception;
                }
                finally
                {
                    conn.Close();
                }


            }



            return orderId;

        }

        /// <summary>
		/// 刪除訂單
		/// </summary>
		public void DeleteOrderById(string orderId)
        {
            try
            {
                string sql = "Delete FROM Sales.Orders Where orderid=@orderid";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@orderid", orderId));
                    cmd.ExecuteNonQuery();///回傳0,-1
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateOrder(Models.Order Order)
        {
            string UpdateOrderSql = @"UPDATE Sales.Orders
                                        SET CustomerID = @CustomerID, EmployeeID = @EmployeeID, OrderDate = @OrderDate,
                                        RequiredDate = @RequiredDate, ShippedDate = @ShippedDate, ShipperID = @ShipperID,
                                        Freight = @Freight, ShipName = @ShipName, ShipAddress = @ShipAddress, ShipCity = @ShipCity,
                                        ShipRegion = @ShipRegion, ShipPostalCode = @ShipPostalCode, ShipCountry = @ShipCountry
                                        WHERE OrderID = @OrderID;";
            string DeleteOrderDetailsSql = @"DELETE FROM Sales.OrderDetails
                                             Where  OrderID = @OrderID;";
            string InsertNewOrderDetailsSql = @"Insert INTO Sales.OrderDetails
                                             (
                                                OrderID,ProductID,UnitPrice,Qty,Discount   
                                               )
                                              VALUES
                                               (
                                                 @OrderID,@ProductID,@UnitPrice,@Qty,@Discount
                                                  )";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction("UpdateOrder");
                try
            {
                using (SqlCommand cmd = new SqlCommand(UpdateOrderSql, conn))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new SqlParameter("@CustomerID", Order.CustomerID));
                    cmd.Parameters.Add(new SqlParameter("@EmployeeID", Order.EmployeeID));
                    cmd.Parameters.Add(new SqlParameter("@orderdate", Order.Orderdate));
                    cmd.Parameters.Add(new SqlParameter("@requireddate", Order.RequireDdate));
                    cmd.Parameters.Add(new SqlParameter("@shippeddate", Order.ShippedDate));
                    cmd.Parameters.Add(new SqlParameter("@shipperid", Order.ShipperId));
                    cmd.Parameters.Add(new SqlParameter("@freight", Order.Freight));
                    cmd.Parameters.Add(new SqlParameter("@shipname", Order.ShipperName));
                    cmd.Parameters.Add(new SqlParameter("@shipaddress", Order.ShipAddress));
                    cmd.Parameters.Add(new SqlParameter("@shipcity", Order.ShipCity));
                    cmd.Parameters.Add(new SqlParameter("@shipregion", Order.ShipRegion));
                    cmd.Parameters.Add(new SqlParameter("@shippostalcode", Order.ShipPostalCode));
                    cmd.Parameters.Add(new SqlParameter("@shipcountry", Order.ShipCountry));
                    cmd.Parameters.Add(new SqlParameter("@OrderID", Order.OrderId));
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(DeleteOrderDetailsSql, conn))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new SqlParameter("@OrderID", Order.OrderId));
                    cmd.ExecuteNonQuery();
                }
                foreach (var OrderDetail in Order.OrderDetails)
                {
                    using (SqlCommand cmd = new SqlCommand(InsertNewOrderDetailsSql, conn))
                    {
                        cmd.Transaction = transaction;
                        cmd.Parameters.Add(new SqlParameter("@OrderID", Order.OrderId));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", OrderDetail.ProductId));
                        cmd.Parameters.Add(new SqlParameter("@UnitPrice", OrderDetail.UnitPrice));
                        cmd.Parameters.Add(new SqlParameter("@Qty", OrderDetail.Qty));
                        cmd.Parameters.Add(new SqlParameter("@Discount", OrderDetail.Discount));
                        cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
            catch (Exception Exception)
            {
                transaction.Rollback();
                throw Exception;
            }
            finally
            {
                conn.Close();
            }
        }
    }


}
}