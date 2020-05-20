using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly krupagallarydbEntities _db;

        public MyProfileController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/MyProfile
        public ActionResult Index()
        {
            ViewData["clientdetails"] = new tbl_ClientUsers();
            ViewData["clientotherdetails"] = new tbl_ClientOtherDetails();
            ViewBag.OrderCount = 0;
            ViewBag.WalletAmt = 0;
            ViewBag.CreditBls = 0;
            ViewBag.PointsRemaining = 0;
            if (clsClientSession.UserID > 0)
            {
                long userid = clsClientSession.UserID;
                ViewBag.OrderCount = _db.tbl_Orders.Where(o => o.ClientUserId == userid).ToList().Count;
                tbl_ClientUsers objClientUser = _db.tbl_ClientUsers.Where(o => o.ClientUserId == userid).FirstOrDefault();
                tbl_ClientOtherDetails objClientOtherdetails =_db.tbl_ClientOtherDetails.Where(o => o.ClientUserId == userid).FirstOrDefault();
                decimal waltamt = objClientUser.WalletAmt.HasValue ? objClientUser.WalletAmt.Value : 0;
                decimal credit = objClientOtherdetails.CreditLimitAmt.HasValue ? objClientOtherdetails.CreditLimitAmt.Value : 0;
                if(credit > 0)
                {
                    decimal amtdue = objClientOtherdetails.AmountDue.HasValue ? objClientOtherdetails.AmountDue.Value : 0;
                    ViewBag.CreditBls = credit - amtdue;
                }
                DateTime dtNow = DateTime.UtcNow;
              
                List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == userid && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
                decimal pointreamining = 0;
                if (lstpoints != null && lstpoints.Count() > 0)
                {
                    pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
                }
                ViewBag.PointsRemaining = pointreamining;
                ViewBag.WalletAmt = waltamt;               
                ViewData["clientdetails"] = objClientUser;
                ViewData["clientotherdetails"] = objClientOtherdetails;
            }
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();
                    Random random = new Random();
                    int num = random.Next(310450,789899);
                    string msg = "Your change password OTP code is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + MobileNumber + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return num.ToString();
                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string ChangePasswordSubmit(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string CurrentPassword = frm["currentpwd"];
                string NewPassword = frm["newpwd"];

                long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    string EncryptedCurrentPassword = clsCommon.EncryptString(CurrentPassword); 
                    if (objUser.Password == EncryptedCurrentPassword)
                    {
                        objUser.Password = clsCommon.EncryptString(NewPassword);
                        _db.SaveChanges();

                        ReturnMessage = "SUCCESS";
                        Session.Clear();
                    }
                    else
                    {
                        ReturnMessage = "CP_NOT_MATCHED";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public ActionResult SaveProfile(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string fname = Convert.ToString(frm["firstname"]);
                string lname = Convert.ToString(frm["lastname"]);
                string mobile = Convert.ToString(frm["mobilenumber"]);
                string altmobile = Convert.ToString(frm["alternatemobile"]);
                string email = Convert.ToString(frm["email"]);
                string prefix = Convert.ToString(frm["prefix"]);
                string shipfnam = Convert.ToString(frm["shipfirstname"]);
                string shiplname = Convert.ToString(frm["shiplastname"]);
                string shipmobile = Convert.ToString(frm["shipmobilenumber"]);
                string shipaddress = Convert.ToString(frm["shipaddress"]);
                string shipcity = Convert.ToString(frm["shipcity"]);
                string shippincode = Convert.ToString(frm["shippincode"]);
                string shipstate = Convert.ToString(frm["shipstate"]);
                
                long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());
                tbl_ClientUsers objUser = _db.tbl_ClientUsers.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();
                tbl_ClientOtherDetails objotherdetails = _db.tbl_ClientOtherDetails.Where(x => x.ClientUserId == LoggedInUserId).FirstOrDefault();
                objUser.FirstName = fname;
                objUser.LastName = lname;
                objUser.AlternateMobileNo = altmobile;
                objUser.Prefix = prefix;
                objotherdetails.ShipFirstName = shipfnam;
                objotherdetails.ShipLastName = shiplname;
                objotherdetails.ShipPhoneNumber = shipmobile;
                objotherdetails.ShipPostalcode = shippincode;
                objotherdetails.ShipState = shipstate;
                objotherdetails.ShipCity = shipcity;
                objotherdetails.ShipAddress = shipaddress;
                if(clsClientSession.RoleID == 2)
                {
                    string addharcard = Convert.ToString(frm["addharcardno"]);
                    string companyname = Convert.ToString(frm["companyname"]);
                    string gstno = Convert.ToString(frm["gstno"]);
                    string panno = Convert.ToString(frm["panno"]);
                    objotherdetails.Addharcardno = addharcard;
                    objUser.CompanyName = companyname;
                    objotherdetails.GSTno = gstno;
                    objotherdetails.Pancardno = panno;
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return RedirectToAction("Index");
        }
    }
}