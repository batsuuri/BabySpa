using BabySpa.Areas.Admin.Models;
using BabySpa.Core;
using BabySpa.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class TourController : Controller
    {
        // GET: Admin/Tour
        #region Tour
        public ActionResult Index()
        {
            TourSearch search = new TourSearch();
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "TourTab";
            return View(search);
        }
        [HttpPost]
        public ActionResult Index(TourSearch search)
        {
            search.List = new List<BabySpa.Models.Tour>();
           
            #region Search
            Result res = TourDBContext.TourList(search);
            if (res.Succeed)
            {
                search.List = (List<BabySpa.Models.Tour>)res.Data;
            }
            #endregion

            return View(search);
        }

        public ActionResult Edit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "tourTab";

            Tour tour;
            TourSearch search = new TourSearch();
            search.TourID = id;
            Result res = TourDBContext.TourList(search);
            if (res.Succeed)
            {
                search.List = (List<BabySpa.Models.Tour>)res.Data;
                if (search.List != null && search.List.Count > 0)
                {
                    tour = search.List[0];
                }
                else
                {
                    ViewBag.Result = "Not Found Tour:" + id;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Result = "Not Found Tour:" + id;
                return View("Error");
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("TourEdit", tour);
            }
            else
            {
                return View("TourEdit", tour);
            }
        }
        [HttpPost]
        public ActionResult Edit(string id, Tour tour, string submit, HttpPostedFileBase fileCover)
        {
            ViewBag.ActiveTab = "tourTab";
            Session["Message"] = "";
            Result res = new Result();
            string filename = "";
            bool haveFile = fileCover != null && fileCover.ContentLength > 0;
            // 1. Check previous file exist
            if (haveFile)
            {
                if (Func.ToStr(tour.CoverPhoto) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = tour.TourId + @"\" + Path.GetFileNameWithoutExtension(tour.CoverPhoto) + Path.GetExtension(fileCover.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = tour.TourId + @"\" + Guid.NewGuid().ToString() + Path.GetExtension(fileCover.FileName);
                }
                tour.CoverPhoto = filename.Replace(@"\", @"/");
            }
            try
            {
                switch (submit)
                {
                    case "save":

                        res = TourDBContext.Save(tour);
                        if (!res.Succeed)
                        {
                            Main.ErrorLog("Admin-EditOrder", res.Desc);
                            ViewBag.Result = Helper.UN_EXPECTED_MSG;
                            Session["Message"] = Helper.UN_EXPECTED_MSG;
                        }
                        else
                        {
                            ViewBag.Result = Session["Message"] = "Successfully updated tour information";
                            // Refresh tour data on memory
                            //AppConfig.InitTour();
                            if (haveFile)
                            {
                                res = Helper.SaveFile(filename, fileCover);
                                if (!res.Succeed)
                                {
                                    ViewBag.Result = "Successfully updated db, but error occured while saving file:" + res.Desc;
                                }
                            }
                        }
                        return Edit(id);
                    //return Content(ViewBag.Result);
                    case "delete":

                        return Delete(tour.TourId);
                    default:
                        return View(tour);
                }

            }
            catch (Exception ex)
            {
                Main.ErrorLog("Admin-EditOrder", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
                return View("Error");
            }
        }
        public ActionResult ChangeStatus(string id)
        {
            Session["Message"] = "";
            Result res = TourDBContext.ChangeStatus(id.Split('~')[0], Func.ToInt(id.Split('~')[1]));
            if (!res.Succeed)
            {
                Main.ErrorLog("Admin-ChangeTourStatus", res.Desc);
                Session["Message"] = Helper.UN_EXPECTED_MSG;
            }
            else
            {
                //App.tourList[id.Split('~')[0]].Status = ((Tour)App.tourHDT[id.Split('~')[0]]).Status = Func.ToInt(id.Split('~')[1]);
                Session["Message"] = "Tour status updated.";
            }
            return RedirectToAction("Edit", new { id = id.Split('~')[0] });
        }
        public ActionResult Delete(string id)
        {
            Session["Message"] = "";
            // Some business logic here

            Result res = TourDBContext.Delete(id);
            if (!res.Succeed)
            {
                Main.ErrorLog("Admin-DeleteTour", res.Desc);
                Session["Message"] = Helper.UN_EXPECTED_MSG;
            }
            else
            {
                App.tourHDT.Remove(id);
                App.tourList.Remove(id);
                Session["Message"] = "Successfully deleted tour.";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Copy()
        {
            Session["Message"] = "";
            // Some business logic here
            Result res = TourDBContext.Copy(Request["TourID"],Func.ToInt(Request["TourSeason"]));
            if (!res.Succeed)
            {
                Main.ErrorLog("Admin-CopyTour", res.Desc);
                return Content(res.Desc);
            }
            else
            {
                return RedirectToAction("Edit", new { id = Func.ToStr(res.Data) });
                //return Edit(Func.ToStr(res.Data));
                // return Content("Successful.");
            }

        }
        #endregion

        //********************* Itinerary ************************//
        #region Itinerary
        public ActionResult Itinerary(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "itineraryTab";

            ItineraryViewModel model = new ItineraryViewModel();
            model.List = new List<BabySpa.Models.Itinerary>();
            model.TourID = id;

            Result res = TourDBContext.GetInineraryList(id);
            if (res.Succeed)
            {
                DataTable dt = ((DataSet)res.Data).Tables[0];
                Itinerary item;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        item = new Itinerary();

                        item.ItineraryID = Func.ToInt(dt.Rows[i]["ItineraryID"]);
                        item.TourId = Func.ToStr(dt.Rows[i]["tourid"]);
                        item.DayNo = Func.ToStr(dt.Rows[i]["DayNo"]);
                        item.Subject = Func.ToStr(dt.Rows[i]["Subject"]);
                        item.Texts = Func.ToStr(dt.Rows[i]["Texts"]);
                        item.PhotoName = Func.ToStr(dt.Rows[i]["PhotoName"]);
                        item.Hint = Func.ToStr(dt.Rows[i]["Hint"]);
                        item.Distance = Func.ToStr(dt.Rows[i]["Distance"]);
                        item.Distance2 = Func.ToStr(dt.Rows[i]["Distance2"]);
                        item.Distance3 = Func.ToStr(dt.Rows[i]["Distance3"]);

                        item.Duration = Func.ToStr(dt.Rows[i]["Duration"]);
                        item.Duration2 = Func.ToStr(dt.Rows[i]["Duration2"]);
                        item.Duration3 = Func.ToStr(dt.Rows[i]["Duration3"]);
                        item.OrderNo = Func.ToInt(dt.Rows[i]["OrderNo"]);

                        #region Itinerary Dic Values 
                        item.Transport.dataID = Func.ToStr(dt.Rows[i]["Transport"]);

                        item.Accommodation.dataID = Func.ToStr(dt.Rows[i]["Accommodation"]);
                        item.Accommodation2.dataID = Func.ToStr(dt.Rows[i]["Accommodation2"]);
                        item.Accommodation3.dataID = Func.ToStr(dt.Rows[i]["Accommodation3"]);

                        item.Destination.dataID = Func.ToStr(dt.Rows[i]["Destination"]);
                        item.Destination2.dataID = Func.ToStr(dt.Rows[i]["Destination2"]);
                        item.Destination3.dataID = Func.ToStr(dt.Rows[i]["Destination3"]);

                        item.BreakFast.dataID = Func.ToStr(dt.Rows[i]["BreakFast"]);
                        item.BreakFast2.dataID = Func.ToStr(dt.Rows[i]["BreakFast2"]);
                        item.BreakFast3.dataID = Func.ToStr(dt.Rows[i]["BreakFast3"]);

                        item.Lunch.dataID = Func.ToStr(dt.Rows[i]["Lunch"]);
                        item.Lunch2.dataID = Func.ToStr(dt.Rows[i]["Lunch2"]);
                        item.Lunch3.dataID = Func.ToStr(dt.Rows[i]["Lunch3"]);

                        item.Dinner.dataID = Func.ToStr(dt.Rows[i]["Dinner"]);
                        item.Dinner2.dataID = Func.ToStr(dt.Rows[i]["Dinner2"]);
                        item.Dinner3.dataID = Func.ToStr(dt.Rows[i]["Dinner3"]);
                        #endregion

                        model.List.Add(item);
                    }
                }
                else
                {
                    ViewBag.Result = "Not registerd itinerary for tour, TourID:" + id;
                }
            }
            else
            {
                ViewBag.Result = "Error occured when get Itinerary list, TourID:" + id;
            }
            return PartialView("Itinerary", model);
        }
        public ActionResult ItineraryEdit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "itineraryTab";

            //List<BabySpa.Models.Itinerary> list = (List<BabySpa.Models.Itinerary>)Session["ItineraryList" + id.Split('~')[0]];
            Itinerary item = new Itinerary();
            Result res = TourDBContext.GetIninerary(id);
            if (res.Succeed)
            {
                DataTable dt = ((DataSet)res.Data).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    item.ItineraryID = Func.ToInt(dt.Rows[0]["ItineraryID"]);
                    item.TourId = Func.ToStr(dt.Rows[0]["tourid"]);
                    item.DayNo = Func.ToStr(dt.Rows[0]["DayNo"]);
                    item.Subject = Func.ToStr(dt.Rows[0]["Subject"]);
                    item.Texts = Func.ToStr(dt.Rows[0]["Texts"]);
                    item.PhotoName = Func.ToStr(dt.Rows[0]["PhotoName"]);
                    item.Hint = Func.ToStr(dt.Rows[0]["Hint"]);
                    item.Distance = Func.ToStr(dt.Rows[0]["Distance"]);
                    item.Distance2 = Func.ToStr(dt.Rows[0]["Distance2"]);
                    item.Distance3 = Func.ToStr(dt.Rows[0]["Distance3"]);

                    item.Duration = Func.ToStr(dt.Rows[0]["Duration"]);
                    item.Duration2 = Func.ToStr(dt.Rows[0]["Duration2"]);
                    item.Duration3 = Func.ToStr(dt.Rows[0]["Duration3"]);
                    item.OrderNo = Func.ToInt(dt.Rows[0]["OrderNo"]);

                    #region Itinerary Dic Values 

                    item.Transport.dataID = Func.ToStr(dt.Rows[0]["Transport"]); ;
                    item.Accommodation.dataID = Func.ToStr(dt.Rows[0]["Accommodation"]);
                    item.Accommodation2.dataID = Func.ToStr(dt.Rows[0]["Accommodation2"]);
                    item.Accommodation3.dataID = Func.ToStr(dt.Rows[0]["Accommodation3"]);

                    item.Destination.dataID = Func.ToStr(dt.Rows[0]["Destination"]);
                    item.Destination2.dataID = Func.ToStr(dt.Rows[0]["Destination2"]);
                    item.Destination3.dataID = Func.ToStr(dt.Rows[0]["Destination3"]);

                    item.BreakFast.dataID = Func.ToStr(dt.Rows[0]["BreakFast"]);
                    item.BreakFast2.dataID = Func.ToStr(dt.Rows[0]["BreakFast2"]);
                    item.BreakFast3.dataID = Func.ToStr(dt.Rows[0]["BreakFast3"]);

                    item.Lunch.dataID = Func.ToStr(dt.Rows[0]["Lunch"]);
                    item.Lunch2.dataID = Func.ToStr(dt.Rows[0]["Lunch2"]);
                    item.Lunch3.dataID = Func.ToStr(dt.Rows[0]["Lunch3"]);

                    item.Dinner.dataID = Func.ToStr(dt.Rows[0]["Dinner"]);
                    item.Dinner2.dataID = Func.ToStr(dt.Rows[0]["Dinner2"]);
                    item.Dinner3.dataID = Func.ToStr(dt.Rows[0]["Dinner3"]);

                    #endregion

                }
                else
                {
                    ViewBag.Result = "Not found itinerary with id:" + id;
                }
            }
            else
            {
                ViewBag.Result = "Error occured when get Itinerary, ID:" + id;
            }

            return PartialView("ItineraryEdit", item);
        }

        [HttpPost]
        public ActionResult ItineraryEdit(int ItineraryID, Itinerary item)
        {
            Result res = new Result();
           
            try
            {
                item.ItineraryID = ItineraryID;
                res = TourDBContext.ItinerarySave(item);
                if (!res.Succeed)
                {
                    Main.ErrorLog("Admin-EditItinerary", res.Desc);
                    ViewBag.Result = Helper.UN_EXPECTED_MSG;
                }
                else
                {
                    ViewBag.Result = "Successfully!";
                    //AppConfig.InitTour();
                }

                    return ItineraryEdit(Func.ToStr(item.ItineraryID));
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Admin-EditItinerary", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
                return View("Error");
            }
        }

        public ActionResult ItineraryNew(string id)
        {
            Itinerary item = new Itinerary();
            item.TourId = id;
            return PartialView("ItineraryNew", item);
        }
        [HttpPost]
        public ActionResult ItineraryNew(string tourID, Itinerary item)
        {
            Result res = new Result();
            try
            {
                item.TourId = tourID;
                res = TourDBContext.ItineraryInsert(item);
                if (!res.Succeed)
                {
                    Main.ErrorLog("Admin-InsertItinerary", res.Desc);
                    ViewBag.Result = Helper.UN_EXPECTED_MSG;
                    return PartialView(item);
                }
                else
                {
                    //AppConfig.InitTour();
                    ViewBag.Result = "Successfully!";
                    return ItineraryEdit(Func.ToStr(res.Data));
                }

            }
            catch (Exception ex)
            {
                Main.ErrorLog("Admin-InsertItinerary", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ItineraryDelete(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "itineraryTab";

            Result res = TourDBContext.DeleteItinerary(id);
            if (res.Succeed)
            {
                Session["Message"] = "Deleted Itinerary";
            }
            else
            {
                Session["Message"] = "Error occured when Delete Itinerary, ID:" + id;
            }

            return Itinerary(Request.QueryString["tourid"]);
        }
        #endregion

        //******************** Transport *******************//
        #region Transport
        public ActionResult TransportList(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "TransportTab";


            return PartialView("Transport", InfoList(id, "transport"));
        }

        public ActionResult TransportNew(string id)
        {

            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "TransportTab";
            InfoViewModel model = InfoList(id, "transport");
            model.CurrentInfo = new Models.Info();
            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return PartialView("Transport", model);
        }
        [HttpPost]
        public ActionResult TransportInsert(string TourID, Models.Info CurrentInfo)
        {
            InfoViewModel model;

            // Insert New Trasport here
            CurrentInfo.TourID = TourID;
            CurrentInfo.InfoType = "transport";
            Result res = TourDBContext.InfoInsert(CurrentInfo);
            model = InfoList(TourID, "transport"); ;
            if (res.Succeed)
            {
                Session["Message"] = "Successfully added";
                return RedirectToAction("TransportEdit", "Tour",new { id = Func.ToStr(res.Data) });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                return PartialView("Transport", model);
            }
        }
        public ActionResult TransportEdit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.Info info = InfoDetail(id);
            InfoViewModel model = InfoList(info.TourID, "transport"); ;

            model.InfoKey = id;
            model.CurrentInfo = info;
            model.DisplayMode = "EditOnly";
            return PartialView("Transport", model);
        }

        [HttpPost]
        public ActionResult TransportUpdate(string TourID, Models.Info CurrentInfo)
        {
            InfoViewModel model;

            // Insert New Trasport here
            CurrentInfo.TourID = TourID;
            CurrentInfo.InfoType = "transport";
            Result res = TourDBContext.InfoUpdate(CurrentInfo);
            model = InfoList(TourID, "transport"); ;
            model.DisplayMode = "EditOnly";
            model.InfoKey = CurrentInfo.InfoKey;
            foreach (Models.Info item in model.List)
            {
                if (item.InfoKey == model.InfoKey)
                {
                    model.CurrentInfo = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return PartialView("Transport", model);
        }

        public ActionResult TransportDelete(string id)
        {
            Models.Info info = InfoDetail(id);

            Result res = TourDBContext.InfoDelete(info);
            InfoViewModel model = InfoList(info.TourID, "transport"); ;
            if (res.Succeed)
            {
                model.CurrentInfo = null;
                ViewBag.Result = "Successfully Deleted.";
            }
            else
                ViewBag.Result = res.Desc;
            model.DisplayMode = "ReadOnly";
            return PartialView("Transport", model);
        }
        #endregion

        //******************** Staff *********************//
        #region Staff 
        public ActionResult StaffList(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "StaffTab";


            return PartialView("Staff", InfoList(id, "staff"));
        }

        public ActionResult StaffNew(string id)
        {

            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ViewBag.ActiveTab = "StaffTab";
            InfoViewModel model = InfoList(id, "staff");
            model.CurrentInfo = new Models.Info();
            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return PartialView("Staff", model);
        }
        [HttpPost]
        public ActionResult StaffInsert(string TourID, Models.Info CurrentInfo)
        {
            InfoViewModel model;

            // Insert New Trasport here
            CurrentInfo.TourID = TourID;
            CurrentInfo.InfoType = "staff";
            Result res = TourDBContext.InfoInsert(CurrentInfo);
            model = InfoList(TourID, "staff"); ;
            if (res.Succeed)
            {
                Session["Message"] = "Successfully added";
                return RedirectToAction("StaffEdit", "Tour", new { id = Func.ToStr(res.Data) });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                return PartialView("Staff", model);
            }
        }
        public ActionResult StaffEdit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.Info info = InfoDetail(id);
            InfoViewModel model = InfoList(info.TourID, "staff"); ;

            model.InfoKey = id;
            model.CurrentInfo = info;
            model.DisplayMode = "EditOnly";
            return PartialView("Staff", model);
        }

        [HttpPost]
        public ActionResult StaffUpdate(string TourID, Models.Info CurrentInfo)
        {
            InfoViewModel model;

            // Insert New Trasport here
            CurrentInfo.TourID = TourID;
            CurrentInfo.InfoType = "staff";
            Result res = TourDBContext.InfoUpdate(CurrentInfo);
            model = InfoList(TourID, "staff"); ;
            model.DisplayMode = "EditOnly";
            model.InfoKey = CurrentInfo.InfoKey;
            foreach (Models.Info item in model.List)
            {
                if (item.InfoKey == model.InfoKey)
                {
                    model.CurrentInfo = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return PartialView("Staff", model);
        }

        public ActionResult StaffDelete(string id)
        {
            Models.Info info = InfoDetail(id);

            Result res = TourDBContext.InfoDelete(info);
            InfoViewModel model = InfoList(info.TourID, "staff"); ;
            if (res.Succeed)
            {
                model.CurrentInfo = null;
                ViewBag.Result = "Successfully Deleted.";
            }
            else
                ViewBag.Result = res.Desc;
            model.DisplayMode = "ReadOnly";
            return PartialView("Staff", model);
        }
        private InfoViewModel InfoList(string tourID, string type)
        {
            InfoViewModel model = new InfoViewModel();
            model.List = new List<BabySpa.Areas.Admin.Models.Info>();
            model.TourID = tourID;
            model.InfoType = type;
            model.DisplayMode = "ReadOnly";
            try
            {
                Result res = TourDBContext.GetInfoList(tourID, type.ToLower());
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    BabySpa.Areas.Admin.Models.Info item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.Info();

                            item.TourID = Func.ToStr(dt.Rows[i]["tourid"]);
                            item.InfoType = Func.ToStr(dt.Rows[i]["InfoType"]);
                            item.InfoValue = Func.ToStr(dt.Rows[i]["InfoValue"]);
                            item.InfoValue2 = Func.ToStr(dt.Rows[i]["InfoValue2"]);
                            item.InfoDesc = Func.ToStr(dt.Rows[i]["InfoDesc"]);
                            item.InfoOrder = Func.ToInt(dt.Rows[i]["InfoOrder"]);
                            item.InfoKey = Func.ToStr(dt.Rows[i]["InfoKey"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd " + type + " for tour, TourID:" + tourID;
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get " + type + " list, TourID:" + tourID;
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get InfoList, Type:" + type, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        private Models.Info InfoDetail(string infoKey)
        {
            Models.Info model = new Models.Info();
            try
            {
                Result res = TourDBContext.GetInfoDetail(infoKey);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.TourID = Func.ToStr(dt.Rows[0]["tourid"]);
                        model.InfoType = Func.ToStr(dt.Rows[0]["InfoType"]);
                        model.InfoValue = Func.ToStr(dt.Rows[0]["InfoValue"]);
                        model.InfoValue2 = Func.ToStr(dt.Rows[0]["InfoValue2"]);
                        model.InfoDesc = Func.ToStr(dt.Rows[0]["InfoDesc"]);
                        model.InfoOrder = Func.ToInt(dt.Rows[0]["InfoOrder"]);
                        model.InfoKey = Func.ToStr(dt.Rows[0]["InfoKey"]);
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd info for tour, InfoKey:" + infoKey;
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get info, InfoKey:" + infoKey;
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Info, InfoKey:" + infoKey, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }
        #endregion

        //************* Other Details ************//
        #region Other Details
        public ActionResult OtherDetails(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            
            TourDetailViewModel model = new TourDetailViewModel();
            model.TourID = id;

            Result res = TourDBContext.GetOtherDetails(id);
            if (res.Succeed)
            {
                DataTable dt = ((DataSet)res.Data).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.Accommodation = Func.ToStr(dt.Rows[0]["Accommodation"]);
                    model.Included = Func.ToStr(dt.Rows[0]["Include"]);
                    model.NotIncluded= Func.ToStr(dt.Rows[0]["NotInclude"]);
                    model.Package= Func.ToStr(dt.Rows[0]["Package"]);
                    model.RangedPrice = Func.ToStr(dt.Rows[0]["RangedPrice"]);
                    model.MapPath = Func.ToStr(dt.Rows[0]["TourMapUrl"]);
                    model.GoogleMapUrl = Func.ToStr(dt.Rows[0]["GoogleMapUrl"]);
                }
                else
                {
                    ViewBag.Result = "Not registerd any details for tour, TourID:" + id;
                }
            }
            else
            {
                ViewBag.Result = "Error occured when get other details , TourID:" + id;
            }

            return PartialView("OtherDetails", model);
        }

        [HttpPost]
        public ActionResult OtherDetailsUpdate(TourDetailViewModel tourdetailviewmodel, HttpPostedFileBase file)
        {
            string filename ="";
            bool haveFile = file != null && file.ContentLength > 0;
            // 1. Check previous file exist
            if (haveFile)
            {
                if (Func.ToStr(tourdetailviewmodel.MapPath) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = tourdetailviewmodel.TourID+ @"\"+ Path.GetFileNameWithoutExtension(tourdetailviewmodel.MapPath) + Path.GetExtension(file.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = tourdetailviewmodel.TourID + @"\"+Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                }
                tourdetailviewmodel.MapPath = filename.Replace(@"\",@"/");
            }
            // 4. Execute DB process
            Result res = TourDBContext.DeteilsUpdate(tourdetailviewmodel);
            // 5. If DB process sucess, Save file with name

            if (res.Succeed)
            {
                if (haveFile)
                {
                    res = Helper.SaveFile(filename, file);
                    if (!res.Succeed)
                    {
                        Session["Message"] = "Successfully updated db, but error occured while saving file:" + res.Desc;
                    }
                }

                Session["Message"] = "Successfully updated";
            }
            else
            {
                Session["Message"] = res.Desc;
            }
            //return RedirectToAction("OtherDetails", "tour",tourdetailviewmodel.TourID);
            //ModelState.Clear();
            return OtherDetails(tourdetailviewmodel.TourID);
        }
        #endregion

        //************************* Photos ****************************//
        #region Photos
        public ActionResult PhotoList(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            

            return View("Photo", GetPhotoList(id));
        }

        public ActionResult PhotoNew(string id)
        {

            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            TourPhotosModel model = GetPhotoList(id);
            model.CurrentPhoto = new Models.TourPhoto();
            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return View("Photo", model);
        }
        [HttpPost]
        public ActionResult PhotoInsert(string TourID, Models.TourPhoto CurrentPhoto, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            TourPhotosModel model;
            CurrentPhoto.TourID = TourID;

            string filename = "";
            bool haveFile = file != null && file.ContentLength > 0;
            // 1. Check previous file exist
            if (haveFile)
            {

                // 3. If not exist generate new name with uploaded file extension
                filename = CurrentPhoto.TourID + @"\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                CurrentPhoto.PhotoUrl = filename.Replace(@"\", @"/");
            }
            // 4. Execute DB process
            Result res = TourDBContext.PhotoInsert(CurrentPhoto);
            model = GetPhotoList(TourID);
            if (res.Succeed)
            {
                int id = Func.ToInt(res.Data);
                Session["Message"] = "Successfully added";
                if (haveFile)
                {
                    res = Helper.SaveFile(filename, file);
                    if (!res.Succeed)
                    {
                        ViewBag.Result = "Successfully updated db, but error occured while saving file:" + res.Desc;
                    }
                    if (fileSmall !=null && fileSmall.ContentLength > 0)
                    {
                        res = Helper.SaveFile(filename.Replace("_big","_small"), fileSmall);
                    }
                }
                return RedirectToAction("PhotoEdit", "Tour", new { id = id });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                return View("Photo", model);
            }
        }
        public ActionResult PhotoEdit(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.TourPhoto photo = PhotoDetail(id);
            TourPhotosModel model = GetPhotoList(photo.TourID) ;
           
            model.PhotoID = id;
            model.CurrentPhoto= photo;
            model.DisplayMode = "EditOnly";
            return View("photo", model);
        }

        [HttpPost]
        public ActionResult PhotoUpdate(string TourID, Models.TourPhoto CurrentPhoto, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            TourPhotosModel model;
            CurrentPhoto.TourID = TourID;

            string filename = "";
            string filenameSmall = "";
            bool haveFile = file != null && file.ContentLength > 0;
            bool haveFileSmall = (fileSmall != null && fileSmall.ContentLength > 0 && Func.ToStr(CurrentPhoto.PhotoUrl) != "");
            // 1. Check previous file exist
            if (haveFile)
            {
                if (Func.ToStr(CurrentPhoto.PhotoUrl) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = CurrentPhoto.TourID + @"\" + Path.GetFileNameWithoutExtension(CurrentPhoto.PhotoUrl) + Path.GetExtension(file.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = CurrentPhoto.TourID + @"\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                }
                CurrentPhoto.PhotoUrl = filename.Replace(@"\", @"/");
            }
            if (haveFileSmall)
            {
                // 2. If exist save exact name with uploaded file extension
                filenameSmall = CurrentPhoto.TourID + @"\" + Path.GetFileNameWithoutExtension(CurrentPhoto.PhotoUrl).Replace("_big", "_small") + Path.GetExtension(fileSmall.FileName);
            }
            Result res = TourDBContext.PhotoUpdate(CurrentPhoto);
            if (res.Succeed)
            {
                if (haveFile)
                {
                    res = Helper.SaveFile(filename, file);
                    if (!res.Succeed)
                    {
                        ViewBag.Result = "Successfully updated db, but error occured while saving file:" + res.Desc;
                    }
                }
                if (haveFileSmall)
                {
                    res = Helper.SaveFile(filenameSmall, fileSmall);
                }
            }
            model = GetPhotoList(TourID);
            model.DisplayMode = "EditOnly";
            model.PhotoID = CurrentPhoto.PhotoID;
            foreach (Models.TourPhoto item in model.List)
            {
                if (item.PhotoID == model.PhotoID)
                {
                    model.CurrentPhoto = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return View("Photo", model);
        }

        public ActionResult PhotoDelete(int id)
        {
            Models.TourPhoto photo = PhotoDetail(id);

            Result res = TourDBContext.PhotoDelete(photo);
            TourPhotosModel model = GetPhotoList(photo.TourID); ;
            if (res.Succeed)
            {
                Helper.DeleteFile(photo.PhotoUrl);
                model.CurrentPhoto = null;
                ViewBag.Result = "Successfully Deleted.";
            }
            else
                ViewBag.Result = res.Desc;
            model.DisplayMode = "ReadOnly";
            return View("Photo", model);
        }

        private TourPhotosModel GetPhotoList(string tourID)
        {
            TourPhotosModel model = new TourPhotosModel();
            model.List = new List<BabySpa.Areas.Admin.Models.TourPhoto>();
            model.TourID = tourID;
            model.DisplayMode = "ReadOnly";
            model.Days = Days(tourID);
            try
            {
                Result res = TourDBContext.GetPhotoList(tourID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    BabySpa.Areas.Admin.Models.TourPhoto item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.TourPhoto();

                            item.TourID = Func.ToStr(dt.Rows[i]["tourid"]);
                            item.PhotoID = Func.ToInt(dt.Rows[i]["PhotoID"]);
                            item.PhotoName = Func.ToStr(dt.Rows[i]["PhotoName"]);
                            item.PhotoDesc = Func.ToStr(dt.Rows[i]["PhotoDesc"]);
                            item.PhotoUrl = Func.ToStr(dt.Rows[i]["PhotoUrl"]);
                            item.DayNo = Func.ToStr(dt.Rows[i]["DayNo"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd photos for tour, TourID:" + tourID;
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get photo list, TourID:" + tourID;
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get PhotoList, tourid:" + tourID, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        private Models.TourPhoto PhotoDetail(int photoID)
        {
            Models.TourPhoto model = new Models.TourPhoto();
            try
            {
                Result res = TourDBContext.GetPhotoDetail(photoID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        model.PhotoID = photoID;
                        model.TourID = Func.ToStr(dt.Rows[0]["tourid"]);
                        model.PhotoName = Func.ToStr(dt.Rows[0]["PhotoName"]);
                        model.PhotoDesc = Func.ToStr(dt.Rows[0]["PhotoDesc"]);
                        model.PhotoUrl = Func.ToStr(dt.Rows[0]["PhotoUrl"]);
                        model.DayNo = Func.ToStr(dt.Rows[0]["DayNo"]);
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd photo for tour, photoid:" + photoID.ToString();
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get photo,  photoid:" + photoID.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Photo, photoid:" + photoID.ToString(), ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }

        private List<SelectListItem> Days(string tourid) {

            List<SelectListItem> dataList = new List<SelectListItem>();
            dataList.Add(new SelectListItem()
            {
                Text = "",
                Value = "",
            });
            Result res = TourDBContext.GetInineraryList(tourid);
            if (res.Succeed)
            {
                DataTable dt = ((DataSet)res.Data).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Func.ToStr(dt.Rows[i]["TourID"]) == tourid)
                        {
                        dataList.Add(new SelectListItem()
                        {
                            Text = Func.ToStr(dt.Rows[i]["DayNo"]),
                            Value = Func.ToStr(dt.Rows[i]["DayNo"]),
                        });
                        }
                    }
                }
            }
            else
            {
                ViewBag.Result = "Error occured when get Itinerary list, TourID:" + tourid;
            }
            dataList.Sort((a, b) => string.Compare(a.Text, b.Text));
            return dataList;
        }
        #endregion
    }
}
