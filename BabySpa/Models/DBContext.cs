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
        public static Result GetAvailableTimeTable(string day)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * From TOUR_Itinerary
                            where TourID = @TourID order by orderno, dayno;");
            SqlParameter pOrderDate = new SqlParameter("@OrderDate", SqlDbType.NVarChar);
            pOrderDate.Value = day;


            Result res = App.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pOrderDate},1);
            return res;
        }
    }
}