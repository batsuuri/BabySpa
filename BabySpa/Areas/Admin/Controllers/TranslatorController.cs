using EM.Areas.Admin.Models;
using EM.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EM.Areas.Admin.Controllers
{
    [Authorize]
    public class TranslatorController : Controller
    {
        // GET: Admin/Translator
        public ActionResult Index(TranslatorViewModel model)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            Models.TranslatorValue item = new TranslatorValue();
            // TODO: Fill item
            item.ColumnName = Request.QueryString["c"];
            item.TableName = Request.QueryString["t"];
            item.KeyField = Request.QueryString["k"];
            if (Func.ToStr(item.ColumnName).Contains("."))
            {
                item.ColumnName = item.ColumnName.Split('.')[1];
            }
            return PartialView("Translator", GetTranslatorList(item));
        }

        // GET: Admin/Translator/Create
        public ActionResult New(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }

            Models.TranslatorValue item = new TranslatorValue();
            // TODO: Fill item
            item.KeyField = id;
            item.TableName = Request.QueryString["t"];
            item.ColumnName = Request.QueryString["c"];
            TranslatorViewModel model = GetTranslatorList(item);
            model.CurrentEntry = new Models.TranslatorValue();

            model.DisplayMode = "WriteOnly";
            ViewBag.Result = "";
            return PartialView("Translator", model);
        }

        // POST: Admin/Translator/Create
        [HttpPost]
        public ActionResult Insert(Models.TranslatorViewModel model)
        {

            // 4. Execute DB process
            model.CurrentEntry.TableName = model.TableName;
            model.CurrentEntry.ColumnName = model.ColumnName;
            model.CurrentEntry.KeyField = model.KeyField;
            Result res = AdminDBContext.TranslatorInsert(model.CurrentEntry);

            if (res.Succeed)
            {
                int id = Func.ToInt(res.Data);
                ViewBag.Resul = "Successfully added";
                TranslatorValue temp = model.CurrentEntry;
                model = GetTranslatorList(model.CurrentEntry);
                model.CurrentEntry = temp;
                model.DisplayMode = "EditOnly";
                return PartialView("Translator", model);
            }
            else
            {
                model.DisplayMode = "EditOnly";
                ViewBag.Result = res.Desc;
                model = GetTranslatorList(model.CurrentEntry);
                return PartialView("Translator", model);
            }
        }

        // GET: Admin/Translator/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["Message"] != null)
            {
                ViewBag.Result = Session["Message"];
                Session["Message"] = null;
            }
            TranslatorValue model = new TranslatorValue();
            model.Lang = id;
            model.KeyField = Request.QueryString["k"];
            model.TableName = Request.QueryString["t"];
            model.ColumnName = Request.QueryString["c"];
            model = TranslatorDetail(model);
            TranslatorViewModel viewmodel = GetTranslatorList(model);

            viewmodel.CurrentEntry = model;
            viewmodel.DisplayMode = "EditOnly";
            return PartialView("Translator", viewmodel);
        }

        // POST: Admin/Translator/Edit/5
        [HttpPost]
        public ActionResult Update(TranslatorViewModel model)
        {
            // 4. Execute DB process
            model.CurrentEntry.TableName = model.TableName;
            model.CurrentEntry.ColumnName = model.ColumnName;
            model.CurrentEntry.KeyField = model.KeyField;
            Result res = AdminDBContext.TranslatorUpdate(model.CurrentEntry);
            TranslatorValue temp = model.CurrentEntry;
            model = GetTranslatorList(model.CurrentEntry);
            model.CurrentEntry = temp;
            model.DisplayMode = "EditOnly";
            
            if (!res.Succeed)
            {
                ViewBag.Result = res.Desc;
            }
            else
                ViewBag.Result = "Successfully updated";
            return PartialView("Translator", model);
        }

        // GET: Admin/Translator/Delete/5
        public ActionResult Delete(string id)
        {
            TranslatorValue model = new TranslatorValue();
            model.Lang = id;
            model.KeyField = Request.QueryString["k"];
            model.TableName = Request.QueryString["t"];
            model.ColumnName = Request.QueryString["c"];
            TranslatorValue temp = model;

            Result res = AdminDBContext.TranslatorDelete(model);
            TranslatorViewModel viewModel = GetTranslatorList(model);
            if (res.Succeed)
            {

                viewModel.CurrentEntry = null;
                ViewBag.Result = "Successfully Deleted.";
                viewModel.DisplayMode = "ReadOnly";
            }
            else
            {
                viewModel.CurrentEntry = temp;
                ViewBag.Result = res.Desc;
                viewModel.DisplayMode = "EditOnly";
            }
            return PartialView("Translator", viewModel);
        }

        private TranslatorViewModel GetTranslatorList(Models.TranslatorValue CurrentEntry)
        {
            TranslatorViewModel model = new TranslatorViewModel();
            model.TableName = CurrentEntry.TableName;
            model.ColumnName = CurrentEntry.ColumnName;
            model.KeyField = CurrentEntry.KeyField;
            model.List = new List<EM.Areas.Admin.Models.TranslatorValue>();
            model.DisplayMode = "ReadOnly";
            try
            {
                Result res = AdminDBContext.GetTranslatorList(CurrentEntry.TableName, CurrentEntry.ColumnName, CurrentEntry.KeyField);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    EM.Areas.Admin.Models.TranslatorValue item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new Models.TranslatorValue();

                            item.TableName = Func.ToStr(dt.Rows[i]["TableName"]);
                            item.ColumnName = Func.ToStr(dt.Rows[i]["ColumnName"]);
                            item.KeyField = Func.ToStr(dt.Rows[i]["KeyField"]);
                            item.Lang = Func.ToStr(dt.Rows[i]["Lang"]);
                            item.TextValue = Func.ToStr(dt.Rows[i]["TextValue"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        //ViewBag.Result = "Not registerd any Translators";
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Translators list";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Translator List", ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            model.LangList = new List<SelectListItem>();
            //model.LangList.Add(new SelectListItem() { Text = "English", Value = "en" });
            model.LangList.Add(new SelectListItem() { Text = "Japanese", Value = "jp" });
            model.LangList.Add(new SelectListItem() { Text = "Korean", Value = "ko" });
            model.LangList.Add(new SelectListItem() { Text = "Chinese", Value = "zh" });
            model.LangList.Add(new SelectListItem() { Text = "French", Value = "fr" });
            return model;
        }
        private Models.TranslatorValue TranslatorDetail(Models.TranslatorValue item)
        {
            Models.TranslatorValue model = new Models.TranslatorValue();

            try
            {
                Result res = AdminDBContext.GetTranslatorDetail(item.TableName,item.ColumnName,item.KeyField,item.Lang);
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        model.TableName = Func.ToStr(dt.Rows[0]["TableName"]);
                        model.ColumnName = Func.ToStr(dt.Rows[0]["ColumnName"]);
                        model.KeyField = Func.ToStr(dt.Rows[0]["KeyField"]);
                        model.Lang = Func.ToStr(dt.Rows[0]["Lang"]);
                        model.TextValue= Func.ToStr(dt.Rows[0]["TextValue"]);
                    }
                    else
                    {
                        ViewBag.Result = "Not found Translator, TranslatorID:" + item.KeyField;
                    }
                }
                else
                {
                    ViewBag.Result = "Error occured when get Translator,  TranslatorID:" + item.KeyField;
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Translator, TranslatorID:" + item.KeyField, ex);
                ViewBag.Result = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }
    }
}
