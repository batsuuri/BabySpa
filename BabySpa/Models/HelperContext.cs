using BabySpa.Areas.Admin.Models;
using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Models
{
    public class HelperContext
    {

        public static BlogViewModel GetBlogList(BlogFilter filter)
        {
            BlogViewModel model = new BlogViewModel();
            Result res;
            model.List = new List<BabySpa.Areas.Admin.Models.Blog>();
            model.CalendarList = new List<BlogCalendar>();
            model.TagList = new List<BlogTags>();
            try
            {

                StringBuilder cond = new StringBuilder();
                if (Func.ToInt(filter.BlogID) > 0)
                {
                    cond.Append(" and b.blogid =" + Func.ToStr(Func.ToInt(filter.BlogID)));
                }
                else
                {
                    if (Func.ToInt(filter.CategoryID) > 0)
                    {
                        cond.Append(" and b.Category=" + Func.ToStr(Func.ToInt(filter.CategoryID)));
                    }
                    if (Func.ToInt(filter.BlogType) > 0)
                    {
                        cond.Append(" and b.BlogType=" + Func.ToStr(Func.ToInt(filter.BlogType)));
                    }
                    if (filter.Author != null && filter.Author != "")
                    {
                        cond.Append(" and (B.Author like '%" + filter.Author + "%')");
                    }
                    if (filter.BlogDate != null && filter.BlogDate != "")
                    {
                        cond.Append(" and CONVERT(CHAR(7),blogdate,20) = '" + filter.BlogDate+"'");
                    }
                    if (filter.Tags !=null && filter.Tags.Length>0)
                    {
                        string temp=" and ( ";
                        for (int i = 0; i < filter.Tags.Length; i++)
                        {
                            temp = temp + "b.Tags  like '%;" + filter.Tags[i] + ";%' or";
                        }
                        cond.Append(temp.TrimEnd("or".ToCharArray())+" )");
                    }

                }
                string sql = cond.ToString();
                if (sql.Length > 0)
                {
                    sql = " where " + sql.TrimStart(" and".ToCharArray());
                }
                sql = @"begin select b.BlogID, B.BlogName, BlogDate, 
                            CONVERT(CHAR(7),blogdate,20) dateValue, 
                            CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120) DisplayText, 
                            Tags, Category, BlogType, IsFeatured, 
                            case when len(BlogContent)>=800 then left(BlogContent, 800) else BlogContent end BlogContent,
                            Author, TourID, CoverUrl
                            from Blog b "
                        + sql + @" order by BlogDate desc;
                        select* from (
                                    select CONVERT(CHAR(7), blogdate, 20) DateValue,
                                        CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120) DisplayText,
                                        COUNT(BlogID) BCount

                                    from BLOG

                                    group by CONVERT(CHAR(7), blogdate, 20),
                                            CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120))b
                                order by DateValue Desc; 
                            select DataID, DataName, Count(DataID)TCount from DATA_BASIC d
                            ,BLOG b 
                            where DataType= 'blogtag' and b.Tags like '%;' + cast(d.DataID as nvarchar) +';%'  
                            group by DataID, DataName;
                            end;";

                res = Main.DataSetExecute(sql.ToString());

                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];

                    BabySpa.Areas.Admin.Models.Blog item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int start = 0;
                        if (filter.Page>0)
                        {
                            start = filter.Page * 10 - 1;
                        }
                        int end = start + 10;
                        if (end > dt.Rows.Count)
                        {
                            end = dt.Rows.Count;
                            model.HaveNext = false;
                        }
                        else
                            model.HaveNext = true;

                        for (int i = start; i < end; i++)
                        {
                            item = new Blog();

                            item.BlogID = Func.ToInt(dt.Rows[i]["BlogID"]);
                            item.BlogName = Func.ToStr(dt.Rows[i]["BlogName"]);
                            item.BlogDate = Func.ToDateTime(dt.Rows[i]["BlogDate"]);
                            item.BlogType = Func.ToInt(dt.Rows[i]["BlogType"]);
                            item.Author = Func.ToStr(dt.Rows[i]["Author"]);
                            item.Tags = Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
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
                        res.Desc= "Not registerd any Blogs";
                    }
                    dt = ((DataSet)res.Data).Tables[1];
                    BlogCalendar cal;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            cal = new BlogCalendar();
                            cal.DateValue = Func.ToStr(dt.Rows[i]["DateValue"]);
                            cal.DisplayText = Func.ToStr(dt.Rows[i]["DisplayText"]);
                            cal.Count = Func.ToInt(dt.Rows[i]["BCount"]);
                            model.CalendarList.Add(cal);
                        }
                    }
                    dt = ((DataSet)res.Data).Tables[2];
                    BlogTags tag;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            tag = new BlogTags();
                            tag.TagID = Func.ToInt(dt.Rows[i]["DataID"]);
                            tag.TagName = CultureHelper.GetRes("data_basic", "dataname", "blogtag" + App.keydelm + tag.TagID, CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["DataName"]));
                            tag.Count = Func.ToInt(dt.Rows[i]["TCount"]);
                            model.TagList.Add(tag);
                        }
                    }
                }
                else
                {
                    res.Desc = "Error occured when get Blogs list";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Blog List", ex);
                res.Desc = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        public static BlogViewModel BlogDetail(int BlogID)
        {
            BlogViewModel model = new BlogViewModel();
            model.TagList = new List<BlogTags>();
            model.CalendarList = new List<BlogCalendar>();

            Result res;
            try
            {
                string sql = @"begin select b.BlogID, B.BlogName, BlogDate, 
                            CONVERT(CHAR(7),blogdate,20) dateValue, 
                            CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120) DisplayText, 
                            Tags, Category, BlogType, IsFeatured, BlogContent, author, TourID, CoverUrl
                            from Blog b where blogid=@BlogID
                        order by BlogDate desc;
                        select* from (
                                    select CONVERT(CHAR(7), blogdate, 20) DateValue,
                                        CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120) DisplayText,
                                        COUNT(BlogID) BCount

                                    from BLOG

                                    group by CONVERT(CHAR(7), blogdate, 20),
                                            CONVERT(CHAR(4), blogdate, 100) + CONVERT(CHAR(4), blogdate, 120))b
                                order by DateValue Desc;
                         select DataID, DataName, Count(DataID)TCount 
                                        from DATA_BASIC d ,BLOG b 
                                where DataType= 'blogtag' and b.Tags like '%;' + cast(d.DataID as nvarchar) +';%'  
                                group by DataID, DataName;
                        end;";
                SqlParameter pBlogID = new SqlParameter("@BlogID", SqlDbType.Int);
                pBlogID.Value = BlogID;

                res = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pBlogID});
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    model.CurrentBlog = new Blog();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.CurrentBlog.BlogID = Func.ToInt(dt.Rows[0]["BlogID"]);
                        model.CurrentBlog.BlogName = Func.ToStr(dt.Rows[0]["BlogName"]);
                        model.CurrentBlog.BlogDate = Func.ToDateTime(dt.Rows[0]["BlogDate"]);
                        model.CurrentBlog.BlogType = Func.ToInt(dt.Rows[0]["BlogType"]);
                        model.CurrentBlog.Author = Func.ToStr(dt.Rows[0]["Author"]);
                        model.CurrentBlog.Tags = Func.ToStr(dt.Rows[0]["Tags"]).Trim(';').Split(';');
                        model.CurrentBlog.Category = Func.ToInt(dt.Rows[0]["Category"]);
                        model.CurrentBlog.BlogContent = Func.ToStr(dt.Rows[0]["BlogContent"]);
                        model.CurrentBlog.IsFeatured = Func.ToInt(dt.Rows[0]["IsFeatured"]);
                        model.CurrentBlog.TOURID = Func.ToStr(dt.Rows[0]["TOURID"]);
                        model.CurrentBlog.CoverUrl = Func.ToStr(dt.Rows[0]["CoverUrl"]);
                    }
                    else
                    {
                        res.Desc = "Not found Blog, BlogID:" + BlogID.ToString();
                    }

                    dt = ((DataSet)res.Data).Tables[1];
                    BlogCalendar cal;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            cal = new BlogCalendar();
                            cal.DateValue = Func.ToStr(dt.Rows[i]["DateValue"]);
                            cal.DisplayText = Func.ToStr(dt.Rows[i]["DisplayText"]);
                            cal.Count = Func.ToInt(dt.Rows[i]["BCount"]);
                            model.CalendarList.Add(cal);
                        }
                    }

                    dt = ((DataSet)res.Data).Tables[2];
                    BlogTags tag;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            tag = new BlogTags();
                            tag.TagID = Func.ToInt(dt.Rows[i]["DataID"]);
                            tag.TagName = CultureHelper.GetRes("data_basic", "dataname", "blogtag" + App.keydelm + tag.TagID, CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["DataName"]));
                            tag.Count = Func.ToInt(dt.Rows[i]["TCount"]);
                            model.TagList.Add(tag);
                        }
                    }
                }
                else
                {
                    res.Desc = "Error occured when get Blog,  BlogID:" + BlogID.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Blog, BlogID:" + BlogID.ToString(), ex);
                res.Desc = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }

        public static GalleryViewModel GetGalleryList(GalleryFilter filter)
        {
            GalleryViewModel model = new GalleryViewModel();
            Result res;
            model.List = new List<BabySpa.Areas.Admin.Models.Gallery>();
            model.GategoryList = new List<GalleryCategory>();
            model.TagList = new List<GalleryTags>();
            try
            {

                StringBuilder cond = new StringBuilder();
                if (Func.ToInt(filter.GalleryID) > 0)
                {
                    cond.Append(" and b.Galleryid =" + Func.ToStr(Func.ToInt(filter.GalleryID)));
                }
                else
                {
                    if (Func.ToInt(filter.CategoryID) > 0)
                    {
                        cond.Append(" and b.Category=" + Func.ToStr(Func.ToInt(filter.CategoryID)));
                    }
                    if (Func.ToInt(filter.GalleryType) > 0)
                    {
                        cond.Append(" and b.GalleryType=" + Func.ToStr(Func.ToInt(filter.GalleryType)));
                    }
                    if (filter.Author != null && filter.Author != "")
                    {
                        cond.Append(" and (B.Author like '%" + filter.Author + "%')");
                    }
                    if (filter.GalleryDate != null && filter.GalleryDate != "")
                    {
                        cond.Append(" and CONVERT(CHAR(7),Gallerydate,20) = '" + filter.GalleryDate + "'");
                    }
                    if (filter.Tags != null && filter.Tags.Length > 0)
                    {
                        string temp = " and ( ";
                        for (int i = 0; i < filter.Tags.Length; i++)
                        {
                            temp = temp + "b.Tags  like '%;" + filter.Tags[i] + ";%' or";
                        }
                        cond.Append(temp.TrimEnd("or".ToCharArray()) + " )");
                    }

                }
                string sql = cond.ToString();
                if (sql.Length > 0)
                {
                    sql = " where " + sql.TrimStart(" and".ToCharArray());
                }
                sql = @"begin select b.GalleryID, B.GalleryName, GalleryDate, 
                            CONVERT(CHAR(7),Gallerydate,20) dateValue, 
                            CONVERT(CHAR(4), Gallerydate, 100) + CONVERT(CHAR(4), Gallerydate, 120) DisplayText, 
                            Tags, Category, GalleryType, IsFeatured, 
                            case when len(GalleryDesc)>=750 then left(GalleryDesc, 750) + '...' else GalleryDesc end GalleryDesc,
                            Author, TourID, CoverUrl
                            from Gallery b "
                        + sql + @" order by GalleryDate desc;
                       select category,COUNT(photoid) BCount
                                from Gallery_photo p
                                right join GALLERY g on p.GalleryID = g.GalleryID
                                group by category;
                         select DataID, DataName, Count(DataID)TCount 
                                        from DATA_BASIC d ,Gallery_photo b 
                                where DataType= 'gallerytag' and b.Tags like '%;' + cast(d.DataID as nvarchar) +';%'  
                                group by DataID, DataName;
                        end;";

                res = Main.DataSetExecute(sql.ToString());

                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];

                    BabySpa.Areas.Admin.Models.Gallery item;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int start = 0;
                        if (filter.Page > 0)
                        {
                            start = filter.Page * 10 - 1;
                        }
                        int end = start + 10;
                        if (end > dt.Rows.Count)
                        {
                            end = dt.Rows.Count;
                            model.HaveNext = false;
                        }
                        else
                            model.HaveNext = true;

                        for (int i = start; i < end; i++)
                        {
                            item = new Gallery();

                            item.GalleryID = Func.ToInt(dt.Rows[i]["GalleryID"]);
                            item.GalleryName = Func.ToStr(dt.Rows[i]["GalleryName"]);
                            item.GalleryDate = Func.ToDateTime(dt.Rows[i]["GalleryDate"]);
                            item.GalleryType = Func.ToInt(dt.Rows[i]["GalleryType"]);
                            item.Author = Func.ToStr(dt.Rows[i]["Author"]);
                            item.Tags = Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            item.Category = Func.ToInt(dt.Rows[i]["Category"]);
                            item.GalleryDesc = Func.ToStr(dt.Rows[i]["GalleryDesc"]);
                            item.IsFeatured = Func.ToInt(dt.Rows[i]["IsFeatured"]);
                            item.TOURID = Func.ToStr(dt.Rows[i]["TOURID"]);
                            item.CoverUrl = Func.ToStr(dt.Rows[i]["CoverUrl"]);
                            model.List.Add(item);
                        }
                    }
                    else
                    {
                        res.Desc = "Not registerd any Gallerys";
                    }
                    dt = ((DataSet)res.Data).Tables[1];
                    GalleryCategory cat;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            cat = new GalleryCategory();
                            cat.CategoryID = Func.ToInt (dt.Rows[i]["Category"]);
                            cat.CategoryName = App.getBasicDataNameWithCulture("gallerycategory", Func.ToStr(cat.CategoryID));
                            cat.Count = Func.ToInt(dt.Rows[i]["BCount"]);
                            model.GategoryList.Add(cat);
                        }
                    }
                    dt = ((DataSet)res.Data).Tables[2];
                    GalleryTags tag;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            tag = new GalleryTags();
                            tag.TagID = Func.ToInt(dt.Rows[i]["DataID"]);
                            tag.TagName = CultureHelper.GetRes("data_basic", "dataname", "gallerytag" + App.keydelm + tag.TagID, CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["DataName"]));
                            tag.Count = Func.ToInt(dt.Rows[i]["TCount"]);
                            model.TagList.Add(tag);
                        }
                    }
                }
                else
                {
                    res.Desc = "Error occured when get Gallerys list";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Gallery List", ex);
                res.Desc = Helper.UN_EXPECTED_MSG;
            }

            return model;
        }
        public static GalleryViewModel GalleryDetail(int CategoryID)
        {
            GalleryViewModel model = new GalleryViewModel();
            model.TagList = new List<GalleryTags>();
            model.GategoryList = new List<GalleryCategory>();
            model.CurrentGallery = new Gallery();
            model.CurrentGallery.GalleryPhotoViewModel = new GalleryPhotoViewModel();
            model.CurrentGallery.GalleryPhotoViewModel.List = new List<GalleryPhoto>();
            GalleryPhoto photo;
            Result res;
            try
            {
                string sql = @"begin select top 1 b.GalleryID, B.GalleryName, GalleryDate, 
                            CONVERT(CHAR(7),Gallerydate,20) dateValue, 
                            CONVERT(CHAR(4), Gallerydate, 100) + CONVERT(CHAR(4), Gallerydate, 120) DisplayText, 
                            Tags, Category, GalleryType, IsFeatured, GalleryDesc, author, TourID, CoverUrl
                            from Gallery b where Category=@CategoryID
                        order by GalleryDate desc;
                        select category,COUNT(photoid) BCount
                                from Gallery_photo p
                                left join GALLERY g on p.GalleryID = g.GalleryID
                                group by category;
                         select DataID, DataName, Count(DataID)TCount 
                                        from DATA_BASIC d ,Gallery_photo b 
                                where DataType= 'gallerytag' and b.Tags like '%;' + cast(d.DataID as nvarchar) +';%'  
                                group by DataID, DataName;
                        select * From GALLERY_PHOTO P left join GALLERY G on p.GalleryID = G.GalleryID where G.Category = @CategoryID;
                        end;";
                SqlParameter pCategoryID = new SqlParameter("@CategoryID", SqlDbType.Int);
                pCategoryID.Value = CategoryID;

                res = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pCategoryID});
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    model.CurrentGallery = new Gallery();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.CurrentGallery.GalleryID = Func.ToInt(dt.Rows[0]["GalleryID"]);
                        model.CurrentGallery.GalleryName = Func.ToStr(dt.Rows[0]["GalleryName"]);
                        model.CurrentGallery.GalleryDate = Func.ToDateTime(dt.Rows[0]["GalleryDate"]);
                        model.CurrentGallery.GalleryType = Func.ToInt(dt.Rows[0]["GalleryType"]);
                        model.CurrentGallery.Author = Func.ToStr(dt.Rows[0]["Author"]);
                        model.CurrentGallery.Tags = Func.ToStr(dt.Rows[0]["Tags"]).Trim(';').Split(';');
                        model.CurrentGallery.Category = Func.ToInt(dt.Rows[0]["Category"]);
                        model.CurrentGallery.GalleryDesc = Func.ToStr(dt.Rows[0]["GalleryDesc"]);
                        model.CurrentGallery.IsFeatured = Func.ToInt(dt.Rows[0]["IsFeatured"]);
                        model.CurrentGallery.TOURID = Func.ToStr(dt.Rows[0]["TOURID"]);
                        model.CurrentGallery.CoverUrl = Func.ToStr(dt.Rows[0]["CoverUrl"]);
                    }
                    else
                    {
                        res.Desc = "Not found Gallery, GalleryID:" + pCategoryID.ToString();
                    }

                    dt = ((DataSet)res.Data).Tables[1];
                    GalleryCategory cat;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            cat = new GalleryCategory();
                            cat.CategoryID = Func.ToInt(dt.Rows[i]["Category"]);
                            cat.CategoryName = App.getBasicDataNameWithCulture("gallerycategory", Func.ToStr(cat.CategoryID));
                            cat.Count = Func.ToInt(dt.Rows[i]["BCount"]);
                            model.GategoryList.Add(cat);
                        }
                    }

                    dt = ((DataSet)res.Data).Tables[2];
                    GalleryTags tag;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            tag = new GalleryTags();
                            tag.TagID = Func.ToInt(dt.Rows[i]["DataID"]);
                            tag.TagName = CultureHelper.GetRes("data_basic", "dataname", "gallerytag" + App.keydelm + tag.TagID, CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["DataName"]));
                            tag.Count = Func.ToInt(dt.Rows[i]["TCount"]);
                            model.TagList.Add(tag);
                        }
                    }
                    dt = ((DataSet)res.Data).Tables[3];
                    model.CurrentGallery.GalleryPhotoViewModel = new GalleryPhotoViewModel();
                    model.CurrentGallery.GalleryPhotoViewModel.List = new List<GalleryPhoto>();
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
                            if (Func.ToStr(dt.Rows[i]["Tags"]).Length > 1)
                            {
                                photo.Tags = Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            }
                            else
                                photo.Tags = new string [0];

                            model.CurrentGallery.GalleryPhotoViewModel.List.Add(photo);
                        }
                    }
                }
                else
                {
                    res.Desc = "Error occured when get Gallery,  GalleryID:" + CategoryID.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Gallery, GalleryID:" + CategoryID.ToString(), ex);
                res.Desc = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }
        public static GalleryViewModel GalleryPhotos(string tags)
        {
            GalleryViewModel model = new GalleryViewModel();
            model.TagList = new List<GalleryTags>();
            model.GategoryList = new List<GalleryCategory>();
            model.CurrentGallery = new Gallery();
            model.CurrentGallery.GalleryPhotoViewModel = new GalleryPhotoViewModel();
            model.CurrentGallery.GalleryPhotoViewModel.List = new List<GalleryPhoto>();
            GalleryPhoto photo;
            Result res;
            try
            {
                string sql = @"begin 
                        select category,COUNT(photoid) BCount
                                from Gallery_photo p
                                left join GALLERY g on p.GalleryID = g.GalleryID
                                group by category;
                         select DataID, DataName, Count(DataID)TCount 
                                        from DATA_BASIC d ,Gallery_photo b 
                                where DataType= 'gallerytag' and b.Tags like '%;' + cast(d.DataID as nvarchar) +';%'  
                                group by DataID, DataName;
                        select * From GALLERY_PHOTO P where Tags like '%;"+tags+";%';end;";
              

                res = Main.DataSetExecute(sql.ToString());
                if (res.Succeed)
                {
                    DataTable dt = ((DataSet)res.Data).Tables[0];
                    model.CurrentGallery = new Gallery();

                    dt = ((DataSet)res.Data).Tables[0];
                    GalleryCategory cat;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            cat = new GalleryCategory();
                            cat.CategoryID = Func.ToInt(dt.Rows[i]["Category"]);
                            cat.CategoryName = App.getBasicDataNameWithCulture("gallerycategory", Func.ToStr(cat.CategoryID));
                            cat.Count = Func.ToInt(dt.Rows[i]["BCount"]);
                            model.GategoryList.Add(cat);
                        }
                    }

                    dt = ((DataSet)res.Data).Tables[1];
                    GalleryTags tag;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            tag = new GalleryTags();
                            tag.TagID = Func.ToInt(dt.Rows[i]["DataID"]);
                            tag.TagName = CultureHelper.GetRes("data_basic", "dataname", "gallerytag" + App.keydelm + tag.TagID, CultureHelper.GetCurrentCulture(), Func.ToStr(dt.Rows[i]["DataName"]));
                            tag.Count = Func.ToInt(dt.Rows[i]["TCount"]);
                            model.TagList.Add(tag);
                        }
                    }
                    dt = ((DataSet)res.Data).Tables[2];
                    model.CurrentGallery.GalleryPhotoViewModel = new GalleryPhotoViewModel();
                    model.CurrentGallery.GalleryPhotoViewModel.List = new List<GalleryPhoto>();
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
                            if (Func.ToStr(dt.Rows[i]["Tags"]).Length > 1)
                            {
                                photo.Tags = Func.ToStr(dt.Rows[i]["Tags"]).Trim(';').Split(';');
                            }
                            else
                                photo.Tags = new string [0];

                            model.CurrentGallery.GalleryPhotoViewModel.List.Add(photo);
                        }
                    }
                }
                else
                {
                    res.Desc = "Error occured when get Gallery Photos,  tag:" + tags.ToString();
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Get Gallery Photos, tag:" + tags.ToString(), ex);
                res.Desc = Helper.UN_EXPECTED_MSG;
            }
            return model;
        }


    }
}