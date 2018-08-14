using BabySpa.Areas.Admin.Models;
using BabySpa.Core;
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
    public class BlogController : Controller
    {
        // GET: Admin/Blog
        public ActionResult Index()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }


            return View("Blog", GetBlogList());
        }

        // GET: Admin/Blog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Blog/Create
        public ActionResult New()
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            BlogViewModel model = GetBlogList();
            model.CurrentBlog = new Models.Blog();
            model.CurrentBlog.CoverUrl = "";

            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return View("Blog", model);
        }

        // POST: Admin/Blog/Create
        [HttpPost]
        public ActionResult Insert(Models.Blog CurrentBlog, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            BlogViewModel model;
            string filename = "";
            bool haveFile = false;
            
                haveFile = file != null && file.ContentLength > 0;
                if (haveFile)
                {
                    filename = @"Blog\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                    CurrentBlog.CoverUrl = filename.Replace(@"\", @"/");
                }
            // 4. Execute DB process
            Result res = AdminDBContext.BlogInsert(CurrentBlog);
            model = GetBlogList();
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
                return RedirectToAction("Edit", "Blog", new { id = id });
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                return View("Blog", model);
            }
        }

        // GET: Admin/Blog/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.Blog Blog = BlogDetail(id);
            BlogViewModel model = GetBlogList();

            model.CurrentBlog= Blog;
            model.DisplayMode = "EditOnly";
            return View("Blog", model);
        }

        // POST: Admin/Blog/Edit/5
        [HttpPost]
        public ActionResult Update(Models.Blog CurrentBlog, HttpPostedFileBase file, HttpPostedFileBase fileSmall)
        {
            BlogViewModel model;
            string filename = "";
            string filenameSmall = "";
            bool haveFile = false;
            bool haveFileSmall = false;

            haveFile = file != null && file.ContentLength > 0;
            haveFileSmall = (fileSmall != null && fileSmall.ContentLength > 0);
            if (haveFile)
            {
                if (Func.ToStr(CurrentBlog.CoverUrl) != "")
                {
                    // 2. If exist save exact name with uploaded file extension
                    filename = @"Blog\" + Path.GetFileNameWithoutExtension(CurrentBlog.CoverUrl) + Path.GetExtension(file.FileName);
                }
                else
                {
                    // 3. If not exist generate new name with uploaded file extension
                    filename = @"Blog\" + Guid.NewGuid().ToString() + "_big" + Path.GetExtension(file.FileName);
                }
                CurrentBlog.CoverUrl = filename.Replace(@"\", @"/");
            }
            // If big photo have and small image uploaded
            if (haveFileSmall && Func.ToStr(CurrentBlog.CoverUrl) != "")
            {
                haveFileSmall = true;
                filenameSmall = @"Blog\" + Path.GetFileNameWithoutExtension(CurrentBlog.CoverUrl).Replace("_big", "_small") + Path.GetExtension(fileSmall.FileName);
            }
            // 4. Execute DB process
            Result res = AdminDBContext.BlogUpdate(CurrentBlog);
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
            model = GetBlogList();
            model.DisplayMode = "EditOnly";
            model.BlogID = CurrentBlog.BlogID;
            foreach (Models.Blog item in model.List)
            {
                if (item.BlogID == model.BlogID)
                {
                    model.CurrentBlog = item;
                    break;
                }
            }
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return View("Blog", model);
        }

        // GET: Admin/Blog/Delete/5
        public ActionResult Delete(int id)
        {

            Models.Blog Blog = BlogDetail(id);
            Result res = AdminDBContext.BlogDelete(id);
            BlogViewModel model = GetBlogList();
            if (res.Succeed)
            {
                for (int i = 0; i < Blog.CoverUrl.Count(); i++)
                {
                    Helper.DeleteFile(Blog.CoverUrl);
                }
                model.CurrentBlog = null;
                ViewBag.Result = "Successfully Deleted.";
                model.DisplayMode = "ReadOnly";
            }
            else
            {
                model.CurrentBlog = Blog;
                model.BlogID = id;
                ViewBag.Result = res.Desc;
            model.DisplayMode = "EditOnly";
            }
            return View("Blog", model);
        }

        public BlogViewModel GetBlogList()
        {
            BlogViewModel model = new BlogViewModel();
            model.List = new List<BabySpa.Areas.Admin.Models.Blog>();
            model.DisplayMode = "ReadOnly";
            try
            {
                Result res = AdminDBContext.GetBlogList();
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    BabySpa.Areas.Admin.Models.Blog item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.Blog();
                            item.CoverUrl ="";

                            item.BlogID = Func.ToInt(dt.Rows[i]["BlogID"]);
                            item.BlogName = Func.ToStr(dt.Rows[i]["BlogName"]);
                            item.BlogDate = Func.ToDateTime(dt.Rows[i]["BlogDate"]);
                            item.BlogType = Func.ToInt(dt.Rows[i]["BlogType"]);
                            item.Author = Func.ToStr(dt.Rows[i]["Author"]);
                            item.Tags =  Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            item.Category = Func.ToInt(dt.Rows[i]["Category"]);
                            item.BlogContent = Func.ToStr(dt.Rows[i]["BlogContent"]);
                            item.IsFeatured = Func.ToInt(dt.Rows[i]["IsFeatured"]);
                            item.TOURID = Func.ToStr(dt.Rows[i]["TOURID"]);
                            item.CoverUrl = Func.ToStr(dt.Rows[i]["CoverUrl"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        ViewBag.Result = "Not registerd any Blogs";
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Blogs list";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Blog List", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        public Models.Blog BlogDetail(int BlogID)
        {
            Models.Blog model = new Models.Blog();
            model.CoverUrl = "";

            try
            {
                Result res = AdminDBContext.GetBlogDetail(BlogID);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        model.BlogID = Func.ToInt(dt.Rows[0]["BlogID"]);
                        model.BlogName = Func.ToStr(dt.Rows[0]["BlogName"]);
                        model.BlogDate = Func.ToDateTime(dt.Rows[0]["BlogDate"]);
                        model.BlogType = Func.ToInt(dt.Rows[0]["BlogType"]);
                        model.Author = Func.ToStr(dt.Rows[0]["Author"]);
                        model.Tags = Func.ToStr(dt.Rows[0]["Tags"]).Trim(';').Split(';');
                        model.Category = Func.ToInt(dt.Rows[0]["Category"]);
                        model.BlogContent = Func.ToStr(dt.Rows[0]["BlogContent"]);
                        model.IsFeatured = Func.ToInt(dt.Rows[0]["IsFeatured"]);
                        model.TOURID = Func.ToStr(dt.Rows[0]["TOURID"]);
                        model.CoverUrl = Func.ToStr(dt.Rows[0]["CoverUrl"]);
                    }
                    else
                    {
                        ViewBag.Result = "Not found Blog, BlogID:" + BlogID.ToString();
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Blog,  BlogID:" + BlogID.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Blog, BlogID:" + BlogID.ToString(), ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }
    }
}
