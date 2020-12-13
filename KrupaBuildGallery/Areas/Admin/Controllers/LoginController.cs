using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HiQPdf;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Newtonsoft.Json;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {

        private readonly krupagallarydbEntities _db;
        public LoginController()
        {
            _db = new krupagallarydbEntities();
        }

        public ActionResult Index()
        {



            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginVM userLogin)
        {
            try
            {
                string EncyptedPassword = userLogin.Password; // Encrypt(userLogin.Password);

                var data = _db.tbl_AdminUsers.Where(x => x.MobileNo == userLogin.MobileNo && x.Password == EncyptedPassword && !x.IsDeleted).FirstOrDefault();

                if (data != null)
                {
                    if (data.AdminRoleId == (int)AdminRoles.Agent || data.AdminRoleId == (int)AdminRoles.DeliveryUser || data.AdminRoleId == (int)AdminRoles.ChannelPartner)
                    {
                        TempData["LoginError"] = "You are not authorize to access Admin panel";
                        return View();
                    }

                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        return View();
                    }

                    var roleData = _db.tbl_AdminRoles.Where(x => x.AdminRoleId == data.AdminRoleId).FirstOrDefault();

                    if (!roleData.IsActive)
                    {
                        TempData["LoginError"] = "Your Role is not active. Please contact administrator.";
                        return View();
                    }

                    if (roleData.IsDelete)
                    {
                        TempData["LoginError"] = "Your Role is deleted. Please contact administrator.";
                        return View();
                    }

                    clsAdminSession.SessionID = Session.SessionID;
                    clsAdminSession.UserID = data.AdminUserId;
                    clsAdminSession.RoleID = data.AdminRoleId;
                    clsAdminSession.RoleName = roleData.AdminRoleName;
                    clsAdminSession.UserName = data.FirstName + " " + data.LastName;
                    clsAdminSession.ImagePath = data.ProfilePicture;
                    clsAdminSession.MobileNumber = data.MobileNo;
                    // Get Role Permissions
                    if (data.AdminRoleId != 1)
                    {
                        List<RoleModuleVM> lstPermissions = (from m in _db.tbl_AdminRoleModules
                                                             join p in _db.tbl_AdminRolePermissions.Where(x => x.AdminRoleId == data.AdminRoleId) on m.AdminRoleModuleId equals p.AdminRoleModuleId into outerPerm
                                                             from p in outerPerm
                                                             select new RoleModuleVM
                                                             {
                                                                 AdminRoleModuleId = m.AdminRoleModuleId,
                                                                 SelectedValue = (p != null ? p.Permission : 0)
                                                             }).ToList();

                        UserPermissionVM objUserPermission = new UserPermissionVM();
                        objUserPermission.Role = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Role).First().SelectedValue;
                        objUserPermission.Category = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Category).First().SelectedValue;
                        objUserPermission.Product = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Product).First().SelectedValue;
                        objUserPermission.SubProduct = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.SubProduct).First().SelectedValue;
                        objUserPermission.ProductItem = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ProductItem).First().SelectedValue;
                        objUserPermission.Stock = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Stock).First().SelectedValue;
                        objUserPermission.Order = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Order).First().SelectedValue;
                        objUserPermission.Offer = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Role).First().SelectedValue;
                        objUserPermission.Customers = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Customers).First().SelectedValue;
                        objUserPermission.Distibutors = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Distibutors).First().SelectedValue;
                        objUserPermission.DistibutorRequest = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.DistibutorRequest).First().SelectedValue;
                        objUserPermission.ContactRequest = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ContactRequest).First().SelectedValue;
                        objUserPermission.Setting = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.Setting).First().SelectedValue;
                        objUserPermission.ManagePageContent = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ManagePageContent).First().SelectedValue;
                        objUserPermission.ItemText = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.ItemText).First().SelectedValue;
                        objUserPermission.BidItem = lstPermissions.Where(x => x.AdminRoleModuleId == (int)RoleModules.BidItem).First().SelectedValue;
                        string jsonPermissionValues = JsonConvert.SerializeObject(objUserPermission, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        clsAdminSession.UserPermission = jsonPermissionValues;

                    }

                    // Add Login history entry
                    #region LoginHistoryEntry
                    tbl_LoginHistory objLogin = new tbl_LoginHistory();
                    objLogin.UserId = data.AdminUserId;
                    objLogin.Type = "Login";
                    objLogin.DateAction = DateTime.UtcNow;
                    string VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (VisitorsIPAddr != null || VisitorsIPAddr != String.Empty)
                    {
                        VisitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
                    }
                    objLogin.IPAddress = VisitorsIPAddr;
                    _db.tbl_LoginHistory.Add(objLogin);
                    _db.SaveChanges();
                    #endregion LoginHistoryEntry

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["LoginError"] = "Invalid Mobile or Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                tbl_AdminUsers objtbl_AdminUsers = _db.tbl_AdminUsers.Where(o =>  o.MobileNo.ToLower() == MobileNumber.ToLower() && !o.IsDeleted).FirstOrDefault();
                if (objtbl_AdminUsers == null)
                {
                    return "NotExist";
                }
                if (!objtbl_AdminUsers.IsActive)
                {
                    return "InActiveAccount";
                }

                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    //string msg = "Your Otp code for Login is " + num;
                    int SmsId = (int)SMSType.LoginOtpForAdmin;
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
                        string msg1 = "Your Otp code for Login is " + num;                      
                        return num.ToString();

                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        public ActionResult Signout()
        {
            tbl_LoginHistory objLogin = new tbl_LoginHistory();
            objLogin.UserId = clsAdminSession.UserID;
            objLogin.Type = "LogOut";
            objLogin.DateAction = DateTime.Now;
            string VisitorsIPAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (VisitorsIPAddr != null || VisitorsIPAddr != String.Empty)
            {
                VisitorsIPAddr = Request.ServerVariables["REMOTE_ADDR"];
            }
            objLogin.IPAddress = VisitorsIPAddr;
            _db.tbl_LoginHistory.Add(objLogin);

            clsAdminSession.SessionID = "";
            clsAdminSession.UserID = 0;

            return RedirectToAction("Index");
        }

        public void DownloadInvoice(long OrderId)
        {
            StreamReader sr;
            string newhtmldata = "";

            OrderVM objOrder = new OrderVM();
            objOrder = (from p in _db.tbl_Orders
                        join c in _db.tbl_ClientUsers on p.ClientUserId equals c.ClientUserId
                        join u in _db.tbl_ClientOtherDetails on c.ClientUserId equals u.ClientUserId
                        where p.OrderId == OrderId
                        select new OrderVM
                        {
                            OrderId = p.OrderId,
                            ClientUserName = c.FirstName + " " + c.LastName,
                            ClientUserId = p.ClientUserId,
                            ClientAddress = u.Address + ", " + u.City,
                            ClientEmail = c.Email,
                            ClientMobileNo = c.MobileNo,
                            OrderAmount = p.OrderAmount,
                            OrderShipCity = p.OrderShipCity,
                            OrderShipState = p.OrderShipState,
                            OrderShipAddress = p.OrderShipAddress,
                            OrderPincode = p.OrderShipPincode,
                            OrderShipClientName = p.OrderShipClientName,
                            OrderShipClientPhone = p.OrderShipClientPhone,
                            OrderStatusId = p.OrderStatusId,
                            PaymentType = p.PaymentType,
                            OrderDate = p.CreatedDate,
                            GSTNo = p.GSTNo,
                            InvoiceNo = p.InvoiceNo.Value,
                            InvoiceYear = p.InvoiceYear,
                            ShipmentCharge = p.ShippingCharge.HasValue ? p.ShippingCharge.Value : 0,
                            ShippingStatus = p.ShippingStatus.HasValue ? p.ShippingStatus.Value : 2,
                            ExtraAmount = p.ExtraAmount.HasValue ? p.ExtraAmount.Value : 0,
                            IsCashOnDelivery = p.IsCashOnDelivery.HasValue ? p.IsCashOnDelivery.Value : false
                        }).OrderByDescending(x => x.OrderDate).FirstOrDefault();

            if (objOrder != null)
            {
                string PaymentMode = "Cash On Delivery";
                if (objOrder.IsCashOnDelivery == false)
                {
                    PaymentMode = "Online Payment";
                }
                objOrder.OrderStatus = GetOrderStatus(objOrder.OrderStatusId);
                List<OrderItemsVM> lstOrderItms = (from p in _db.tbl_OrderItemDetails
                                                   join c in _db.tbl_ProductItems on p.ProductItemId equals c.ProductItemId
                                                   join u in _db.tbl_ItemVariant on p.VariantItemId equals u.VariantItemId
                                                   where p.OrderId == OrderId
                                                   select new OrderItemsVM
                                                   {
                                                       OrderId = p.OrderId.Value,
                                                       OrderItemId = p.OrderDetailId,
                                                       ProductItemId = p.ProductItemId.Value,
                                                       ItemName = p.ItemName,
                                                       Qty = p.Qty.Value,
                                                       Price = p.Price.Value,
                                                       Sku = p.Sku,
                                                       GSTAmt = p.GSTAmt.Value,
                                                       IGSTAmt = p.IGSTAmt.Value,
                                                       ItemImg = c.MainImage,
                                                       HSNCode = c.HSNCode,
                                                       MRPPrice = p.MRPPrice.HasValue ? p.MRPPrice.Value : p.Price.Value,
                                                       VariantQtytxt = u.UnitQty,
                                                       GST_Per = (p.GSTPer.HasValue ? p.GSTPer.Value : 0),
                                                       Discount = p.Discount.HasValue ? p.Discount.Value : 0,
                                                       FinalAmt = p.FinalItemPrice.Value,
                                                       IsCombo = p.IsCombo.HasValue ? p.IsCombo.Value : false,
                                                       ComboId = p.ComboId.HasValue ? p.ComboId.Value : 0,
                                                       IsMainItem = p.IsMainItem.HasValue ? p.IsMainItem.Value : false,
                                                       ComboName = p.ComboOfferName,
                                                       ComboQty = p.ComboQty.HasValue ? p.ComboQty.Value : p.Qty.Value
                                                   }).OrderBy(x => x.OrderItemId).ToList();
                //}).OrderByDescending(x => x.GST_Per).ToList();

                objOrder.OrderItems = lstOrderItms;
                string file = Server.MapPath("~/templates/Invoice.html");
                string GSTTitle = "GST";
                if (objOrder.OrderShipState != "Gujarat")
                {
                    GSTTitle = "IGST";
                    // file = Server.MapPath("~/templates/InvoiceIGST.html");
                }
                string htmldata = "";

                FileInfo fi = new FileInfo(file);
                sr = System.IO.File.OpenText(file);
                htmldata += sr.ReadToEnd();
                string InvoiceNo = "S&S/" + objOrder.InvoiceYear + "/" + objOrder.InvoiceNo;
                string DateOfInvoice = objOrder.OrderDate.ToString("dd-MM-yyyy");
                string orderNo = objOrder.OrderId.ToString(); ;
                string ClientUserName = objOrder.ClientUserName;
                string ItemHtmls = "";
                decimal TotalFinal = 0;
                decimal SubTotal = 0;
                StringBuilder srBuild = new StringBuilder();
                if (lstOrderItms != null && lstOrderItms.Count() > 0)
                {
                    int cntsrNo = 1;

                    foreach (var objItem in lstOrderItms)
                    {
                        // decimal InclusiveGST = Math.Round(objItem.Price - objItem.Price * (100 / (100 + objItem.GST_Per)), 2);
                        // decimal PreGSTPrice = Math.Round(objItem.Price - InclusiveGST, 2);
                        decimal basicTotalPrice = Math.Round(objItem.Price * objItem.Qty, 2);
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            basicTotalPrice = Math.Round(objItem.Price * objItem.ComboQty, 2);
                        }

                        decimal SGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal CGST = Math.Round(Convert.ToDecimal(objItem.GST_Per / 2), 2);
                        decimal SGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal CGSTAmt = Math.Round(objItem.GSTAmt / 2, 2);
                        decimal IGSTAmt = Math.Round(objItem.GSTAmt);
                        decimal IGST = Math.Round(Convert.ToDecimal(objItem.GST_Per));
                        decimal FinalPrice = Math.Round(objItem.FinalAmt, 2);
                        decimal TaxableAmt = Math.Round(basicTotalPrice - objItem.Discount, 2);
                        TotalFinal = TotalFinal + FinalPrice;
                        if (objItem.IsCombo && objItem.IsMainItem)
                        {
                            srBuild.Append("<tr>");
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                            srBuild.Append("<td colspan='2'>" + objItem.ComboName + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.ComboQty + "</td>");
                            srBuild.Append("<td class=\"text-center\"></td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                            //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                            srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                            //if (objOrder.OrderShipState != "Gujarat")
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                            //}
                            //else
                            //{
                            //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                            //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                            //}

                            srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            srBuild.Append("</tr>");
                            cntsrNo = cntsrNo + 1;
                        }
                        srBuild.Append("<tr>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td>" + cntsrNo + "</td>");
                        }
                        else
                        {
                            srBuild.Append("<td></td>");
                        }
                        srBuild.Append("<td>" + objItem.ItemName + "</td>");
                        srBuild.Append("<td>" + objItem.HSNCode + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Qty + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.VariantQtytxt + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.MRPPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Price + "</td>");
                        //srBuild.Append("<td class=\"text-center\">" + basicTotalPrice + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + objItem.Discount + "</td>");
                        srBuild.Append("<td class=\"text-center\">" + TaxableAmt + "</td>");

                        //if (objOrder.OrderShipState != "Gujarat")
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + IGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + IGSTAmt + "</td>");
                        //}
                        //else
                        //{
                        //    srBuild.Append("<td class=\"text-center\">" + CGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + CGSTAmt + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGST + "</td>");
                        //    srBuild.Append("<td class=\"text-center\">" + SGSTAmt + "</td>");

                        //}

                        srBuild.Append("<td class=\"text-center\">" + Convert.ToDecimal(objItem.GST_Per).ToString("0.##") + "%</td>");
                        if (objItem.IsCombo == false)
                        {
                            srBuild.Append("<td class=\"text-center\">" + Math.Round(FinalPrice, 2) + "</td>");
                            cntsrNo = cntsrNo + 1;
                        }
                        else
                        {
                            srBuild.Append("<td class=\"text-center\"></td>");
                        }
                        srBuild.Append("</tr>");


                    }
                }
                SubTotal = TotalFinal;
                TotalFinal = TotalFinal + objOrder.ShipmentCharge + objOrder.ExtraAmount;
                ItemHtmls = srBuild.ToString();

                string GST_HTML_DATA = getGSTCalculationHtmlDataByOrder(lstOrderItms, objOrder.OrderShipState != "Gujarat");
                string GSTNo = "";
                if (!string.IsNullOrEmpty(objOrder.GSTNo))
                {
                    GSTNo = "GST No." + objOrder.GSTNo;
                }
                double RoundAmt = CommonMethod.GetRoundValue(Convert.ToDouble(TotalFinal));
                double RoundedAmt = CommonMethod.GetRoundedValue(Convert.ToDouble(TotalFinal));
                string address = objOrder.OrderShipAddress + "<br/>" + objOrder.OrderShipCity + "-" + objOrder.OrderPincode + "<br/>" + objOrder.OrderShipState + "<br/> INDIA";
                newhtmldata = htmldata.Replace("--INVOICENO--", InvoiceNo).Replace("--GSTTITLE--", GSTTitle).Replace("--GSTNo--", GSTNo).Replace("--INVOICEDATE--", DateOfInvoice).Replace("--ORDERNO--", orderNo).Replace("--CLIENTUSERNAME--", ClientUserName).Replace("--CLIENTUSERADDRESS--", address).Replace("--CLIENTUSEREMAIL--", objOrder.ClientEmail).Replace("--CLIENTUSERMOBILE--", objOrder.ClientMobileNo).Replace("--ITEMLIST--", ItemHtmls).Replace("--GSTCALCULATIONDATA--", GST_HTML_DATA).Replace("--SHIPPING--", Math.Round(objOrder.ShipmentCharge, 2).ToString()).Replace("--SUBTOTAL--", Math.Round(SubTotal, 2).ToString()).Replace("--TOTAL--", Math.Round(TotalFinal, 2).ToString()).Replace("--EXTRAAMOUNT--", Math.Round(objOrder.ExtraAmount, 2).ToString()).Replace("--ROUNDOFF--", Math.Round(RoundedAmt, 2).ToString()).Replace("--ROUNDTOTAL--", Math.Round(RoundAmt, 2).ToString()).Replace("--PAYMENTMODE--", "Payment: " + PaymentMode); 

            }

            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set browser width
            htmlToPdfConverter.BrowserWidth = 1200;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);

            // convert HTML code to a PDF memory buffer
             htmlToPdfConverter.ConvertHtmlToFile(newhtmldata, "", Server.MapPath("~/Documents/") + "Inv"+objOrder.OrderId+".pdf");
            // convert HTML to PDF
    //        byte[] pdfBuffer = null;
    //        pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(newhtmldata, "");
            // inform the browser about the binary data format
     //       Response.AddHeader("Content-Type", "application/pdf");

            // let the browser know how to open the PDF document, attachment or inline, and the file name
     //       Response.AddHeader("Content-Disposition", String.Format("{0}; filename=HtmlToPdf.pdf; size={1}",
      //         "inline", pdfBuffer.Length.ToString()));

            // write the PDF buffer to HTTP response
      //      Response.BinaryWrite(pdfBuffer);

            // call End() method of HTTP response to stop ASP.NET page processing
         //   Response.End();
        }

        private string getGSTCalculationHtmlDataByOrder(List<OrderItemsVM> lstOrderItms, bool IsIGST)
        {
            string htmlData = string.Empty;

            StringBuilder srBuild = new StringBuilder();


            decimal[] lstGSTPer = new decimal[] { 5.00m, 12.00m, 18.00m, 28.00m };

            decimal Grand_TotaltaxableAmount = 0;
            decimal Grand_IGST_Amt = 0;
            decimal Grand_CGST_Amt = 0;
            decimal Grand_SGST_Amt = 0;
            decimal Grand_FinalAmt = 0;

            lstGSTPer.ToList().ForEach(per =>
            {
                decimal TotaltaxableAmount = lstOrderItms.Where(x => x.GST_Per == per && x.IsCombo == false).Select(x => x.Price * x.Qty - x.Discount).Sum();
                decimal TotaltaxableAmount1 = lstOrderItms.Where(x => x.GST_Per == per && x.IsCombo == true).Select(x => x.Price * x.ComboQty - x.Discount).Sum();
                TotaltaxableAmount = TotaltaxableAmount + TotaltaxableAmount1;
                decimal IGST_Amt = 0;
                decimal CGST_Amt = 0;
                decimal SGST_Amt = 0;

                if (IsIGST)
                {
                    IGST_Amt = (TotaltaxableAmount * per) / 100;
                }
                else
                {
                    decimal half_per = per / 2;
                    CGST_Amt = (TotaltaxableAmount * half_per) / 100;
                    SGST_Amt = (TotaltaxableAmount * half_per) / 100;
                }

                decimal FinalAmt = TotaltaxableAmount + IGST_Amt + CGST_Amt + SGST_Amt;

                srBuild.Append("<tr>");
                srBuild.Append("<td class=\"text-center\"><strong> " + per.ToString("0.##") + "%</strong></td>");
                srBuild.Append("<td class=\"text-center\">" + TotaltaxableAmount.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + IGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + CGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + SGST_Amt.ToString("0.##") + "</td>");
                srBuild.Append("<td class=\"text-center\">" + FinalAmt.ToString("0.##") + "</td>");
                srBuild.Append("</tr>");

                Grand_TotaltaxableAmount += TotaltaxableAmount;
                Grand_IGST_Amt += IGST_Amt;
                Grand_CGST_Amt += CGST_Amt;
                Grand_SGST_Amt += SGST_Amt;
                Grand_FinalAmt += FinalAmt;

            });

            // Taxable Amount
            srBuild.Append("<tr>");
            srBuild.Append("<td class=\"text-center\"><strong>Taxable Amount</strong></td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_TotaltaxableAmount.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_IGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_CGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_SGST_Amt.ToString("0.##") + "</td>");
            srBuild.Append("<td class=\"text-center\">" + Grand_FinalAmt.ToString("0.##") + "</td>");
            srBuild.Append("</tr>");

            htmlData = srBuild.ToString();

            return htmlData;
        }

        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }
    }
}