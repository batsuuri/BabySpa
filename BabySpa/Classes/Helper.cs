using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Web;
using static BabySpa.App;
using System.Net.Mail;
using System.Web.UI;
using BabySpa.Core;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BabySpa
{
    public class Helper
    {
        public static string NOT_FOUND_TOUR = "We are sorry, cannot find {0} tour.";
        public static string UN_EXPECTED_MSG = "We are sorry, we check error and fix it asap.";
        public static string MSG_DELETE_AVOID = "Энэ мэдээлэл өөр бүртгэлд ашиглагдаж байгаа тул утсгах боломжгүй.";
        public static string BaseTourPath = "~/Content/tour/";
        public static Dictionary GetDic(string type, int id)
        {
            Dictionary dic;
            string dicKey = type.ToLower() + App.keydelm + id;
            if (App.dicTable.ContainsKey(dicKey))
            {
                dic = (Dictionary)App.dicTable[dicKey];
                dic.name = CultureHelper.GetRes("data_" + type, type + "Name", id.ToString(), CultureHelper.GetCurrentCulture(), dic.name);
                dic.extra = CultureHelper.GetRes("data_" + type, type + "Desc", id.ToString(), CultureHelper.GetCurrentCulture(), dic.extra);
                return dic;
            }
            else
                return null;
        }

        public async Task SendMail(string[] to, string body, string subject, string[] cc = null)
        {

            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(getConfigValue("systemmail"), "BabySpa");
            for (int i = 0; i < to.Length; i++)
            {
                mail.To.Add(new MailAddress(to[i]));
            }
            if (cc != null)
            {
                for (int i = 0; i < cc.Length; i++)
                {
                    mail.CC.Add(new MailAddress(cc[i]));
                }
            }
            mail.Subject = subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(getConfigValue("systemmail"), getConfigValue("systemmailpass"));
            //client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Port = Func.ToInt(getConfigValue("systemmailhostport"));
            client.Host = getConfigValue("systemmailhost");
            //client.EnableSsl = true;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
            try
            {
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }
                Main.ErrorLog("MailSend", errorMessage);
            }
        }

        public static string ArrayToStr(string[] arr,bool trimend=true)
        {
            string ret = "";
            if (arr == null)
            {
                return ret;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                ret = ret + arr[i] + ";";
            }
            if (trimend)
            {
                return ret.TrimEnd(';');
            }
            else
                return ret;

        }
        public static string ArrayToStr(int[] arr)
        {
            string ret = "";
            if (arr == null)
            {
                return ret;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                ret = ret + arr[i].ToString() + ";";
            }
            return ret.TrimEnd(';');
        }
        public static string ArrayToStr(object[] arr)
        {
            string ret = "";
            if (arr == null)
            {
                return ret;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                ret = ret + Func.ToStr(arr[i]) + ";";
            }
            return ret.TrimEnd(';');
        }

        public static string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstSentence(string paragraph)
        {
            int start = 0;
            for (int i = 0; i < paragraph.Length; i++)
            {
                if (paragraph[i] == '>')
                {
                    start = i;
                }
                switch (paragraph[i])
                {
                    case '.':
                        if (i < (paragraph.Length - 1) &&
                            char.IsWhiteSpace(paragraph[i + 1]))
                        {
                            goto case '!';
                        }
                        break;
                    case '?':
                    case '!':
                        return paragraph.Substring(start+1, i- start + 1);
                }
            }
            return paragraph;
        }
        public static Result SaveFile(string path,HttpPostedFileBase file) {
            Result res = new Result(true);
            try
            {
                path = Main.apppath+@"\Content\tour\"+path;
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                file.SaveAs(path);
            }
            catch (Exception ex)
            {
                res.Succeed = false;
                res.Desc = ex.Message;
                Main.ErrorLog("SaveFile, path-" + path, ex);
            }
            return res;
        }
        public static Result DeleteFile(string path,bool deleteSmall = true)
        {
            Result res = new Result(true);
            try
            {
                File.Delete(Main.apppath + @"\Content\tour\" + path);
                if (deleteSmall)
                {
                    string temp = GetPreviewImageUrl(path, false);
                    if (temp != "")
                    {
                        File.Delete(Main.apppath + @"\Content\tour\" + temp);
                    }
                }
            }
            catch (Exception ex)
            {
                res.Succeed = false;
                res.Desc = ex.Message;
                Main.ErrorLog("DeleteFile, path-" + path, ex);
            }
            return res;
        }
        public static string GetPreviewImageUrl(string path,bool getOrigUrl=true) {
            if (path ==null || path=="")
            {
                return "";
            }
            string url = path.Replace("_big", "_small");
            try
            {
                
                if (!File.Exists(Main.apppath + @"\Content\tour\" + url)) {
                    url = getOrigUrl? path:"";
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("GetPreviewImageUrl", ex);
                url = path;
            }

            return url;
        }
    }
    public class PageInfo
    {
        public string PageTitle { get; set; }

        public string PageCaption { get; set; }

        public string PageImageUrl { get; set; }

        private Dictionary<string, string[]> Breadcrumb { get; set; }
        public void AddBreadcrumb(string Title, string[] Url)
        {
            if (Breadcrumb == null)
            {
                Breadcrumb = new Dictionary<string, string[]>();
            }
            if (!Breadcrumb.ContainsKey(Title))
            {
                Breadcrumb.Add(Title, Url);
            }
        }

        public string BreadcrumbText()
        {
            StringBuilder text = new StringBuilder();
            string[] values;
            string temp = "<a class='breadcrumb' href='";
            if (CultureHelper.GetCurrentCulture() != CultureHelper.GetDefaultCulture())
            {
                temp = temp + "/" + CultureHelper.GetCurrentCulture();
            }
            foreach (string key in Breadcrumb.Keys)
            {
                values = Breadcrumb[key];
                switch (values.Length)
                {
                    case 0:
                    case 1:
                        text.Append(temp + "/'>" + key + "</a>");
                        break;
                    case 2:
                        text.Append(temp + "/" + values[0] + "/" + values[1] + "'>" + key + "</a>");
                        break;
                    case 3:
                        text.Append(temp + "/" + values[0] + "/" + values[1] + "/" + values[2] + "'>" + key + "</a>");
                        break;
                    default:
                        text.Append(temp + "'>" + key + "</a>");
                        break;
                }

            }
            return text.ToString();
        }
    }
}