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
        public ActionResult NewDistributorRequest(FormCollection frm,HttpPostedFileBase aadhharphoto, HttpPostedFileBase aadhharphoto2, HttpPostedFileBase gstphoto, HttpPostedFileBase pancardnophoto, HttpPostedFileBase photofile, HttpPostedFileBase shopphoto, HttpPostedFileBase shopphoto2, HttpPostedFileBase cancellationchequephoto)
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
                string cancellationchequephotoname = string.Empty;

                string addharphoto2 = string.Empty;
                string shopphotoname2 = string.Empty;

                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == email.ToLower() || o.MobileNo.ToLower() == mobileno.ToLower()) && o.IsDelete == false && o.Status == 0).FirstOrDefault();
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

                    TempData["RegisterError"] = "Email or Mobile is already exist. Please try with another email or mobile";                    
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
                    if (aadhharphoto2 != null)
                    {
                        addharphoto2 = Guid.NewGuid() + "-" + Path.GetFileName(aadhharphoto2.FileName);
                        aadhharphoto2.SaveAs(path + addharphoto2);
                    }
                    if (pancardnophoto != null)
                    {
                        pancardphotoname = Guid.NewGuid() + "-" + Path.GetFileName(pancardnophoto.FileName);
                        pancardnophoto.SaveAs(path + pancardphotoname);
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

                    if (shopphoto != null)
                    {
                        shopphotoname = Guid.NewGuid() + "-" + Path.GetFileName(shopphoto.FileName);
                        shopphoto.SaveAs(path + shopphotoname);
                    }


                    if (shopphoto2 != null)
                    {
                        shopphotoname2 = Guid.NewGuid() + "-" + Path.GetFileName(shopphoto2.FileName);
                        shopphoto2.SaveAs(path + shopphotoname2);
                    }

                    if (cancellationchequephoto != null)
                    {
                        cancellationchequephotoname = Guid.NewGuid() + "-" + Path.GetFileName(cancellationchequephoto.FileName);
                        cancellationchequephoto.SaveAs(path + cancellationchequephotoname);
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
                    objRequest.AddharPhoto2 = addharphoto2;
                    objRequest.ShopPhoto2 = shopphotoname2;
                    objRequest.GSTPhoto = gstphotoname;
                    DateTime dt = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                    objRequest.Dob = dt;
                    objRequest.IsDelete = false;
                    objRequest.Status = 0;
                    objRequest.Reason = "";
                    objRequest.CancellationChequePhoto = cancellationchequephotoname;
                    _db.tbl_DistributorRequestDetails.Add(objRequest);
                    _db.SaveChanges();
                    tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    string AdminEmail = objGensetting.AdminEmail;

                    _db.SaveChanges();
                    string FromEmail = objGensetting.FromEmail;
                    if (!string.IsNullOrEmpty(objRequest.Email))
                    {
                        FromEmail = objRequest.Email;
                    }
                    string dobstr = objRequest.Dob.Value.ToString("dd-MMM-yyyy");
                    string Subject = "New Distributor Request - Shopping & Saving";
                    string bodyhtml = "Following Are The Details:<br/>";
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
                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => (o.Email.ToLower() == Email.ToLower() || o.MobileNo.ToLower() == MobileNumber.ToLower()) && o.IsDelete == false && o.Status == 0).FirstOrDefault();
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
                    //string msg = "Your distributor request OTP code is " + num;
                    int SmsId = (int)SMSType.DistributorReqOtp;
                    clsCommon objcm = new clsCommon();
                    string msg = objcm.GetSmsContent(SmsId);
                    msg = msg.Replace("{{OTP}}", num + "");
                    msg = HttpUtility.UrlEncode(msg);
                    //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", MobileNumber).Replace("--MSG--", msg);
                    var json = webClient.DownloadString(url);                   
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        string FromEmail = objGensetting.FromEmail;
                        Random random1 = new Random();
                        int num1 = random1.Next(111566,499999);
                        string msg1 = "Your Distributor Request OTP Code Is " + num1;
                        clsCommon.SendEmail(Email, FromEmail, "OTP Code For Distributor Request - Shopping & Saving", msg1);
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