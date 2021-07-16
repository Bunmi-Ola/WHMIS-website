using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHMIS_Project.Models;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Text;
using System.Text.RegularExpressions;

namespace WHMIS_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        public ActionResult ForgotPassword(login user)
        {
          DataSheet_Utility Du = new DataSheet_Utility();

            try
            {
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    if (!Du.CheckUserName(user.UserName, null))
                    {
                        ModelState.Clear();
                        ViewBag.Message = "Email provided is not recognised by the system";
                        return View("login");
                        // return View("LoginError");      
                    }
                }
                
            }
            catch
            {
                return View("Error");
            }

            try
            {
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    if (Du.CheckUserName(user.UserName, null))
                    {

                        Random generator = new Random();
                        string s = generator.Next(0, 1000000).ToString("D6");
                        string newPassword = "SDS" + s;
                       
                        Du.UserPasswordUpdate(user.UserName, newPassword);
                        // HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                        var senderEmail = new MailAddress("comp214projectmgt@gmail.com", "PolyWHMIS");
                        var receiverEmail = new MailAddress(user.UserName, "Receiver");
                        var password = "projectmgt@123";
                        var subject = "Request to reset Password";

                        string htmlString = @"Dear PolyWHMIS user,

You have requested a reset of your password to PolyWHHMIS. PLEASE USE PASSWORD " + newPassword + @" TO LOG IN.

If you did not make this request, Please contact a member of Saskatchewan Polytechnic's Health, Safety and Security Department at 306-536-9288 ASAP.
                        
For your own security, it is recommended that you change this new password to one that is secure and memorable to you as soon as you log in next.
                        

- Saskatchewan Polytechnic Health, Safety and Security Team
                        
THIS IS AN AUTOMATED MESSAGE. PLEASE DO NOT REPLY.

SCENT-SAFE: Saskatchewan Polytechnic promotes a scent-safe working and learning environment. Please do not use scented products. Notice of Confidentiality: This e-mail and any attached files may contain confidential information or privileged material protected by statutes. It is intended for a specific use by the person(s) to whom it is addressed. Review, dissemination, or other use of this material by persons other than the intended recipient(s) is prohibited. If you receive this message in error, please contact the sender and delete this transmission from your computer. Thank you. 

Please consider the environment before printing this email.
";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var message = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = subject,
                            Body = htmlString.ToString()
                    })
                            
                            smtp.Send(message);
                    }                   
                }
                return View("Sent");
            }
            catch
            {
                return View("Error");
            }           
        }

        [HttpPost]
        public ActionResult ResetPassword(login user)
        {
            DataSheet_Utility Du = new DataSheet_Utility();
            try
            {
                if (!string.IsNullOrEmpty(user.UserName) || !string.IsNullOrEmpty(user.Password))
                {
                    if (!Du.CheckUserName(user.UserName, user.Password))
                    {
                        return View("LoginError");
                    }
                }
            }
            catch
            {
                return View("Error");
            }
            try
            {             
                Du.UserPasswordUpdate(user.UserName, user.NewPassword);
                ModelState.Clear();
                ViewBag.Message = "User password reset successfully!";
                return View("login", user);
               
            }
            catch
            {
                return View("Error");
            }
           
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult login(login user)
        {
            DataSheet_Utility Du = new DataSheet_Utility();
            try
            {
                if (!Du.CheckUserName(user.UserName, user.Password))
                {
                    return View("LoginError");
                }
            }
            catch
            {
                return View("Error");
            }
            try
            {
                
                login user1 = Du.logIn(user);
                if (user1.UserName != "null")
                {
                    Session["userName"] = user1.UserName;
                    Session["isMaster"] = user1.IsMaster;

                    if (user.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("Login");
                        cookie.Values.Add("EmailID", user1.UserName);
                        cookie.Values.Add("Password", user1.Password);
                        cookie.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookie);
                    }
                }
                return View("Index");
            }
            catch
            {
                return View("Error");
            }
        
        }

        public ActionResult login()
        {
            login user = new login();
            if (Request.Cookies["Login"] != null)
            {
                user.UserName = Request.Cookies["Login"].Values["EmailID"];
                user.Password = Request.Cookies["Login"].Values["Password"];
            }
            return View(user);
        }

        public ActionResult logOut()
        {
            Session.Clear();
            return View("Index");
        }

        [HttpPost]
        public ActionResult AddUser(login USER)
        {
            if (Session["userName"] == null)
            {
                return View("Denied");
            }
            try
            {
                DataSheet_Utility DU = new DataSheet_Utility();
                if (!DU.CheckUserName(USER.UserName, null))
                {
                    if (USER.NewPassword == USER.ConfirmPassword)
                    {
                        DU.UserInsert(USER);
                        ModelState.Clear();
                        ViewBag.Message = "User added successfully!";
                    }
                    else {
                        ModelState.Clear();
                        ViewBag.Message = "Passwords do not match!";
                    }
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.Message = "User name already exists!";
                }
            }
            catch
            {
                return View("Error");
            }
                 
            return View("AddUser");         
           
        }

        public ActionResult ManageAccounts(login US)
        {
            try
            {
                if (Session["isMaster"] == null || Convert.ToBoolean(Session["isMaster"]) == false)
                {
                    return View("Denied");
                }
            }
            catch
            {
                return View("Error");
            }

            try
            {
                DataSheet_Utility Du = new DataSheet_Utility();
                US.Items = Du.userSelectAll();

                return View(US);
            }
            catch
            {
                return View("Error");
            }
        }


        public ActionResult DeleteUser(int? id, login user)
        {
            int id2 = id ?? default(int);
            try
            {
                DataSheet_Utility DU = new DataSheet_Utility();

                DU.UserDelete(id2);

                DataSheet_Utility Du = new DataSheet_Utility();
                user.Items = Du.userSelectAll();
                ViewBag.Message = "File deleted successfully";
            }
            catch
            {
                return View("Error");
            }
            return View("ManageAccounts", user);
        }

        public ActionResult AddUser()
        {
            if (Session["userName"] == null)
            {
                return View("Denied");
            }
            return View("AddUser");
        }

            public ActionResult LoginError()
        {
            return View();
        }
      
        public ActionResult SafetySheet(Safety_DataSheet SD)
        {
            try
            {
                DataSheet_Utility Du = new DataSheet_Utility();
                SD.Items = Du.docSelectAll();

                return View(SD);
            }
            catch
            {
                return View("Error");
            }

        }

        public ActionResult ViewSDS(int? id, Safety_DataSheet doc)
        {

            int id2 = id ?? default(int);
            try
            {

                MemoryStream workStream = new MemoryStream();
                DataSheet_Utility Du = new DataSheet_Utility();
                byte[] docs = Du.getDocbyId(id2);

                if (docs != null)
                {
                    workStream.Write(docs, 0, docs.Length);
                    workStream.Position = 0;
                    return File(docs, "application/pdf");

                }
            }
            catch
            {
                return View("Error");
            }
            return null;

        }

        
        public ActionResult AddSDS(Safety_DataSheet SD)
        {
            try
            {
                if (Session["userName"] == null )
                {
                    return View("Denied");
                }
            }
            catch
            {

                return View("Error");
            }
            try
                {
                    var supportedTypes = new[] { "pdf" };

                    if (Request.Files.Count > 0)
                    {

                        HttpPostedFileBase file = Request.Files[0];
                        if (file.ContentLength > 0)
                        {
                            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                            if (supportedTypes.Contains(fileExt))
                            {
                                try
                                {

                                    string chemicalname = SD.ChemicalName;
                                    string CAS = SD.CAS;
                                    var fileName = Path.GetFileName(file.FileName);

                                    string filePath = Path.Combine(Server.MapPath("~/doc"), fileName);

                                    using (Stream fs = file.InputStream)
                                    {
                                        using (BinaryReader br = new BinaryReader(fs))
                                        {
                                            byte[] bytes = br.ReadBytes((Int32)fs.Length);


                                            DataSheet_Utility DU = new DataSheet_Utility();

                                      if (DU.CheckChemicalName(chemicalname, 0))
                                        {
                                            ViewBag.Message = "Chemical name already exist";
                                        }
                                        else if (DU.CheckDoc(bytes, 0))
                                            {
                                                ViewBag.Message = "PDF Documents already exist";
                                            }

                                            else
                                            {
                                                DU.docInsert(chemicalname, fileName, bytes, CAS, SD.lastRevisionDate);
                                                ModelState.Clear();
                                                ViewBag.Message = "File uploaded successfully";
                                            }
                                        }
                                    }                                                                       
                                }
                                catch 
                                {

                                return View("Error");
                            }
                            }
                            else
                            {
                                ViewBag.Message = "can only upload pdf files";
                            }
                        }
                        else
                        {
                            ViewBag.Message = "No SDS document selected";
                        }

                    }
                }
                catch
                {
                    return View("Error");
                }
            
           
            return View("AddSDS");
        }

        public ActionResult UpdateSDS(Safety_DataSheet SD, string update, int? id, HttpPostedFileBase file)
        {
            /*try
            {*/
                if (Session["userName"] == null )
                {
                    return View("Denied");
                }/*
            }
            catch
            {

                return View("Error");
            }*/
            try
            {
                int id2 = id ?? default(int);

                var supportedTypes = new[] { "pdf" };

                DataSheet_Utility DU = new DataSheet_Utility();

                if (update == "update")
                {

                    if (file != null)
                    {


                        if (file.ContentLength > 0)
                        {
                            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                            if (supportedTypes.Contains(fileExt))
                            {
                                try
                                {

                                    string chemicalname = SD.ChemicalName;
                                    string CAS = SD.CAS;
                                    var fileName = Path.GetFileName(file.FileName);

                                    string filePath = Path.Combine(Server.MapPath("~/doc"), fileName);

                                    using (Stream fs = file.InputStream)
                                    {
                                        using (BinaryReader br = new BinaryReader(fs))
                                        {
                                            byte[] bytes = br.ReadBytes((Int32)fs.Length);

                                            
                                            if (DU.CheckChemicalName(chemicalname, 0))
                                            {
                                                ViewBag.Message = "Chemical name already exist";
                                            }

                                            else if (DU.CheckDoc(bytes, SD.Id))
                                            {
                                                ViewBag.Message = "PDF Documents already exist";
                                            }

                                            else
                                            {
                                                Safety_DataSheet SD2 = new Safety_DataSheet(SD.Id, chemicalname, fileName, SD.lastRevisionDate, CAS);

                                                DU.InfoUpdatewithDocReplace(SD2, bytes);
                                                ModelState.Clear();
                                                ViewBag.Message = "File updated successfully";
                                            }

                                        }
                                    }


                                }
                                catch 
                                {
                                    return View("Error");
                                }
                            }
                            else
                            {
                                ViewBag.Message = "can only upload pdf files";
                            }
                        }


                    }
                    else
                    {
                        if (DU.CheckChemicalName(SD.ChemicalName, SD.Id))
                        {
                            ViewBag.Message = "Chemical name already exist";
                            SD = DU.getInfoByID(id2);
                            return View(SD);
                        }
                        else
                        {
                            DU.InfoUpdate(SD);
                            SD.IsEditable = false;
                            ViewBag.Message = "File updated successfully";
                        }
                    }
                }
                else
                {
                    if (id != null)
                    {
                        SD = DU.getInfoByID(id2);
                    }
                    else
                    {
                        return View("AddSDS");
                    }
                }
            }
            catch
            {
                return View("Error");
            }
            return View(SD);
        }

        public ActionResult InfoEdit(int? id, Safety_DataSheet SD)
        {

            int id2 = id ?? default(int);
            try
            {
                DataSheet_Utility Du = new DataSheet_Utility();
                SD = Du.getInfoByID(SD.Id);

                SD.IsEditable = true;

                //ViewBag.Message = "Updating Safty Date-Sheet for: " + SD.ChemicalName;
            }
            catch
            {
                return View("Error");
            }

            return View("UpdateSDS", SD);

        }

        public ActionResult AllDocs(Safety_DataSheet doc)
        {
            try
            {
                DataSheet_Utility Du = new DataSheet_Utility();
                doc.Items = Du.docSelectAll();
            }
            catch
            {
                return View("Error");
            }
            return View(doc);
        }

        public ActionResult DeleteAction(int? id, Safety_DataSheet SD)
        {
            int id2 = id ?? default(int);
            try
            {
                DataSheet_Utility DU = new DataSheet_Utility();

                DU.InfoDelete(id2);

                DataSheet_Utility Du = new DataSheet_Utility();
                SD.Items = Du.docSelectAll();
                ViewBag.Message = "File deleted successfully";
            }
            catch
            {
                return View("Error");
            }
            return View("SafetySheet", SD);
        }

       

    }
}