using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Client.Controllers
{
    public class RegisterController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public RegisterController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Client/Register
        public ActionResult Index(string referer = "")
        {
            ViewBag.Referer = referer;
            return View();
        }
        public ActionResult CreateAccount(FormCollection frm)
        {
            string referer = frm["hdnReferer"];
            try
            {
                string email = frm["email"].ToString();
                string firstnm = frm["fname"].ToString();
                string lastnm = frm["lname"].ToString();
                string mobileno = frm["mobileno"].ToString();
                string password = frm["password"].ToString();
              
                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower()).FirstOrDefault();
                if(objClientUsr != null)
                {
                    TempData["RegisterError"] = "Your Account is already exist.Please go to Login or Contact to support";
                    TempData["email"] = email;
                    TempData["firstnm"] = firstnm;
                    TempData["lastnm"] = lastnm;
                    TempData["mobileno"] = mobileno;
                    if (!string.IsNullOrEmpty(referer))
                    {
                        return RedirectToAction("Index", "Register", new { area = "Client", });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Register", new { area = "Client", referer = referer });
                    }                  
                }
                else
                {
                    string EncyptedPassword = clsCommon.EncryptString(password); // Encrypt(userLogin.Password);
                    objClientUsr = new tbl_ClientUsers();
                    objClientUsr.Email = email;
                    objClientUsr.ClientRoleId = 1;
                    objClientUsr.CreatedBy = 0;
                    objClientUsr.CreatedDate = DateTime.Now;
                    objClientUsr.FirstName = firstnm;
                    objClientUsr.LastName = lastnm;
                    objClientUsr.MobileNo = mobileno;
                    objClientUsr.IsActive = true;
                    objClientUsr.IsDelete = false;
                    objClientUsr.ProfilePicture = "";
                    objClientUsr.UserName = firstnm + lastnm;
                    objClientUsr.Password = EncyptedPassword;
                    _db.tbl_ClientUsers.Add(objClientUsr);
                    _db.SaveChanges();

                    tbl_ClientOtherDetails objtbl_ClientOtherDetails = new tbl_ClientOtherDetails();
                    objtbl_ClientOtherDetails.ClientUserId = objClientUsr.ClientUserId;
                    objtbl_ClientOtherDetails.IsActive = true;
                    objtbl_ClientOtherDetails.IsDelete = false;
                    objtbl_ClientOtherDetails.CreatedDate = DateTime.Now;
                    objtbl_ClientOtherDetails.CreatedBy = objClientUsr.ClientUserId;
                    _db.tbl_ClientOtherDetails.Add(objtbl_ClientOtherDetails);
                    _db.SaveChanges();
                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = objClientUsr.ClientUserId;
                    clsClientSession.RoleID = Convert.ToInt32(objClientUsr.ClientRoleId);
                    clsClientSession.FirstName = objClientUsr.FirstName;
                    clsClientSession.LastName = objClientUsr.LastName;
                    clsClientSession.ImagePath = objClientUsr.ProfilePicture;
                    clsClientSession.Email = objClientUsr.Email;
                    UpdatCarts();
                    if (!string.IsNullOrEmpty(referer))
                    {
                        return RedirectToAction("Index", "Checkout", new { area = "Client" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomePage", new { area = "Client" });
                    }                  
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["RegisterError"] = ErrorMessage;
            }

            if (!string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Register", new { area = "Client", });
            }
            else
            {
                return RedirectToAction("Index", "Register", new { area = "Client", referer = referer });
            }

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