using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BabySpa.App;

namespace BabySpa
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Route("Бидний тухай")]
        public ActionResult About()
        {
            ViewBag.Message = "Бидний тухай";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Route("Салбарын мэдээлэл")]

        public ActionResult Branches()
        {
            ViewBag.Message = "Салбарын мэдээлэл";

            return PartialView("Branches");
        }
        public ActionResult Order()
        {
            IEnumerable<SelectListItem> timesList = new List<SelectListItem>();
            ViewBag.timesList = timesList;
            OrderViewModel model = new OrderViewModel();
            model.List = new List<OrderService>();
            return PartialView("_Order",model);
        }
        [HttpPost]
        public ActionResult Order(OrderViewModel model)
        {
            return PartialView("_OrderSent",model);
        }
        public JsonResult GetCust(string name)
        {
            return Json("");
        }
        public JsonResult GetChild(string name)
        {
            return Json("");
        }
        public JsonResult GetTimeTable(string day, string branch_id)
        {
            int day_no = (int)Func.ToDate(day).DayOfWeek;
            TimeTable tt;
            List<SelectListItem> timeList = new List<SelectListItem>();
            timeList.Insert(0, new SelectListItem() { Text = "Цагаа сонгоно уу", Value = "",Selected=true });

            if (App.timeTable.ContainsKey(branch_id + "_" + day_no))
            {
                tt = (TimeTable)(App.timeTable[branch_id + "_" + day_no]);
                Result res = DBContext.GetOrderTimes(day, branch_id);
                decimal day_start_time =Func.ToDecimal( tt.start_time);
                decimal day_end_time = Func.ToDecimal(tt.end_time);
                decimal tt_end = day_start_time + 1;
                decimal tt_start = day_start_time;
                DataTable dt =((DataSet)res.Data).Tables[0];
                decimal order_start;
                decimal order_end;
                bool have_order ;

                while (tt_end<=day_end_time)
                {
                    have_order = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        order_start =Func.ToDecimal(Func.ToStr(dt.Rows[i][0]).Split('-')[0]);
                        order_end = Func.ToDecimal(Func.ToStr(dt.Rows[i][0]).Split('-')[1]);

                        if ( (tt_start<=order_start && tt_end>order_start) || (tt_start<order_end && tt_end>=order_end))
                        {
                            have_order = true;
                            break;
                        }
                    }
                    if (!have_order)
                    {
                        timeList.Add(new SelectListItem()
                        {
                            Text = tt_start+"-"+tt_end,
                            Value = tt_start + "-" + tt_end,
                        });
                    }
                    tt_start = tt_start + 1;
                    tt_end = tt_end+ 1;
                    if (tt_start < tt_end && tt_end>day_end_time)
                    {
                        tt_end = day_end_time;
                    }
                }
                tt = null;
                dt = null;
                return Json(new SelectList(timeList, "Value", "Text"));
            }
            else
            {
                timeList[0].Text = "Боломжтой цаг алга байна.";
                return Json(new SelectList(timeList, "Value", "Text"));
            }
        }
    }
}