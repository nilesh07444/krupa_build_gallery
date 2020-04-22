using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class DistributorRequestController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public DistributorRequestController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/DistributorRequest
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewDistributorRequest(FormCollection frm,HttpPostedFileBase aadhharphoto, HttpPostedFileBase gstphoto, HttpPostedFileBase pancardphoto, HttpPostedFileBase photofile, HttpPostedFileBase shopphoto)
        {
            try
            {
                string email = frm["email"].ToString();
                string firstnm = frm["fname"].ToString();
                string lastnm = frm["lname"].ToString();
                string mobileno = frm["mobileno"].ToString();
                string businessname = frm["bussinessname"].ToString();
                string addharno = frm["addharno"].ToString();
                string city = frm["city"].ToString();
                string state = frm["state"].ToString();
                string gstno = frm["gstno"].ToString();
                string prefix = frm["prefix"].ToString();
                string dob = frm["dob"].ToString();
                string alternatemobileno = frm["alternatemobileno"].ToString();
                string shopname = frm["shopname"].ToString();
                string pancardno = frm["pancardno"].ToString();
                string photo = string.Empty; 
                string pancardphotoname = string.Empty;
                string gstphotoname = string.Empty;
                string addharphoto = string.Empty;
                string shopphotoname = string.Empty;

                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == email.ToLower() || o.MobileNo.ToLower() == mobileno.ToLower()) && o.IsDelete == false).FirstOrDefault();
                if(objRequest != null)
                {
                    TempData["email"] = frm["email"].ToString();
                    TempData["firstnm"] = frm["fname"].ToString();
                    TempData["lastnm"] = frm["lname"].ToString();
                    TempData["mobileno"] = frm["mobileno"].ToString();
                    TempData["businessname"] = frm["bussinessname"].ToString();
                    TempData["addharno"] = frm["addharno"].ToString();
                    TempData["city"] = frm["city"].ToString();
                    TempData["state"] = frm["state"].ToString();
                    TempData["gstno"] = frm["gstno"].ToString();
                    TempData["pancardno"] = frm["pancardno"].ToString();
                    TempData["alternatemobileno"] = frm["alternatemobileno"].ToString();
                    TempData["dob"] = frm["dob"].ToString();
                    TempData["shopname"] = frm["shopname"].ToString();


                    TempData["RegisterError"] = "You have already sent a request with this email.";
                    return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });
                }

                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == email.ToLower() ||  o.MobileNo.ToLower() == mobileno.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false).FirstOrDefault();
                if (objClientUsr != null)
                {
                    TempData["email"] = frm["email"].ToString();
                    TempData["firstnm"] = frm["fname"].ToString();
                    TempData["lastnm"] = frm["lname"].ToString();
                    TempData["mobileno"] = frm["mobileno"].ToString();
                    TempData["businessname"] = frm["bussinessname"].ToString();
                    TempData["addharno"] = frm["addharno"].ToString();
                    TempData["city"] = frm["city"].ToString();
                    TempData["state"] = frm["state"].ToString();
                    TempData["gstno"] = frm["gstno"].ToString();
                    TempData["pancardno"] = frm["pancardno"].ToString();
                    TempData["alternatemobileno"] = frm["alternatemobileno"].ToString();
                    TempData["dob"] = frm["dob"].ToString();
                    TempData["shopname"] = frm["shopname"].ToString();
                    TempData["RegisterError"] = "Email or Mobile is already exist.Please try with another email or mobile";                    
                    return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });
                }
                else
                {
                    string path = Server.MapPath("~/Images/UsersDocuments/");
                    if (aadhharphoto != null)
                    {
                        addharphoto = Guid.NewGuid() + "-" + Path.GetFileName(aadhharphoto.FileName);
                        aadhharphoto.SaveAs(path + addharphoto);
                    }
                    if (pancardphoto != null)
                    {
                        pancardphotoname = Guid.NewGuid() + "-" + Path.GetFileName(pancardphoto.FileName);
                        pancardphoto.SaveAs(path + pancardphotoname);
                    }
                    if (gstphoto != null)
                    {
                        gstphotoname = Guid.NewGuid() + "-" + Path.GetFileName(gstphoto.FileName);
                        gstphoto.SaveAs(path + gstphotoname);
                    }
                    if (photofile != null)
                    {
                        photo = Guid.NewGuid() + "-" + Path.GetFileName(photofile.FileName);
                        photofile.SaveAs(path + photo);
                    }
                    objRequest = new tbl_DistributorRequestDetails();
                                                     
                    objRequest.CreatedDate = DateTime.Now;
                    objRequest.FirstName = firstnm;
                    objRequest.LastName = lastnm;
                    objRequest.Email = email;
                    objRequest.MobileNo = mobileno;
                    objRequest.CompanyName = businessname;
                    objRequest.City = city;
                    objRequest.State = state;
                    objRequest.AddharcardNo = addharno;
                    objRequest.GSTNo = gstno;
                    objRequest.AlternateMobileNo = alternatemobileno;
                    objRequest.PanCardNo = pancardno;
                    objRequest.PanCardPhoto = pancardphotoname;
                    objRequest.ProfilePhoto = photo;
                    objRequest.Prefix = prefix;
                    objRequest.ShopName = shopname;
                    objRequest.ShopPhoto = shopphotoname;
                    objRequest.AddharPhoto = addharphoto;
                    objRequest.GSTPhoto = gstphotoname;
                    objRequest.Dob = Convert.ToDateTime(dob);
                    objRequest.IsDelete = false;
                    
                    _db.tbl_DistributorRequestDetails.Add(objRequest);
                    _db.SaveChanges();
                    string AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];

                    _db.SaveChanges();
                    string FromEmail = ConfigurationManager.AppSettings["FromEmail"];
                    if (!string.IsNullOrEmpty(objRequest.Email))
                    {
                        FromEmail = objRequest.Email;
                    }
                    string dobstr = objRequest.Dob.Value.ToString("dd-MMM-yyyy");
                    string Subject = "New Distributor Request - Krupa Build Gallery";
                    string bodyhtml = "Following are the details:<br/>";
                    bodyhtml += "FirstName: " + objRequest.FirstName + "<br/>";
                    bodyhtml += "LastName: " + objRequest.LastName + "<br/>";
                    bodyhtml += "Date of Birth: " + dobstr + "<br/>";
                    bodyhtml += "MobileNo: " + objRequest.MobileNo + "<br/>";
                    bodyhtml += "Alternate MobileNo: " + objRequest.AlternateMobileNo + "<br/>";
                    bodyhtml += "Email: " + objRequest.Email + "<br/>";
                    bodyhtml += "CompanyName: " + objRequest.CompanyName + "<br/>";
                    bodyhtml += "City: " + objRequest.City + "<br/>";
                    bodyhtml += "State: " + objRequest.State + "<br/>";
                    bodyhtml += "Addhar Card No: " + objRequest.AddharcardNo+ "<br/>";
                    bodyhtml += "Pan Card No: " + objRequest.PanCardNo + "<br/>";
                    bodyhtml += "GST No: " + objRequest.GSTNo + "<br/>";              

                    clsCommon.SendEmail(AdminEmail, FromEmail, Subject, bodyhtml);
                    TempData["RegisterError"] = "Request receive Successfully.We will contact you asap.";
                    return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["RegisterError"] = ErrorMessage;
            }

            return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });

        }

        public string SendOTP(string MobileNumber,string Email)
        {
            try
            {
                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == Email.ToLower() || o.MobileNo.ToLower() == MobileNumber.ToLower()) && o.IsDelete == false).FirstOrDefault();
                if(objRequest != null )
                {
                    return "AlreadySent";
                }
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == Email.ToLower() || o.MobileNo.ToLower() == MobileNumber.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false).FirstOrDefault();
                if(objClientUsr != null)
                {
                    return "AlreadyExist";
                }
                using (WebClient webClient = new WebClient())
                {                  
                    Random random = new Random();
                    int num = random.Next(555555,999999);
                    string msg = "Your distributor request OTP code is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);                   
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        string FromEmail = ConfigurationManager.AppSettings["FromEmail"];
                        Random random1 = new Random();
                        int num1 = random1.Next(111566,499999);
                        string msg1 = "Your distributor request OTP code is " + num1;
                        clsCommon.SendEmail(Email, FromEmail, "OTP Code for Distributor Request - Krupa Build Gallery", msg1);
                        return num.ToString()+"^"+ num1.ToString();
                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
      
    }
}