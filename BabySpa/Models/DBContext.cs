using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using MDMA;
namespace BabySpa
{
    public class DBContext
    {
        public static Result GetOrderTimes(string day, string branch_id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select order_time from order_service o
                            left join cust_order co on o.order_id = co.order_id
                            where o.order_date = @order_date and co.branch_id = @branch_id
                            order by order_time;");
            SqlParameter pOrderDate = new SqlParameter("@order_date", SqlDbType.NVarChar);
            SqlParameter pBranchId = new SqlParameter("@branch_id", SqlDbType.Int);
            pOrderDate.Value = day;
            pBranchId.Value = branch_id;


            Result res = App.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pOrderDate,pBranchId},1);
            return res;
        }
    }
}