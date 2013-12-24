using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Configuration;

//Net.Mail需要设置IIS - Default SMTP Virtual Server - Properties - Access
//                      1.Access control - Authentication - checked "Anonymous access"
//                      2.Relay restrictions - Relay - a.checked "Only the list below"; b.Add - "checked Single computer" and added IP address "127.0.0.1"

namespace KernelClass
{
    public class MailHelper_V2
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

        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, string remoteMailServer, int MailServerPort, string mailServerLoginID, string mailServerPassword)
        {
            SendMailMessage(from, to, bcc, cc, subject, body, replyToEmail, null, true, remoteMailServer, MailServerPort, mailServerLoginID, mailServerPassword);
        }

        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, List<EmailAttachment> emailAttachment)
        {
            SendMailMessage(from, to, bcc, cc, subject, body, replyToEmail, emailAttachment, true);
        }
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, List<EmailAttachment> emailAttachment, bool isBodyHtml)
        {
            SendMailMessage(from, to, bcc, cc, subject, body, replyToEmail, emailAttachment, isBodyHtml, "", 0, "", "");
        }

        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, List<EmailAttachment> emailAttachment, bool isBodyHtml, string remoteMailServer, int MailServerPort, string mailServerLoginID, string mailServerPassword)
        {
            ICollection<EmailAttachment> Attachments = emailAttachment;
            MailMessage aMessage = MailCommon_V2.ConvertToMailMessage(from, to, bcc, cc, subject, body, replyToEmail, isBodyHtml);
            if (Attachments != null && Attachments.Count > 0)
            {
                MailCommon_V2.PopulateAttachments(aMessage.Attachments, Attachments);
            }
            SmtpClient client = MailCommon_V2.ConvertToSmtpClient(remoteMailServer, MailServerPort, mailServerLoginID, mailServerPassword);
            SendMailMessage(aMessage, client);
        }

        public static void SendMailMessage(MailMessage message, SmtpClient client)
        {
            client.Send(message);
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

    }
}
