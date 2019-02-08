using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailKitReadEmailPOC.Models;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Search;
using System.Threading;
using MimeKit;
using System.IO;
using Microsoft.Extensions.Configuration;
using MailKitReadEmailPOC.Helpers;

namespace MailKitReadEmailPOC.Controllers
{
    public class HomeController : Controller
    {


        public HomeController()
        {

        }
        public IActionResult Index()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();

            model = ReceiveMails();
            model.data = "";
            return View(model);
        }


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

        public DashBoardMailBoxJob ReceiveMails()
        {
            DashBoardMailBoxJob model = new DashBoardMailBoxJob();
            model.Inbox = new List<MailMessege>();
            int success = 0;
            int fail = 0;
            int k = 1;

            var configuration = ConfigurationHelper.GetConfiguration(Directory.GetCurrentDirectory());
           // string folder = configuration.GetValue<string>("FolderForDownload");
            using (var client = new ImapClient())
            {
                using (var cancel = new CancellationTokenSource())
                {
                    client.Connect("imap.gmail.com", 993, true, cancel.Token);

                    // If you want to disable an authentication mechanism,
                    // you can do so by removing the mechanism like this:
                    client.AuthenticationMechanisms.Remove("XOAUTH");

                    client.Authenticate("saritha.azure@gmail.com", "azure@1234", cancel.Token);

                    // The Inbox folder is always available...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                    // let try searching for some messages...
                    var query = SearchQuery.DeliveredAfter(DateTime.Parse("2018-02-01"))
                        //.And(SearchQuery.SubjectContains("MailKit"))
                        .And(SearchQuery.All);
                    
                    foreach (var uid in inbox.Search(query, cancel.Token))
                    {
                        var message = inbox.GetMessage(uid, cancel.Token);
                        //Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
                        string attachedFilenames = "";
                        MailMessege obj = new MailMessege();
                        try
                        {
                            obj.UID = uid.ToString();
                            if (message.Attachments.Count()==0) continue;
                            obj.subject = message.Subject;
                            obj.sender = message.From.ToString();
                            obj.sendDate = message.Date.DateTime;
                            if (message.Attachments == null) { }
                            else
                            {
                                IEnumerable<MimeEntity> attachments= message.Attachments;
                                foreach (MimeEntity mime in attachments)
                                {
                                    Type AttachmentType = mime.GetType();
                                    string filename=((MimeKit.MimePart)mime).FileName;
                                    String folder= @"C:\Files\";
                                    
                                    ViewBag.Folder = folder;
                                    String filepath = folder + filename;
                                    
                                    WriteToFile(mime, filepath);
                                    ConvertToStream(filepath);                                        
                                    attachedFilenames = attachedFilenames+filename + "; ";

                                }
                            }
                            obj.AttachedFiles = attachedFilenames;
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
                        //model.Add(message.HtmlBody);
                    }

                    client.Disconnect(true, cancel.Token);
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


        //public void DownLoadAttachment(dynamic UID)
        //{
        //    DashBoardMailBoxJob model = new DashBoardMailBoxJob();
        //    model.Inbox = new List<MailMessege>();
        //    using (var client = new ImapClient())
        //    {
        //        using (var cancel = new CancellationTokenSource())
        //        {
        //            client.Connect("imap.gmail.com", 993, true, cancel.Token);
        //            client.AuthenticationMechanisms.Remove("XOAUTH");
        //            client.Authenticate("saritha.azure@gmail.com", "azure@1234", cancel.Token);

        //            // The Inbox folder is always available...
        //            var inbox = client.Inbox;
        //            inbox.Open(FolderAccess.ReadOnly, cancel.Token);

        //            // let try searching for some messages...
        //            var query = SearchQuery.DeliveredAfter(DateTime.Parse("2018-02-01"))
        //               //.And(SearchQuery.SubjectContains("MailKit"))
        //               .And(SearchQuery.All);

        //            var message = inbox.GetMessage(UID, cancel.Token,null);
                        

        //            //MailMessege obj = new MailMessege();
        //            //try
        //            //{
        //            //    if (message.Attachments == null) { }
        //            //    else
        //            //    {
        //            //        IEnumerable<MimeEntity> attachments = message.Attachments;
        //            //        foreach (MimeEntity mime in attachments)
        //            //        {
        //            //            Type AttachmentType = mime.GetType();
        //            //            string filename = ((MimeKit.MimePart)mime).FileName;
        //            //            String folder = @"C:\Files\";

        //            //            ViewBag.Folder = folder;
        //            //            String filepath = folder + filename;
        //            //            WriteToFile(mime, filepath);
        //            //            ConvertToStream(filepath);
        //            //   }
        //            // }

        //            // client.Disconnect(true, cancel.Token);

        //            //}
        //            //catch (Exception e)
        //            //{
        //            //}
        //        }
        //    }
        //}

        public void WriteToFile(MimeEntity attachment, string fileName)
        {

            FileInfo fileInfo = new FileInfo(fileName);

            if (!fileInfo.Exists)
            {
                using (var stream = new FileStream(fileName, FileMode.CreateNew))
                {
                    if (attachment is MessagePart)
                    {
                        var rfc822 = (MessagePart)attachment;

                        rfc822.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.ContentObject.DecodeTo(stream);
                    }
                }

            }
        }

        public Stream ConvertToStream(string fileName)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            System.IO.File.WriteAllBytes(fileName, bytes);
            Stream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            return ms;
        }

    }

    }
