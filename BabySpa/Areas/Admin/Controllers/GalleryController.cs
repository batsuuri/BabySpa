using BabySpa.Areas.Admin.Models;
using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class GalleryController : Controller
    {
        // GET: Admin/Gallery
        public ActionResult Index()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }


            return View("Gallery", GetGalleryList());
        }

        // GET: Admin/Gallery/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Gallery/Create
        public ActionResult New()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            GalleryViewModel model = GetGalleryList();
            model.CurrentGallery = new Models.Gallery();
            model.CurrentGallery.CoverUrl = "";

            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return View("Gallery", model);
        }

        // POST: Admin/Gallery/Create
        [HttpPost]
        public ActionResult Insert(Models.Gallery CurrentGallery, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            GalleryViewModel model;
            string filename = "";
            bool haveFile = false;
            
                haveFile = file != null && file.ContentLength > 0;
                if (haveFile)
                {
                    filename = @"Gallery\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                    CurrentGallery.CoverUrl = filename.Replace(@"\", @"/");
                }
            // 4. Execute DB process
            Result res = AdminDBContext.GalleryInsert(CurrentGallery);
            model = GetGalleryList();
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
                        if (fileSmall != null && fileSmall.ContentLength > 0)
                        {
                            res = Helper.SaveFile(filename.Replace("_big", "_small"), fileSmall);
                        }
                    }
                return RedirectToAction("Edit", "Gallery", new { id = id });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                return View("Gallery", model);
            }
        }

        // GET: Admin/Gallery/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.Gallery Gallery = GalleryDetail(id);
            GalleryViewModel model = GetGalleryList();

            model.CurrentGallery= Gallery;
            model.GalleryID = Gallery.GalleryID;
            model.DisplayMode = "EditOnly";
            return View("Gallery", model);
        }

        // POST: Admin/Gallery/Edit/5
        [HttpPost]
        public ActionResult Update(Models.Gallery CurrentGallery, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            GalleryViewModel model;
            string filename = "";
            string filenameSmall = "";
            bool haveFile = false;
            bool haveFileSmall = false;

            haveFile = file != null && file.ContentLength > 0;
            haveFileSmall = (fileSmall != null && fileSmall.ContentLength > 0);
            if (haveFile)
            {
                if (Func.ToStr(CurrentGallery.CoverUrl) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = @"Gallery\" + Path.GetFileNameWithoutExtension(CurrentGallery.CoverUrl) + Path.GetExtension(file.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = @"Gallery\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                }
                CurrentGallery.CoverUrl = filename.Replace(@"\", @"/");
            }
            // If big photo have and small image uploaded
            if (haveFileSmall && Func.ToStr(CurrentGallery.CoverUrl) != "")
            {
                haveFileSmall = true;
                filenameSmall = @"Gallery\" + Path.GetFileNameWithoutExtension(CurrentGallery.CoverUrl).Replace("_big", "_small") + Path.GetExtension(fileSmall.FileName);
            }
            // 4. Execute DB process
            Result res = AdminDBContext.GalleryUpdate(CurrentGallery);
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
            model = GetGalleryList();
            model.DisplayMode = "EditOnly";
            model.GalleryID = CurrentGallery.GalleryID;
            foreach (Models.Gallery item in model.List)
            {
                if (item.GalleryID == model.GalleryID)
                {
                    model.CurrentGallery = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return View("Gallery", model);
        }

        // GET: Admin/Gallery/Delete/5
        public ActionResult Delete(int id)
        {

            Models.Gallery Gallery = GalleryDetail(id);
            Result res = AdminDBContext.GalleryDelete(id);
            GalleryViewModel model = GetGalleryList();
            if (res.Succeed)
            {
                for (int i = 0; i < Gallery.CoverUrl.Count(); i++)
                {
                    Helper.DeleteFile(Gallery.CoverUrl);
                }
                model.CurrentGallery = null;
                ViewBag.Result = "Successfully Deleted.";
                model.DisplayMode = "ReadOnly";
            }
            else
            {
                model.CurrentGallery = Gallery;
                model.GalleryID = id;
                ViewBag.Result = res.Desc;
            model.DisplayMode = "EditOnly";
            }
            return View("Gallery", model);
        }

        public GalleryViewModel GetGalleryList()
        {
            GalleryViewModel model = new GalleryViewModel();
            model.List = new List<BabySpa.Areas.Admin.Models.Gallery>();
            model.DisplayMode = "ReadOnly";
            try
            {
                Result res = AdminDBContext.GetGalleryList();
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    BabySpa.Areas.Admin.Models.Gallery item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.Gallery();
                            item.CoverUrl ="";

                            item.GalleryID = Func.ToInt(dt.Rows[i]["GalleryID"]);
                            item.GalleryName = Func.ToStr(dt.Rows[i]["GalleryName"]);
                            item.GalleryDate = Func.ToDateTime(dt.Rows[i]["GalleryDate"]);
                            item.GalleryType = Func.ToInt(dt.Rows[i]["GalleryType"]);
                            item.Author = Func.ToStr(dt.Rows[i]["Author"]);
                            item.Tags =  Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            item.Category = Func.ToInt(dt.Rows[i]["Category"]);
                            item.GalleryDesc = Func.ToStr(dt.Rows[i]["GalleryDesc"]);
                            item.IsFeatured = Func.ToInt(dt.Rows[i]["IsFeatured"]);
                            item.GalleryID = Func.ToInt(dt.Rows[i]["GalleryID"]);
                            item.CoverUrl = Func.ToStr(dt.Rows[i]["CoverUrl"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd any Gallerys";
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Gallerys list";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Gallery List", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        public Models.Gallery GalleryDetail(int GalleryID)
        {
            Models.Gallery model = new Models.Gallery();
            model.CoverUrl = "";

            try
            {
                Result res = AdminDBContext.GetGalleryDetail(GalleryID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        model.GalleryID = Func.ToInt(dt.Rows[0]["GalleryID"]);
                        model.GalleryName = Func.ToStr(dt.Rows[0]["GalleryName"]);
                        model.GalleryDate = Func.ToDateTime(dt.Rows[0]["GalleryDate"]);
                        model.GalleryType = Func.ToInt(dt.Rows[0]["GalleryType"]);
                        model.Author = Func.ToStr(dt.Rows[0]["Author"]);
                        model.Tags = Func.ToStr(dt.Rows[0]["Tags"]).Trim(';').Split(';');
                        model.Category = Func.ToInt(dt.Rows[0]["Category"]);
                        model.GalleryDesc = Func.ToStr(dt.Rows[0]["GalleryDesc"]);
                        model.IsFeatured = Func.ToInt(dt.Rows[0]["IsFeatured"]);
                        model.GalleryID = Func.ToInt(dt.Rows[0]["GalleryID"]);
                        model.CoverUrl = Func.ToStr(dt.Rows[0]["CoverUrl"]);
                    }
                    else
                    {
                        ViewBag.Result = "Not found Gallery, GalleryID:" + GalleryID.ToString();
                    }

                    dt = ((DataSet)res.Data).Tables[1];
                    GalleryPhoto photo;
                    model.GalleryPhotoViewModel = new GalleryPhotoViewModel();
                    model.GalleryPhotoViewModel.List = new List<GalleryPhoto>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            photo = new GalleryPhoto();
                            photo.PhotoID = Func.ToInt(dt.Rows[i]["PhotoID"]);
                            photo.PhotoName = CultureHelper.GetRes("gallery_photo", "photoname", Func.ToStr(photo.PhotoID), CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["PhotoName"]));
                            photo.PhotoDesc = CultureHelper.GetRes("gallery_photo", "photodesc", Func.ToStr(photo.PhotoID), CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["PhotoDesc"]));
                            photo.PhotoUrl = Func.ToStr(dt.Rows[i]["PhotoUrl"]);
                            photo.IsVideo = Func.ToInt(dt.Rows[i]["IsVideo"]);
                            photo.DateTaken = Func.ToStr(dt.Rows[i]["DateTaken"]);
                            photo.Author = Func.ToStr(dt.Rows[i]["Author"]);
                            model.GalleryPhotoViewModel.List.Add(photo);
                        }
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Gallery,  GalleryID:" + GalleryID.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Gallery, GalleryID:" + GalleryID.ToString(), ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }

        //************************* Photos ****************************//
        #region Photos

        public ActionResult PhotoList(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }


            return PartialView("Photo", GetPhotoList(id));
        }

        public ActionResult PhotoNew(int id)
        {

            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            GalleryPhotoViewModel model = GetPhotoList(id);
            model.CurrentPhoto = new Models.GalleryPhoto();
            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return PartialView("Photo", model);
        }
        [HttpPost]
        public ActionResult PhotoInsert(int GalleryID, Models.GalleryPhoto CurrentPhoto, HttpPostedFileBase pfile, HttpPostedFileBase pfileSmall)
        {
            GalleryPhotoViewModel model;
            CurrentPhoto.GalleryID = GalleryID;

            string filename = "";
            bool haveFile = pfile != null && pfile.ContentLength > 0;
            // 1. Check previous file exist
            if (haveFile)
            {

                // 3. If not exist generate new name with uploaded file extension
                filename =  @"Gallery\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(pfile.FileName);
                CurrentPhoto.PhotoUrl = filename.Replace(@"\", @"/");
            }
            // 4. Execute DB process
            Result res = AdminDBContext.PhotoInsert(CurrentPhoto);
            model = GetPhotoList(GalleryID);
            if (res.Succeed)
            {
                int id = Func.ToInt(res.Data);
                Session["Message"] = "Successfully added";
                if (haveFile)
                {
                    res = Helper.SaveFile(filename, pfile);
                    if (!res.Succeed)
                    {
                        ViewBag.Result = "Successfully updated db, but error occured while saving file:" + res.Desc;
                    }
                    if (pfileSmall != null && pfileSmall.ContentLength > 0)
                    {
                        res = Helper.SaveFile(filename.Replace("_big", "_small"), pfileSmall);
                    }
                }
                return RedirectToAction("PhotoEdit", "gallery", new { id = id });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                ViewBag.StartScript = "InitListBox();";
                return PartialView("Photo", model);
            }
        }
        public ActionResult PhotoEdit(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.GalleryPhoto photo = PhotoDetail(id);
            GalleryPhotoViewModel model = GetPhotoList(photo.GalleryID);

            model.PhotoID = id;
            model.CurrentPhoto = photo;
            model.DisplayMode = "EditOnly";
            ViewBag.StartScript = "InitListBox();";
            return PartialView("photo", model);
        }

        [HttpPost]
        public ActionResult PhotoUpdate(int GalleryID, Models.GalleryPhoto CurrentPhoto, HttpPostedFileBase pfile, HttpPostedFileBase pfileSmall)
        {
            GalleryPhotoViewModel model;
            CurrentPhoto.GalleryID = GalleryID;

            string filename = "";
            string filenameSmall = "";
            bool haveFile = pfile != null && pfile.ContentLength > 0;
            bool haveFileSmall = (pfileSmall != null && pfileSmall.ContentLength > 0 && Func.ToStr(CurrentPhoto.PhotoUrl) != "");
            // 1. Check previous file exist
            if (haveFile)
            {
                if (Func.ToStr(CurrentPhoto.PhotoUrl) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = @"Gallery\" + Path.GetFileNameWithoutExtension(CurrentPhoto.PhotoUrl) + Path.GetExtension(pfile.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = @"Gallery\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(pfile.FileName);
                }
                CurrentPhoto.PhotoUrl = filename.Replace(@"\", @"/");
            }
            if (haveFileSmall)
            {
                // 2. If exist save exact name with uploaded file extension
                filenameSmall = @"Gallery\" + Path.GetFileNameWithoutExtension(CurrentPhoto.PhotoUrl).Replace("_big", "_small") + Path.GetExtension(pfileSmall.FileName);
            }
            Result res = AdminDBContext.PhotoUpdate(CurrentPhoto);
            if (res.Succeed)
            {
                if (haveFile)
                {
                    res = Helper.SaveFile(filename, pfile);
                    if (!res.Succeed)
                    {
                        ViewBag.Result = "Successfully updated db, but error occured while saving file:" + res.Desc;
                    }
                }
                if (haveFileSmall)
                {
                    res = Helper.SaveFile(filenameSmall, pfileSmall);
                }
            }
            model = GetPhotoList(GalleryID);
            model.DisplayMode = "EditOnly";
            model.PhotoID = CurrentPhoto.PhotoID;
            foreach (Models.GalleryPhoto item in model.List)
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
            ViewBag.StartScript = "InitListBox();";
            return PartialView("Photo", model);
        }

        public ActionResult PhotoDelete(int id)
        {
            Models.GalleryPhoto photo = PhotoDetail(id);

            Result res = AdminDBContext.PhotoDelete(photo);
            GalleryPhotoViewModel model = GetPhotoList(photo.GalleryID); ;
            if (res.Succeed)
            {
                Helper.DeleteFile(photo.PhotoUrl);
                model.CurrentPhoto = null;
                ViewBag.Result = "Successfully Deleted.";
            }
            else
                ViewBag.Result = res.Desc;
            model.DisplayMode = "ReadOnly";
            return PartialView("Photo", model);
        }

        private GalleryPhotoViewModel GetPhotoList(int GalleryID)
        {
            GalleryPhotoViewModel model = new GalleryPhotoViewModel();
            model.List = new List<BabySpa.Areas.Admin.Models.GalleryPhoto>();
            model.GalleryID = GalleryID;
            model.DisplayMode = "ReadOnly";
            try
            {
                Result res = AdminDBContext.GetPhotoList(GalleryID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    BabySpa.Areas.Admin.Models.GalleryPhoto item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.GalleryPhoto();

                            item.GalleryID = Func.ToInt(dt.Rows[i]["GalleryID"]);
                            item.PhotoID = Func.ToInt(dt.Rows[i]["PhotoID"]);
                            item.PhotoName = Func.ToStr(dt.Rows[i]["PhotoName"]);
                            item.PhotoDesc = Func.ToStr(dt.Rows[i]["PhotoDesc"]);
                            item.PhotoUrl = Func.ToStr(dt.Rows[i]["PhotoUrl"]);
                            item.IsVideo = Func.ToInt(dt.Rows[i]["IsVideo"]);
                            item.Tags = Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd photos for gallery, GalleryID:" + GalleryID;
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get photo list, GalleryID:" + GalleryID;
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get PhotoList, GalleryID:" + GalleryID, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        private Models.GalleryPhoto PhotoDetail(int photoID)
        {
            Models.GalleryPhoto model = new Models.GalleryPhoto();
            try
            {
                Result res = AdminDBContext.GetPhotoDetail(photoID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        model.PhotoID = photoID;
                        model.GalleryID = Func.ToInt(dt.Rows[0]["GalleryID"]);
                        model.PhotoName = Func.ToStr(dt.Rows[0]["PhotoName"]);
                        model.PhotoDesc = Func.ToStr(dt.Rows[0]["PhotoDesc"]);
                        model.PhotoUrl = Func.ToStr(dt.Rows[0]["PhotoUrl"]);
                        model.IsVideo = Func.ToInt(dt.Rows[0]["IsVideo"]);
                        model.Tags = Func.ToStr(dt.Rows[0]["Tags"]).Trim(';').Split(';');
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd photo for gallery, photoid:" + photoID.ToString();
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

        #endregion
    }
}
