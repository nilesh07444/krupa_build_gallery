using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class LoginController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Login
        public ActionResult Index(string referer = "")
        {
            ViewBag.Referer = referer;
            return View();
        }

        public ActionResult CheckLogin(FormCollection frm)
        {
            string mobilenumber = frm["mobilenumber"];
            string password = frm["password"];
            string referer = frm["hdnReferer"];
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);

                var data = _db.tbl_ClientUsers.Where(x => (x.MobileNo == mobilenumber && x.ClientRoleId == 1)
                && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        if (!string.IsNullOrEmpty(referer))
                        {
                            return RedirectToAction("Index", "Login", new { referer = referer });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }
                    }

                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = data.ClientUserId;
                    clsClientSession.RoleID = Convert.ToInt32(data.ClientRoleId);
                    clsClientSession.FirstName = data.FirstName;
                    clsClientSession.LastName = data.LastName;
                    clsClientSession.ImagePath = data.ProfilePicture;
                    clsClientSession.Email = data.Email;
                    clsClientSession.MobileNumber = data.MobileNo;
                    UpdatCarts();
                    if(!string.IsNullOrEmpty(referer))
                    {
                        if(referer == "checkout")
                        {
                            return RedirectToAction("Index", "Checkout");
                        }
                        else
                        {
                            return RedirectToAction("secondcartcheckout", "Checkout");
                        }
                        
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomePage");
                    }
                   
                }
                else
                {
                    TempData["LoginError"] = "Invalid mobilenumber or password";
                    if (!string.IsNullOrEmpty(referer))
                    {
                        return RedirectToAction("Index", "Login", new { referer = referer });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["LoginError"] = ErrorMessage;
            }
            if (!string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Login", new { referer = referer });
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult Signout()
        {
            clsClientSession.SessionID = "";
            clsClientSession.UserID = 0;
            clsClientSession.FirstName = "";
            clsClientSession.LastName = "";
            clsClientSession.Email = "";
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            string GuidNew = Guid.NewGuid().ToString();
            Response.Cookies["sessionkeyval"].Value = GuidNew;
            Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);           
            return RedirectToAction("Index", "HomePage");
        }

        public void UpdatCarts()
        {

            string GuidNew = "cust" + clsClientSession.UserID; 
            string cookiesessionval = "";
            if (Request.Cookies["sessionkeyval"] != null)
            {
                cookiesessionval = Request.Cookies["sessionkeyval"].Value;
            }
            else
            {
                Response.Cookies["sessionkeyval"].Value = GuidNew;
                Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
            }
            if (clsClientSession.UserID > 0)
            {
                if (string.IsNullOrEmpty(cookiesessionval))
                {
                    cookiesessionval = GuidNew;
                }
                long clientusrid = Convert.ToInt64(clsClientSession.UserID);
                var cartlist = _db.tbl_Cart.Where(o => o.ClientUserId == clientusrid).ToList();
                if (cartlist != null && cartlist.Count() > 0)
                {
                    string sessioncrtid = cartlist.FirstOrDefault().CartSessionId;
                    Response.Cookies["sessionkeyval"].Value = sessioncrtid;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval && o.ClientUserId == 0).ToList();
                    if (cartlistsessions != null && cartlistsessions.Count() > 0)
                    {
                        foreach (var obj in cartlistsessions)
                        {
                            bool IsCashhOrd = obj.IsCashonDelivery.HasValue ? obj.IsCashonDelivery.Value : false;
                            var lstcrtsessions = cartlist.Where(o => o.CartItemId == obj.CartItemId).ToList();
                            if(lstcrtsessions != null && lstcrtsessions.Count() > 0)
                            {
                              
                                var objcrtsess = lstcrtsessions.Where(o => o.IsCashonDelivery == IsCashhOrd).FirstOrDefault();
                                if(objcrtsess != null)
                                {
                                    objcrtsess.CartItemQty = objcrtsess.CartItemQty + obj.CartItemQty;
                                    _db.tbl_Cart.Remove(obj);
                                }
                                else
                                {
                                    var crtobj1 = new tbl_Cart();
                                    crtobj1.CartItemId = obj.CartItemId;
                                    crtobj1.CartItemQty = obj.CartItemQty;
                                    crtobj1.CartSessionId = sessioncrtid;
                                    crtobj1.ClientUserId = clientusrid;
                                    crtobj1.IsCashonDelivery = IsCashhOrd;
                                    crtobj1.CreatedDate = DateTime.Now;
                                    _db.tbl_Cart.Add(crtobj1);
                                    _db.tbl_Cart.Remove(obj);
                                }
                            }
                            else
                            {
                                var crtobj1 = new tbl_Cart();
                                crtobj1.CartItemId = obj.CartItemId;
                                crtobj1.CartItemQty = obj.CartItemQty;
                                crtobj1.CartSessionId = sessioncrtid;
                                crtobj1.ClientUserId = clientusrid;
                                crtobj1.IsCashonDelivery = IsCashhOrd;
                                crtobj1.CreatedDate = DateTime.Now;
                                _db.tbl_Cart.Add(crtobj1);
                                _db.tbl_Cart.Remove(obj);
                            }                     
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    Response.Cookies["sessionkeyval"].Value = "cust" + clsClientSession.UserID;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    foreach (var obj in cartlistsessions)
                    {
                        var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                        var crtobj1 = new tbl_Cart();
                        crtobj1.CartItemId = obj.CartItemId;
                        crtobj1.CartItemQty = obj.CartItemQty;
                        crtobj1.CartSessionId = "cust"+clsClientSession.UserID;
                        crtobj1.ClientUserId = clientusrid;
                        crtobj1.CreatedDate = DateTime.Now;
                        crtobj1.IsCashonDelivery = obj.IsCashonDelivery;
                        _db.tbl_Cart.Add(crtobj1);
                        _db.tbl_Cart.Remove(obj);
                        _db.SaveChanges();
                    }
                }

                var cartlistsecond = _db.tbl_SecondCart.Where(o => o.ClientUserId == clientusrid).ToList();
                if (cartlistsecond != null && cartlistsecond.Count() > 0)
                {
                    string sessioncrtid = cartlistsecond.FirstOrDefault().CartSessionId;
                    Response.Cookies["sessionkeyval"].Value = sessioncrtid;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    var cartlistsessions = _db.tbl_SecondCart.Where(o => o.CartSessionId == cookiesessionval && o.ClientUserId == 0).ToList();
                    if (cartlistsessions != null && cartlistsessions.Count() > 0)
                    {
                        foreach (var obj in cartlistsessions)
                        {
                            var objcrtsession = cartlistsecond.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                            if (objcrtsession != null)
                            {
                                objcrtsession.CartItemQty = objcrtsession.CartItemQty + obj.CartItemQty;
                                _db.tbl_SecondCart.Remove(obj);
                            }
                            else
                            {
                                var crtobj1 = new tbl_SecondCart();
                                crtobj1.CartItemId = obj.CartItemId;
                                crtobj1.CartItemQty = obj.CartItemQty;
                                crtobj1.CartSessionId = sessioncrtid;
                                crtobj1.ClientUserId = clientusrid;
                                crtobj1.CreatedDate = DateTime.Now;
                                _db.tbl_SecondCart.Add(crtobj1);
                                _db.tbl_SecondCart.Remove(obj);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    var cartlistsessions = _db.tbl_SecondCart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    Response.Cookies["sessionkeyval"].Value = "cust" + clsClientSession.UserID;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    foreach (var obj in cartlistsessions)
                    {
                        var objcrtsession = cartlistsecond.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                        var crtobj1 = new tbl_SecondCart();
                        crtobj1.CartItemId = obj.CartItemId;
                        crtobj1.CartItemQty = obj.CartItemQty;
                        crtobj1.CartSessionId = "cust" + clsClientSession.UserID;
                        crtobj1.ClientUserId = clientusrid;
                        crtobj1.CreatedDate = DateTime.Now;
                        _db.tbl_SecondCart.Add(crtobj1);
                        _db.tbl_SecondCart.Remove(obj);
                        _db.SaveChanges();
                    }
                }


            }


        }

        public ActionResult Distributor(string referer = "")
        {
            ViewBag.Referer = referer;
            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {                   
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => (o.Email.ToLower() == MobileNumber || o.MobileNo.ToLower() == MobileNumber.ToLower()) && o.ClientRoleId == 2 && o.IsDelete == false && o.IsActive == true).FirstOrDefault();
                if (objClientUsr == null)
                {
                    return "NotExist";
                }
                using (WebClient webClient = new WebClient())
                {                   
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    string msg = "Your Otp code for Login is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objClientUsr.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        string FromEmail = objGensetting.FromEmail;
                        string msg1 = "Your Otp code for Login is " + num;
                        try
                        {
                            clsCommon.SendEmail(objClientUsr.Email, FromEmail, "OTP Code for Login - Krupa Build Gallery", msg1);
                        }
                        catch(Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }
                        return num.ToString();

                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        public ActionResult CheckLoginDistributor(FormCollection frm)
        {
            string mobilenumber = frm["emailmobile"];
            string password = frm["password"];
            string referer = frm["hdnReferer"];
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);                
                var data = _db.tbl_ClientUsers.Where(x => (x.Email.ToLower() == mobilenumber.ToLower() || x.MobileNo.ToLower() == mobilenumber.ToLower()) && x.ClientRoleId == 2 && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        if (string.IsNullOrEmpty(referer))
                        {
                            return RedirectToAction("Distributor", "Login");
                        }
                        else
                        {
                            return RedirectToAction("Distributor", "Login", new { referer = referer });
                        }
                    }

                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = data.ClientUserId;
                    clsClientSession.RoleID = Convert.ToInt32(data.ClientRoleId);
                    clsClientSession.FirstName = data.FirstName;
                    clsClientSession.LastName = data.LastName;
                    clsClientSession.ImagePath = data.ProfilePicture;
                    clsClientSession.Email = data.Email;
                    clsClientSession.MobileNumber = data.MobileNo;
                    UpdatCarts();
                    if (!string.IsNullOrEmpty(referer))
                    {

                        if (referer == "checkout")
                        {
                            return RedirectToAction("Index", "Checkout");
                        }
                        else
                        {
                            return RedirectToAction("secondcartcheckout", "Checkout");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomePage");
                    }

                }
                else
                {
                    TempData["LoginError"] = "Invalid username or password";
                    if (string.IsNullOrEmpty(referer))
                    {
                        return RedirectToAction("Distributor", "Login");
                    }
                    else
                    {
                        return RedirectToAction("Distributor", "Login", new { referer = referer });
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["LoginError"] = ErrorMessage;
            }
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Distributor", "Login");
            }
            else
            {
                return RedirectToAction("Distributor", "Login", new { referer = referer });
            }

        }

    }
}