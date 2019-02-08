using Aspose.Email;
using System;
using System.Collections.Generic;

namespace MailKitReadEmailPOC.Models
{
    public class MailMessegeInfo
    {
        public string subject { get; set; }
        public string sender { get; set; }
        public string UID { get; set; }
        public int MessegeNo { get; set; }
        public DateTime sendDate { get; set; }

        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; set; }

        public string attachmentName { get; set; }
    }
}