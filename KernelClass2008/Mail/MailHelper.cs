using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mail;
using System.Configuration;

namespace KernelClass
{
    public class MailHelper
    {
        /// <summary>
        /// Sends an email message
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail)
        {
            SendMailMessage(from, to, bcc, cc, subject, body, replyToEmail, null);
        }

        /// <summary>
        /// Sends an email message
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        /// <param name="replyToEmail">the reply email address</param>
        /// <param name="remoteMailServer">remote smtp server</param>
        /// <param name="mailServerLoginID">loginId of smtp server</param>
        /// <param name="mailServerPassword">password of smtp server</param>
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, string remoteMailServer, string mailServerLoginID, string mailServerPassword)
        {
            SendMailMessage(from, to, bcc, cc, subject, body, replyToEmail, null, remoteMailServer, mailServerLoginID, mailServerPassword);
        }

        /// <summary>
        /// Sends an email message with attachments
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        /// <param name="lstAttachment">Attachment filename list</param>
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, string[] lstAttachment)
        {
            MailMessage aMessage = new MailMessage();
            aMessage.From = from;
            aMessage.To = to;
            aMessage.Subject = subject;
            aMessage.BodyEncoding = GetEmailEncoding();
            aMessage.BodyFormat = MailFormat.Html;
            aMessage.Body = body;

            if (!string.IsNullOrEmpty(bcc))
            {
                aMessage.Bcc = bcc;
            }

            if (!string.IsNullOrEmpty(cc))
            {
                aMessage.Cc = cc;
            }

            if (lstAttachment != null)
            {
                MailAttachment attach;
                // limited to 3 attachments.
                for (int i = 0; i < lstAttachment.Length && i < 3; i++)
                {
                    try
                    {
                        attach = new MailAttachment(lstAttachment[i]);
                        aMessage.Attachments.Add(attach);
                    }
                    catch
                    {
                        // continue and ignore if attach exception
                    }
                }
            }

            if (!string.IsNullOrEmpty(replyToEmail))
            {
                aMessage.Headers.Add("Reply-To", replyToEmail);
            }
            SmtpMail.Send(aMessage);
        }

        /// <summary>
        /// Sends an email message with attachments, remote smtp
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        /// <param name="replyToEmail">the reply email address</param>
        /// <param name="lstAttachment">Attachment filename list</param>
        /// <param name="remoteMailServer">remote smtp server</param>
        /// <param name="mailServerLoginID">loginId of smtp server</param>
        /// <param name="mailServerPassword">password of smtp server</param>
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, string[] lstAttachment, string remoteMailServer, string mailServerLoginID, string mailServerPassword)
        {
            MailMessage aMessage = new MailMessage();
            aMessage.From = from;
            aMessage.To = to;
            aMessage.Subject = subject;
            aMessage.BodyEncoding = GetEmailEncoding();
            aMessage.BodyFormat = MailFormat.Html;
            aMessage.Body = body;

            if (!string.IsNullOrEmpty(bcc))
            {
                aMessage.Bcc = bcc;
            }

            if (!string.IsNullOrEmpty(cc))
            {
                aMessage.Cc = cc;
            }

            if (!string.IsNullOrEmpty(replyToEmail))
            {
                aMessage.Headers.Add("Reply-To", replyToEmail);
            }

            if (lstAttachment != null)
            {
                MailAttachment attach;
                // limited to 3 attachments.
                for (int i = 0; i < lstAttachment.Length && i < 3; i++)
                {
                    try
                    {
                        attach = new MailAttachment(lstAttachment[i]);
                        aMessage.Attachments.Add(attach);
                    }
                    catch
                    {
                        // continue and ignore if attach exception
                    }
                }
            }

            //aMessage.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
            //aMessage.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"] = remoteMailServer;

            if (!string.IsNullOrEmpty(mailServerLoginID) && !string.IsNullOrEmpty(mailServerPassword))
            {
                aMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                aMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mailServerLoginID);
                aMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mailServerPassword);
            }

            aMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");

            SmtpMail.SmtpServer = remoteMailServer;
            SmtpMail.Send(aMessage);
        }

        /// <summary>
        /// Send email to administrator while run into exception.
        /// </summary>
        /// <param name="agentName">Agent name, eg, Agent Name: Dimension Forward External Email Agent;</param>
        /// <param name="e">object of catch-exception</param>
        /// <param name="orignalWebSite"></param>
        /// <param name="returnWebSiteCode"></param>
        /// <param name="databaseID">databaseId, eg, DM001</param>
        public static void SendErrorEmail(string agentName, Exception ex, string orignalWebSite, string returnWebSiteCode, string databaseID)
        {
            string content;
            string errorEmailFrom;
            string errorEmailTo;

            errorEmailFrom = ConfigurationManager.AppSettings["ErrorEmailFrom"];
            errorEmailTo = ConfigurationManager.AppSettings["ErrorEmailTo"];

            content = "Error DateTime (GMT): " + DateTime.Now.ToUniversalTime() + "<br />";
            content = content + "Server Name: " + System.Environment.MachineName + "<br/ >";
            content = content + "Web Site: " + orignalWebSite + "<br/ >";
            content = content + "Return Web Site: " + returnWebSiteCode + "<br/ >";
            content = content + "Database: " + databaseID + "<br/ >";
            content = content + "Error Class: " + ex.Source.ToString() + "<br/ >";
            content = content + "Error Function: " + ex.TargetSite.Name + "<br/ >";
            content = content + "Error Description: " + ex.Message.ToString() + "<br/ >";
            content = content + "Error StackTrace: " + ex.StackTrace.ToString() + "<br/ >";

            SendMailMessage(errorEmailFrom, errorEmailTo, "", "", agentName, content, "");
        }

        /// <summary>
        /// Get Email Encoding
        /// </summary>
        /// <returns></returns>
        public static System.Text.Encoding GetEmailEncoding()
        {
            string msgEncoding;
            string dataCenter;
            //Determine the email encoding base on DataCenter code 
            dataCenter = System.Configuration.ConfigurationManager.AppSettings["DataCenter"];
            if (dataCenter == null)
                dataCenter = "";
            switch (dataCenter.ToLower())
            {
                case "cn":
                    //use GB 
                    msgEncoding = "gb2312";
                    break;
                default:
                    //defaut to utf-8 
                    msgEncoding = "utf-8";
                    break;
            }

            return System.Text.Encoding.GetEncoding(msgEncoding);
        }
    }
}
