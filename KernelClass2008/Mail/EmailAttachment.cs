using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace KernelClass
{
    /// <summary>
    /// The EmailAttachment represents an email attachment.
    /// </summary>
    [DataContract]
    public class EmailAttachment
    {
        [DataMember(IsRequired = false)]
        private byte[] data;

        [DataMember(IsRequired = false)]
        private string filePath;

        [DataMember(IsRequired = true)]
        private string contentType;

        /// <summary>
        /// The default content type of an email attachment.
        /// </summary>
        private static readonly string DefaultContentType = "application/octet-stream";

        /// <summary>
        /// Gets the data of the email attachment.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Gets the file path of the email attachment.
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
        }

        /// <summary>
        /// Gets the content type of the email attachment.
        /// </summary>
        public string ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        /// <summary>
        /// Initialize a new instance of EmailAttachment.
        /// </summary>
        /// <param name="filePath">The file path of the email attachment</param>
        public EmailAttachment(string filePath)
            : this(filePath, DefaultContentType)
        {
        }

        /// <summary>
        /// Initialize a new instance of EmailAttachment.
        /// </summary>
        /// <param name="filePath">The file path of the email attachment</param>
        /// <param name="contentType">The content type of the email attachment</param>
        public EmailAttachment(string filePath, string contentType)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            this.filePath = filePath;
            this.data = new byte[0];
            this.contentType = contentType;
        }

        /// <summary>
        /// Initialize a new instance of EmailAttachment.
        /// </summary>
        /// <param name="data">The data of the email attachment</param>
        /// <param name="contentType">The content type of the email attachment</param>
        public EmailAttachment(byte[] data, string fileName, string contentType)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            this.filePath = fileName;
            if (data == null)
            {
                this.data = new byte[0];
            }
            else
            {
                this.data = data;
            }

            this.contentType = contentType;
        }

        /// <summary>
        /// Initialize a new instance of EmailAttachment.
        /// </summary>
        /// <param name="data">The data of the email attachment</param>
        public EmailAttachment(byte[] data, string fileName)
            : this(data, fileName, DefaultContentType)
        {
        }
    }
}
