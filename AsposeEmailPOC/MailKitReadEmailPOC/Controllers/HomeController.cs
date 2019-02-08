using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailKitReadEmailPOC.Models;
using System.Text;
using Aspose;
using System.Threading;
using Aspose.Email.Clients.Imap;
using Aspose.Email.Clients;
using Aspose.Email.Tools.Search;
using Aspose.Email;
using Aspose.Email.Mapi;
using System.IO;

namespace MailKitReadEmailPOC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();

            model = ReceiveMails();
            model.data = "";
            return View(model);
        }

        //public ActionResult GetMessegeBody(string id)
        //{
            
        //    CustomerEmailDetails model = new CustomerEmailDetails();

        //    return View("CreateNewCustomer", model);
        //}

        [HttpGet]
        public ActionResult CreateNewCustomer(CustomerEmailDetails model)
        {

            if (ModelState.ContainsKey("{key}"))
                ModelState["{key}"].Errors.Clear();
            return View(model);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public FileResult DownloadAttachment(string id)
        {
            //string AttachmentFilePath = @"C:\AsposeAttachments\";
            string filename = "";
            const string host = "imap.gmail.com";
            const int port = 993;
            const string username = "conduenthcl@gmail.com";
            const string password = "Hclhyd@1234";

            using (var client = new ImapClient(host, port, username, password))
            {
                IAsyncResult result = client.BeginFetchMessage(id);
                MailMessage message = client.EndFetchMessage(result);

                //var ms = new MemoryStream(_data);
                //message.Attachments.Add(new Attachment(ms, attachment.Name));

                foreach (Attachment attachment in message.Attachments)
                {
                    //obj.Attachments.Add(attachment);
                    //attachment.Save(AttachmentFilePath+attachment.Name);
                    filename = attachment.Name;
                    //obj.attachmentName = attachment.Name;

                    //var message1 = MapiMessage.FromFile(attachment.Name);

                    // For nested email attachments - save as msg


                    //if (attachment.ObjectData != null && attachment.ObjectData.IsOutlookMessage)
                    //{
                    //    var embeddedMessage = MapiMessage.FromStream(new System.IO.MemoryStream(attachment.ObjectData.Data));
                    //    embeddedMessage.Save(“out.msg”);
                    //}

                    //MapiMessage msgMessage = MapiMessage.FromStream((new MemoryStream(attachment)));
                    //msgMessage.Save(attachment.LongFileName);

                    var memoryStream = new MemoryStream();

                    attachment.Save(memoryStream);



                    var getData = MapiMessage.FromStream(memoryStream);

                    //attachment.Insert(1, "new 11", getData);
                }
            }



            //    CustomerEmailDetails model = new CustomerEmailDetails();
           // string filename = "";
            //int fid = Convert.ToInt32(id);
            //var files = objData.GetFiles();
            //string filename = (from f in files
            //                   where f.FileId == fid
            //                   select f.FilePath).First();
            string contentType = "application/pdf";
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The connent type MIME type
            //3. The paraneter for the file save asked by the browser
            return File(filename, contentType, "Report.pdf");


        }

        public DashBoardMailBoxJob ReceiveMails()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();
            model.Inbox = new List<MailMessegeInfo>();
            int success = 0;
            int fail = 0;
            int k = 1;

            const string host = "imap.gmail.com";
            const int port = 993;
            const string username = "conduenthcl@gmail.com";
            const string password = "Hclhyd@1234";

            using (var client = new ImapClient(host, port, username, password))
            {
                using (var cancel = new CancellationTokenSource())
                {
                    client.SelectFolder("Inbox");
                    ImapQueryBuilder builder = new ImapQueryBuilder();
                    // Set conditions, Subject contains "Newsletter", Emails that arrived today
                    //builder.Subject.Contains("AttTest");
                    builder.InternalDate.On(DateTime.Now);
                    // Build the query and Get list of messages
                    MailQuery query = builder.GetQuery();
                    ImapMessageInfoCollection messages = client.ListMessages(query);
                    foreach (ImapMessageInfo info in messages)
                    {
                        MailMessegeInfo obj = new MailMessegeInfo();
                        try
                        {
                            IAsyncResult result = client.BeginFetchMessage(info.UniqueId);
                            MailMessage message = client.EndFetchMessage(result);
                            //Attachment attachment = client.FetchAttachment(info.SequenceNumber, "RequiredAttachmentName");
                            obj.UID = info.UniqueId;
                            obj.subject = info.Subject;
                            obj.sender = info.From.ToString();
                            obj.sendDate = info.Date;
                            //obj.attachmentName = attachment.Name;
                            //if (info == null) { }
                            //obj.Attachments = message.Attachments;
                            // Create a loop to display the no. of attachments present in email message
                            foreach (Attachment attachment in message.Attachments)
                            {
                                //obj.Attachments.Add(attachment);
                                //attachment.Save(attachment.Name);
                                obj.attachmentName = attachment.Name;
                            }

                            model.Inbox.Add(obj);
                            success++;
                        }
                        catch (Exception e)
                        {
                            //DefaultLogger.Log.LogError(
                            //    "TestForm: Message fetching failed: " + e.Message + "\r\n" +
                            //    "Stack trace:\r\n" +
                            //    e.StackTrace);
                            fail++;
                        }
                        k++;

                        //Console.WriteLine("Internal Date: " + info.InternalDate);
                    }
                    // Disconnect from IMAP
                    client.Dispose();
                    // ExEnd:InternalDateFilter

                    //client.Connect("imap.gmail.com", 993, true, cancel.Token);

                    // If you want to disable an authentication mechanism,
                    // you can do so by removing the mechanism like this:
                    //client.AuthenticationMechanisms.Remove("XOAUTH");

                    //client.Authenticate("narayana.iosandwin@gmail.com", "password", cancel.Token);

                    // The Inbox folder is always available...
                    //var inbox = client.Inbox;
                    //inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                    //Console.WriteLine("Total messages: {0}", inbox.Count);
                    //Console.WriteLine("Recent messages: {0}", inbox.Recent);

                    // download each message based on the message index
                    //for (int i = 0; i < inbox.Count; i++)
                    //{
                    //    var message = inbox.GetMessage(i, cancel.Token);
                    //    //Console.WriteLine("Subject: {0}", message.Subject);
                    //}

                    // let try searching for some messages...
                    //var query = SearchQuery.DeliveredAfter(DateTime.Parse("2019-02-05"))
                    //.And(SearchQuery.SubjectContains("MailKit"))
                    //.And(SearchQuery.Seen);

                    //foreach (var uid in inbox.Search(query, cancel.Token))
                    //{
                    //    var message = inbox.GetMessage(uid, cancel.Token);
                    //    //Console.WriteLine("[match] {0}: {1}", uid, message.Subject);

                    //    MailMessege obj = new MailMessege();
                    //    try
                    //    {

                    //        //obj.UID = uid;
                    //        obj.subject = message.Subject;
                    //        obj.sender = message.From.ToString();
                    //        obj.sendDate = message.Date.DateTime;
                    //        //if (message.Attachments == null) { }
                    //        //else obj.Attachments = message.Attachments;

                    //        model.Inbox.Add(obj);
                    //        success++;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        //DefaultLogger.Log.LogError(
                    //        //    "TestForm: Message fetching failed: " + e.Message + "\r\n" +
                    //        //    "Stack trace:\r\n" +
                    //        //    e.StackTrace);
                    //        fail++;
                    //    }
                    //    k++;


                    //    //model.Add(message.HtmlBody);
                    //}

                    //client.Disconnect(true, cancel.Token);
                    model.Inbox = model.Inbox.OrderByDescending(m => m.UID).ToList();
                    model.mess = "Mail received!\nSuccesses: " + success + "\nFailed: " + fail + "\nMessage fetching done";

                    if (fail > 0)
                    {
                        model.mess = "Since some of the emails were not parsed correctly (exceptions were thrown)\r\n" +
                                        "please consider sending your log file to the developer for fixing.\r\n" +
                                        "If you are able to include any extra information, please do so.";
                    }
                }
                return model;
            }
        }

        public DashBoardMailBoxJob ReceiveMailsTemp()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();
            model.Inbox = new List<MailMessegeInfo>();
            int success = 0;
            int fail = 0;
            int k = 1;

            const string host = "imap.gmail.com";
            const int port = 993;
            const string username = "narayana.iosandwin@gmail.com";
            const string password = "padma1983";

            // Specify host, username, password, port and SecurityOptions for your client
            //client.Host = "imap.gmail.com";
            //client.Username = "narayana.iosandwin@gmail.com";
            //client.Password = "padma1983";
            //client.Port = 993;
            //client.SecurityOptions = SecurityOptions.Auto;


            using (var client = new ImapClient(host, port, username, password))
            {
                using (var cancel = new CancellationTokenSource())
                {

                    
                    //ImapClient client = new ImapClient(host, port, username, password);
                    client.SelectFolder("Inbox");

                    // Set conditions, Subject contains "Newsletter", Emails that arrived today
                    ImapQueryBuilder builder = new ImapQueryBuilder();
                    //builder.Subject.Contains("Newsletter");
                    builder.InternalDate.On(DateTime.Now);
                    // Build the query and Get list of messages
                    MailQuery query = builder.GetQuery();
                    ImapMessageInfoCollection messages = client.ListMessages(query);
                    foreach (ImapMessageInfo info in messages)
                    {
                        MailMessegeInfo obj = new MailMessegeInfo();
                        try
                        {
                            //Attachment attachment = client.FetchAttachment(info.SequenceNumber, "RequiredAttachmentName");
                            //obj.UID = uid;
                            obj.subject = info.Subject;
                            obj.sender = info.From.ToString();
                            obj.sendDate = info.Date;
                            //obj.attachmentName = attachment.Name;
                            //if (info == null) { }
                            //obj.Attachments = attachment.;

                            model.Inbox.Add(obj);
                            success++;
                        }
                        catch (Exception e)
                        {
                            //DefaultLogger.Log.LogError(
                            //    "TestForm: Message fetching failed: " + e.Message + "\r\n" +
                            //    "Stack trace:\r\n" +
                            //    e.StackTrace);
                            fail++;
                        }
                        k++;

                        //Console.WriteLine("Internal Date: " + info.InternalDate);
                    }
                    // Disconnect from IMAP
                    client.Dispose();
                    // ExEnd:InternalDateFilter

                    //client.Connect("imap.gmail.com", 993, true, cancel.Token);

                    // If you want to disable an authentication mechanism,
                    // you can do so by removing the mechanism like this:
                    //client.AuthenticationMechanisms.Remove("XOAUTH");

                    //client.Authenticate("narayana.iosandwin@gmail.com", "password", cancel.Token);

                    // The Inbox folder is always available...
                    //var inbox = client.Inbox;
                    //inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                    //Console.WriteLine("Total messages: {0}", inbox.Count);
                    //Console.WriteLine("Recent messages: {0}", inbox.Recent);

                    // download each message based on the message index
                    //for (int i = 0; i < inbox.Count; i++)
                    //{
                    //    var message = inbox.GetMessage(i, cancel.Token);
                    //    //Console.WriteLine("Subject: {0}", message.Subject);
                    //}

                    // let try searching for some messages...
                    //var query = SearchQuery.DeliveredAfter(DateTime.Parse("2019-02-05"))
                        //.And(SearchQuery.SubjectContains("MailKit"))
                        //.And(SearchQuery.Seen);

                    //foreach (var uid in inbox.Search(query, cancel.Token))
                    //{
                    //    var message = inbox.GetMessage(uid, cancel.Token);
                    //    //Console.WriteLine("[match] {0}: {1}", uid, message.Subject);

                    //    MailMessege obj = new MailMessege();
                    //    try
                    //    {

                    //        //obj.UID = uid;
                    //        obj.subject = message.Subject;
                    //        obj.sender = message.From.ToString();
                    //        obj.sendDate = message.Date.DateTime;
                    //        //if (message.Attachments == null) { }
                    //        //else obj.Attachments = message.Attachments;

                    //        model.Inbox.Add(obj);
                    //        success++;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        //DefaultLogger.Log.LogError(
                    //        //    "TestForm: Message fetching failed: " + e.Message + "\r\n" +
                    //        //    "Stack trace:\r\n" +
                    //        //    e.StackTrace);
                    //        fail++;
                    //    }
                    //    k++;


                    //    //model.Add(message.HtmlBody);
                    //}

                    //client.Disconnect(true, cancel.Token);
                    model.Inbox = model.Inbox.OrderByDescending(m => m.sendDate).ToList();
                    model.mess = "Mail received!\nSuccesses: " + success + "\nFailed: " + fail + "\nMessage fetching done";

                    if (fail > 0)
                    {
                        model.mess = "Since some of the emails were not parsed correctly (exceptions were thrown)\r\n" +
                                        "please consider sending your log file to the developer for fixing.\r\n" +
                                        "If you are able to include any extra information, please do so.";
                    }
                }
                return model;
            }
        }
    }
}
