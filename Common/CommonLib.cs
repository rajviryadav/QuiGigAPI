using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Xml;

namespace QuiGigAPI.Common
{
    public class CommonLib
    {
        public static string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
        public static string EmailSMTP = ConfigurationManager.AppSettings["EmailSMTP"].ToString();
        public static int EmailPort = 25;
        public static string GetEmailTemplateValue(string xpath)
        {
            string strValue = "";
            string strPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["TemplatePath"]);
            XmlDocument doc = new XmlDocument();
            doc.Load(strPath);
            XmlElement root = doc.DocumentElement;
            XmlNode node = doc.DocumentElement.SelectSingleNode(xpath);
            XmlNode childNode = node.ChildNodes[0];
            if (childNode is XmlCDataSection)
            {
                XmlCDataSection cdataSection = childNode as XmlCDataSection;
                strValue = cdataSection.Value.ToString();
            }
            else
                strValue = childNode.Value.ToString();
            return strValue;
        }

        public static void SendMail(string strFrom, string strTo, string strSubject, string strBody)
        {
            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = strFrom;
                WebMail.Password = EmailPassword;

                //Sender email address.  
                WebMail.From = strFrom;

                //Send email  
                WebMail.Send(to: strTo, subject: strSubject, body: strBody, isBodyHtml: true);
            }
            catch (Exception ex)
            {
                //LogError(ex);
                throw (ex);
            }

        }


        public static List<int> Year()
        {
            List<int> year = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= 1968; i--)
            {
                year.Add(i);
            }
            return year;
        }

        public static List<string> Month()
        {
            List<string> month = new List<string>();
            string[] localizedMonths = Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames;
            string[] invariantMonths = DateTimeFormatInfo.InvariantInfo.MonthNames;

            for (int i = 0; i < 12; i++)
            {
                month.Add(localizedMonths[i]);
            }
            return month;
        }
    }
}