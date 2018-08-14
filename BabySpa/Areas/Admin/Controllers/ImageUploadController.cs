using BabySpa.Areas.Admin.Models;
using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class ImageUploadController : Controller
    {
        // GET: Admin/ImageUpload
        public ActionResult Index()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }


            return View("ImageUpload", GetImageList());
        }
        public ActionResult New()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            ImageViewModel model = GetImageList();
            model.CurrentImage = new Models.Image();
            model.CurrentImage.ImageUrl = "";

            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return View("ImageUpload", model);
        }
        [HttpPost]
        public ActionResult Insert(Models.Image CurrentImage, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            ImageViewModel model;
            string filename = "";
            bool haveFile = false;

            haveFile = file != null && file.ContentLength > 0;
            if (haveFile)
            {
                CurrentImage.ImageName = Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                filename = @"Upload\" + CurrentImage.ImageName;
                CurrentImage.ImageUrl = filename.Replace(@"\", @"/");
            }
            // 4. Execute DB process
            model = GetImageList();

            if (haveFile)
            {
               Result res = Helper.SaveFile(filename, file);
                if (!res.Succeed)
                {
                    ViewBag.Result = "Error occured while saving file:" + res.Desc;
                }
                if (fileSmall != null && fileSmall.ContentLength > 0)
                {
                    res = Helper.SaveFile(filename.Replace("_big", "_small"), fileSmall);
                }
            }
            return RedirectToAction("Edit", "ImageUpload", new { id =CurrentImage.ImageName.Replace('.', '~')});

        }
        public ActionResult Edit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            id = id.Replace('~', '.');
            Models.Image Image = ImageDetail(id);
            ImageViewModel model = GetImageList();

            model.CurrentImage = Image;
            model.DisplayMode = "EditOnly";
            return View("ImageUpload", model);
        }
        public ActionResult Update(Models.Image CurrentImage, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            ImageViewModel model;
            string filename = "";
            string filenameSmall = "";
            bool haveFile = false;
            bool haveFileSmall = false;

            haveFile = file != null && file.ContentLength > 0;
            haveFileSmall = (fileSmall != null && fileSmall.ContentLength > 0);
            if (haveFile)
            {
                if (Func.ToStr(CurrentImage.ImageUrl) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = @"Image\" + Path.GetFileNameWithoutExtension(CurrentImage.ImageUrl) + Path.GetExtension(file.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = @"Image\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                }
                CurrentImage.ImageName = filename.Replace(@"Image\", "");
                CurrentImage.ImageUrl = filename.Replace(@"\", @"/");
            }
            // If big photo have and small image uploaded
            if (haveFileSmall && Func.ToStr(CurrentImage.ImageUrl) != "")
            {
                haveFileSmall = true;
                filenameSmall = @"Image\" + Path.GetFileNameWithoutExtension(CurrentImage.ImageUrl).Replace("_big", "_small") + Path.GetExtension(fileSmall.FileName);
            }
            // 4. Execute DB process
            Result res = new Result();
            if (haveFile)
            {
                res = Helper.SaveFile(filename, file);
            }

            if (haveFileSmall)
            {
                res = Helper.SaveFile(filenameSmall, fileSmall);
            }
            model = GetImageList();
            model.DisplayMode = "EditOnly";
            model.ImageName = CurrentImage.ImageName;
            foreach (Models.Image item in model.List)
            {
                if (item.ImageName == model.ImageName)
                {
                    model.CurrentImage = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return View("ImageUpload", model);
        }
        public ActionResult Delete(string id)
        {
            id = id.Replace('~', '.');
            Models.Image Image = ImageDetail(id);
            Result res = Helper.DeleteFile(@"Upload\"+id);

            ImageViewModel model = GetImageList();
            if (res.Succeed)
            {
                model.CurrentImage = null;
                ViewBag.Result = "Successfully Deleted.";
                model.DisplayMode = "ReadOnly";
            }
            else
            {
                model.CurrentImage = Image;
                model.ImageName = id;
                ViewBag.Result = res.Desc;
                model.DisplayMode = "EditOnly";
            }
            return View("ImageUpload", model);

        }
        public ImageViewModel GetImageList()
        {
            ImageViewModel model = new ImageViewModel();
            model.List = new List<BabySpa.Areas.Admin.Models.Image>();
            Image img;
            model.DisplayMode = "ReadOnly";
            try
            {
                IEnumerable<string> files =  Directory.EnumerateFiles(Server.MapPath("~/Content/tour/Upload"));
                foreach (var path in files)
                {
                    if (!path.Contains("_small."))
                    {
                    img = new Image();
                    img.ImageUrl = @"Upload\" + Path.GetFileName(path);
                    img.ImageName =  Path.GetFileName(path);
                    img.ImageExt = Path.GetExtension(path);
                    model.List.Add(img);
                    }
                }

            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Image List", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        public Models.Image ImageDetail(string name)
        {
            Models.Image model = new Models.Image();
            model.ImageUrl = "";

            try
            {
                if (System.IO.File.Exists(Main.apppath + @"\Content\tour\Upload\" + name))
                {
                    model.ImageUrl = @"Upload\" + name;
                    model.ImageName= name;
                    model.ImageExt = Path.GetExtension(name);
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Image, name:" + name, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }

    }
}