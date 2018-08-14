using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace BabySpa.Models
{
    [Serializable]
    public class BaseModel
    {
        public BaseModel()
        {
        }
        public static string GetRes(string tablename, string columnname, string keyfield, string culture, string defaultvalue)
        {
            if (CultureHelper.GetDefaultCulture() != culture)
            {
                return CultureHelper.GetRes(tablename, columnname, keyfield, culture, defaultvalue);
            }
            else
                return defaultvalue;
        }

        public static string GetRes(string tablename, string columnname, string keyfield, string defaultvalue)
        {
            return GetRes(tablename, columnname, keyfield, CultureHelper.GetCurrentCulture(), defaultvalue);
        }

        public static string GetRes(string tablename, string columnname, object keyfield, object defaultvalue)
        {
            return GetRes(tablename, columnname, Func.ToStr(keyfield), CultureHelper.GetCurrentCulture(), Func.ToStr(defaultvalue));
        }
        public static string GetRes(string columnname, string keyfield, string defaultvalue)
        {
            return GetRes("TOUR", columnname, keyfield, defaultvalue);
        }

        public PageInfo PageInfo { get; set; }
    }
}