using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class TranslatorValue
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string KeyField { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Language field is required")]
        public string Lang { get; set; }
        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Text field is required")]
        public string TextValue { get; set; }
    }

    public class TranslatorViewModel
    {
        public TranslatorValue CurrentEntry { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string KeyField { get; set; }
        public List<TranslatorValue> List {get;set;}
        public string DisplayMode { get; set; }
        public List<SelectListItem> LangList { get; set; }
    }
}