using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;
using System.Net.Mime;

namespace KernelClass
{
    public class MailCommon_V2
    {
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

        public static MailMessage ConvertToMailMessage(string from, string to, string bcc, string cc, string subject, string body, string replyToEmail, bool isBodyHtml)
        {
            MailMessage aMessage = new MailMessage();
            aMessage.From = new MailAddress(from);
            foreach (string s in to.Split(';'))
            {
                aMessage.To.Add(s);
            }

            aMessage.Subject = subject;
            aMessage.BodyEncoding = GetEmailEncoding();
            aMessage.IsBodyHtml = isBodyHtml;
            aMessage.Body = body;

            if (!string.IsNullOrEmpty(bcc))
            {
                aMessage.Bcc.Add(bcc);
            }

            if (!string.IsNullOrEmpty(cc))
            {
                aMessage.CC.Add(cc);
            }

            if (!string.IsNullOrEmpty(replyToEmail))
            {
                aMessage.Headers.Add("Reply-To", replyToEmail);
            }
                        
            return aMessage;
        }

        public static SmtpClient ConvertToSmtpClient()
        {
            string host = ConfigurationManager.AppSettings["DefaultSmtpHost"] == null ? "127.0.0.1" : ConfigurationManager.AppSettings["DefaultSmtpHost"];
            int port = ConfigurationManager.AppSettings["DefaultSmtpPort"] == null ? 25 : Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSmtpPort"]);
            string username = ConfigurationManager.AppSettings["DefaultSmtpLoginID"] == null ? "" : ConfigurationManager.AppSettings["DefaultSmtpHost"];
            string password = ConfigurationManager.AppSettings["DefaultSmtpPassword"] == null ? "" : ConfigurationManager.AppSettings["DefaultSmtpHost"];

            return ConvertToSmtpClient(host, port, username, password);
        }

        public static SmtpClient ConvertToSmtpClient(string remoteMailServer, int MailServerPort, string mailServerLoginID, string mailServerPassword)
        {
            if (string.IsNullOrEmpty(remoteMailServer))
            {
                return ConvertToSmtpClient();
            }
            else
            {
                SmtpClient client = new SmtpClient();
                client.Host = remoteMailServer;
                client.Port = MailServerPort;

                if (!String.IsNullOrEmpty(mailServerLoginID) && !String.IsNullOrEmpty(mailServerPassword))
                {
                    client.Credentials = new NetworkCredential(mailServerLoginID, mailServerPassword);
                }

                return client;
            }
        }

        /// <summary>
        /// Populate the email attachment to the specific Attachment collection.
        /// </summary>
        /// <param name="attachments">The specific Attachment collection instance</param>
        /// <param name="emailAttachments">The EmailAttachment collection instance where the data are read from</param>
        public static void PopulateAttachments(ICollection<Attachment> attachments, ICollection<EmailAttachment> emailAttachments)
        {
            foreach (var emailAttachment in emailAttachments)
            {
                attachments.Add(ConvertToAttachment(emailAttachment));
            }
        }

        /// <summary>
        /// Convert an EmailAttachment instance to an Attachment instance.
        /// </summary>
        /// <param name="emailAttachment">The EmailAttachment instance</param>
        /// <returns>The Attachment instance</returns>
        public static Attachment ConvertToAttachment(EmailAttachment emailAttachment)
        {
            if (emailAttachment.Data != null && emailAttachment.Data.Length > 0)
            {
                return new Attachment(new MemoryStream(emailAttachment.Data), emailAttachment.FilePath);
            }

            ContentType contentType = new ContentType(emailAttachment.ContentType);
            return new Attachment(emailAttachment.FilePath, contentType);
        }
    }
}
