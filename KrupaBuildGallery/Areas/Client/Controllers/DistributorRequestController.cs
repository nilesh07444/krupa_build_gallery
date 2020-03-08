﻿using KrupaBuildGallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult NewDistributorRequest(FormCollection frm)
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
                 
                tbl_DistributorRequestDetails objRequest = _db.tbl_DistributorRequestDetails.Where(o => o.Email.ToLower() == email.ToLower() && o.IsDelete == false).FirstOrDefault();
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
                    TempData["RegisterError"] = "You have already sent a request with this email.";
                    return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });
                }

                tbl_ClientUsers objClientUsr = _db.tbl_ClientUsers.Where(o => o.Email.ToLower() == email.ToLower() && o.IsDelete == false).FirstOrDefault();
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
                    TempData["RegisterError"] = "Email is already exist.Please try with another email";                    
                    return RedirectToAction("Index", "DistributorRequest", new { area = "Client" });
                }
                else
                {
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
                    objRequest.IsDelete = false;
                  
                    _db.tbl_DistributorRequestDetails.Add(objRequest);
                    _db.SaveChanges();
                    TempData["RegisterError"] = "Request recieve Successfully.We will contact you asap.";
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
    }
}