using MailKit;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace MailKitReadEmailPOC.Models
{
    public class MailMessege
    {
        public string subject { get; set; }
        public string sender { get; set; }
        public dynamic  UID { get; set; }
        public int MessegeNo { get; set; }
        public DateTime sendDate { get; set; }
        public string AttachedFiles { get; set; }

        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}