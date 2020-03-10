using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string username = frm["email"];
            string password = frm["password"];
            string referer = frm["hdnReferer"];
            try
            {
                string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);

                var data = _db.tbl_ClientUsers.Where(x => (x.UserName == username || x.Email == username)
                && x.Password == EncyptedPassword && !x.IsDelete).FirstOrDefault();

                if (data != null)
                {
                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        if (!string.IsNullOrEmpty(referer))
                        {
                            return RedirectToAction("Index", "Login");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login", new {referer = referer });
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
                        return RedirectToAction("Index", "Checkout");
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomePage");
                    }
                   
                }
                else
                {
                    TempData["LoginError"] = "Invalid username or password";
                    if (!string.IsNullOrEmpty(referer))
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login", new { referer = referer });
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
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Login", new {referer = referer });
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

            string GuidNew = Guid.NewGuid().ToString();
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
                    var cartlistsessions = _db.tbl_Cart.Where(o => o.CartSessionId == cookiesessionval).ToList();
                    if (cartlistsessions != null && cartlistsessions.Count() > 0)
                    {
                        foreach (var obj in cartlistsessions)
                        {
                            var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                            if (objcrtsession != null)
                            {
                                objcrtsession.CartItemQty = objcrtsession.CartItemQty + obj.CartItemQty;
                                _db.tbl_Cart.Remove(obj);
                            }
                            else
                            {
                                var crtobj1 = new tbl_Cart();
                                crtobj1.CartItemId = obj.CartItemId;
                                crtobj1.CartItemQty = obj.CartItemQty;
                                crtobj1.CartSessionId = sessioncrtid;
                                crtobj1.ClientUserId = clientusrid;
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
                    Response.Cookies["sessionkeyval"].Value = GuidNew;
                    Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
                    foreach (var obj in cartlistsessions)
                    {
                        var objcrtsession = cartlist.Where(o => o.CartItemId == obj.CartItemId).FirstOrDefault();
                        var crtobj1 = new tbl_Cart();
                        crtobj1.CartItemId = obj.CartItemId;
                        crtobj1.CartItemQty = obj.CartItemQty;
                        crtobj1.CartSessionId = GuidNew;
                        crtobj1.ClientUserId = clientusrid;
                        crtobj1.CreatedDate = DateTime.Now;
                        _db.tbl_Cart.Add(crtobj1);
                        _db.tbl_Cart.Remove(obj);
                        _db.SaveChanges();
                    }
                }

            }
        }
    }
}