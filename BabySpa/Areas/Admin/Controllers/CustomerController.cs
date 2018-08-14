using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BabySpa.Areas.Admin.Models;
using System.Data;
using BabySpa.Core;
using BabySpa.Models;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class CustomerController : Controller
    {

        public ActionResult Search(CustSearch search)
        {
            Result res = AdminDBContext.CustList(search);
            if (res.Succeed)
            {
                search.List = ((DataSet)res.Data).Tables[0];
            }
            else
            {
                Main.ErrorLog("Admin-SearchCust", res.Desc);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            ViewBag.ActiveTab = "custTab";
            return PartialView("CustList", search);
        }
        // GET: Admin/Customer
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Customer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customer/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Customer/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "customerTab";

            Customer cust = new Customer();

            Result res = AdminDBContext.GetCustomer(id);

            if (res.Succeed)
            {
                DataTable dt = ((DataSet)res.Data).Tables[0];
                ViewBag.Requirement = ((DataSet)res.Data).Tables[1];
                ViewBag.PaymentList = ((DataSet)res.Data).Tables[2];
                ViewBag.EmailList = ((DataSet)res.Data).Tables[3];
                ViewBag.GroupList = BabySpa.App.getSelectListFromTable(((DataSet)res.Data).Tables[4], "GroupName", "GroupID", true);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    cust.CustID = id;
                    cust.GroupID = Func.ToInt(row["GroupID"]);
                    cust.RegDate = Func.ToDate(row["RegDate"]);
                    cust.Nationality = Func.ToStr(row["Nationality"]);
                    cust.Country = Func.ToStr(row["Country"]);
                    cust.City = Func.ToStr(row["City"]);
                    cust.Address = Func.ToStr(row["Address"]);
                    cust.Address2 = Func.ToStr(row["Address2"]);
                    cust.Phone = Func.ToStr(row["Phone"]);
                    cust.Mobile = Func.ToStr(row["Mobile"]);
                    cust.Email = Func.ToStr(row["Email"]);
                    cust.SocialID = Func.ToStr(row["SocialID"]);
                    cust.Title = Func.ToStr(row["Title"]);
                    cust.FName = Func.ToStr(row["FName"]);
                    cust.LName = Func.ToStr(row["LName"]);
                    cust.Gender = Func.ToInt(row["Gender"]);
                    cust.BirthDate = Func.ToDate(row["BirthDate"]);
                    cust.Languages = Func.ToStr(row["Languages"]);
                    cust.PassportNo = Func.ToStr(row["PassportNo"]);
                    cust.PassportValidDate = Func.ToStr(row["PassportValidDate"]);
                    cust.MembershipID = Func.ToInt(row["MembershipID"]);
                    cust.IsGroupLeader = Func.ToInt(row["IsGroupLeader"]);
                    cust.UBFlightRequired = Func.ToInt(row["UBFlightRequired"]);
                    cust.ExtraRoomRequired = Func.ToInt(row["ExtraRoomRequired"]);
                    cust.JoinGroup = Func.ToInt(row["JoinGroup"]);
                    cust.ArriveDate = Func.ToDate(row["ArriveDate"]);
                    cust.DepartureDate = Func.ToDate(row["DepartureDate"]);
                    cust.MealRequirement = Func.ToStr(row["MealRequirement"]);
                    cust.MedicalCondition = Func.ToStr(row["MedicalCondition"]);
                    cust.Comments = Func.ToStr(row["Comments"]);
                    cust.WhyChooseReason = Func.ToStr(row["MeanReasonChooseUs"]);
                    cust.HearUS = Func.ToStr(row["WhereHearUs"]);
                    cust.AdminNote = Func.ToStr(row["AdminNote"]);
                    ViewBag.TourName = Func.ToStr(row["TourName"]);
                    ViewBag.TourID = Func.ToStr(row["TourID"]);
                    ViewBag.TourDate = Func.ToStr(row["coverdatetext"]);
                    // TODO: Get Cust, Payment,Email, ExtraRequest lists
                }
                else
                {
                    ViewBag.Result = id.ToString() + " дугаартай хэрэглэгч бүртгэлгүй байна.";
                }
            }
            else
                ViewBag.Result = res.Desc;

            if (Request.IsAjaxRequest())
            {
                return PartialView("Index", cust);
            }
            else
            {
                return View("Index", cust);
            }
        }

        // POST: Admin/Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Customer cust)
        {
            Result res = new Result();
            try
            {
                cust.CustID = id;
                res = AdminDBContext.CustSave(cust);
                if (!res.Succeed)
                {
                    Main.ErrorLog("Admin-EditCust", res.Desc);
                    ViewBag.Result = Helper.UN_EXPECTED_MSG;
                }
                else
                    ViewBag.Result = "Successfully";

                return Edit(cust.CustID);
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Admin-EditCust", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
                return View("Error");
            }
        }

        // GET: Admin/Customer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
