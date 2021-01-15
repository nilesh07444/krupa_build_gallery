using ConstructionDiary.Models;
using KrupaBuildGallery.Helper;
using KrupaBuildGallery.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class PurchaseDealersController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public string UsersDocumentsDirectoryPath = "";
        public PurchaseDealersController()
        {
            _db = new krupagallarydbEntities();
            UsersDocumentsDirectoryPath = ErrorMessage.UsersDocumentsDirectoryPath;
        }
        // GET: Admin/PurchaseDealers
        public ActionResult Index(string StartDate = "", string EndDate = "")
        {
            List<PurchaseDealerVM> lstpurchasedelr = new List<PurchaseDealerVM>();
            try
            {
                DateTime dtStart = DateTime.MinValue;
                if (!string.IsNullOrEmpty(StartDate))
                {
                    dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
                }

                DateTime dtEnd = DateTime.MaxValue;
                if (!string.IsNullOrEmpty(StartDate))
                {
                    dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
                }
                lstpurchasedelr = (from cu in _db.tbl_PurchaseDealers
                                   where !cu.IsDelete.Value && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                                   select new PurchaseDealerVM
                                   {
                                       Pk_Dealer_Id = cu.Pk_Dealer_Id,
                                       FirmName = cu.FirmName,
                                       FirmCity = cu.FirmCity,
                                       State = cu.State,
                                       Email = cu.Email,
                                       FirmContactNo = cu.FirmContactNo,
                                       OwnerName = cu.OwnerName,
                                       IsActive = cu.IsActive.Value,
                                       Status = cu.Status.HasValue ? cu.Status.Value : 0,
                                       CreatedDate = cu.CreatedDate.Value,
                                       OwnerContactNo = cu.OwnerContactNo,
                                       BussinessCode = cu.BussinessCode
                                   }).OrderByDescending(x => x.CreatedDate).ToList();


                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstpurchasedelr);
        }

        public ActionResult RequestList(int Status = 0)
        {
            List<PurchaseDealerVM> lstDealerRequest = new List<PurchaseDealerVM>();
            try
            {

                lstDealerRequest = (from cu in _db.tbl_PurchaseDealersRequest
                                    where !cu.IsDelete.Value && (Status == -1 || cu.Status == Status)
                                    select new PurchaseDealerVM
                                    {
                                        Pk_Dealer_Id = cu.Pk_Dealer_Id,
                                        FirmName = cu.FirmName,
                                        FirmCity = cu.FirmCity,
                                        State = cu.State,
                                        Email = cu.Email,
                                        FirmContactNo = cu.FirmContactNo,
                                        OwnerName = cu.OwnerName,
                                        Status = cu.Status.HasValue ? cu.Status.Value : 0
                                    }).OrderBy(x => x.FirmName).ToList();

                ViewBag.Status = Status;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstDealerRequest);
        }

        public ActionResult RequestDetail(int Id)
        {
            PurchaseDealerVM objDealerReq = new PurchaseDealerVM();
            try
            {
                objDealerReq = (from cu in _db.tbl_PurchaseDealersRequest
                                where !cu.IsDelete.Value && cu.Pk_Dealer_Id == Id
                                select new PurchaseDealerVM
                                {
                                    Pk_Dealer_Id = cu.Pk_Dealer_Id,
                                    FirmName = cu.FirmName,
                                    FirmAddress = cu.FirmAddress,
                                    Email = cu.Email,
                                    FirmCity = cu.FirmCity,
                                    FirmGSTNo = cu.FirmGSTNo,
                                    VisitingCardPhoto = cu.VisitingCardPhoto,
                                    FirmContactNo = cu.FirmContactNo,
                                    AlternateNo = cu.AlternateNo,
                                    BusinessDetails = cu.BusinessDetails,
                                    BankAcNumber = cu.BankAcNumber,
                                    IFSCCode = cu.IFSCCode,
                                    BankBranch = cu.BankBranch,
                                    BankAcNumber2 = cu.BankAcNumber2,
                                    BankBranch2 = cu.BankBranch2,
                                    IFSCCode2 = cu.IFSCCode2,
                                    OwnerName = cu.OwnerName,
                                    OwnerContactNo = cu.OwnerContactNo,
                                    Remark = cu.Remark,
                                    VisitingCardPhoto2 = cu.VisitingCardPhoto2,
                                    Pincode = cu.Pincode,
                                    BussinessCode = cu.BussinessCode,
                                    Status = cu.Status.HasValue ? cu.Status.Value : 0,
                                    State = cu.State,
                                    IsActive = cu.IsActive.Value
                                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }
            return View(objDealerReq);
        }

        [HttpPost]
        public string ApproveRejectDealerRequest(long RequestId, string IsApprove, string Password = "", string Reason = "")
        {
            string ReturnMessage = "";

            try
            {
                var objReq = _db.tbl_PurchaseDealersRequest.Where(o => o.Pk_Dealer_Id == RequestId).FirstOrDefault();
                if (objReq != null)
                {
                    if (IsApprove == "false")
                    {
                        objReq.Status = 2; //   0 For Pending  1 For Accept 2 For Reject
                        objReq.RejectReason = Reason;
                        _db.SaveChanges();
                        try
                        {
                            string ToEmail = objReq.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Registration as a Purchase Dealer Rejected - Shoping Saving";
                            string bodyhtml = "Following is the reason<br/>";
                            bodyhtml += "===============================<br/>";
                            bodyhtml += Reason;
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Your Registration As Purchase Dealer Rejected - Shopping & Saving\n";
                            msg += "Following Is The Reason:\n";
                            msg += Reason;
                            //int SmsId = (int)SMSType.DistributorReqRejected;
                            //clsCommon objcm = new clsCommon();
                            //string msg = objcm.GetSmsContent(SmsId);
                            //msg = msg.Replace("{{Reason}}", Reason);
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objReq.FirmContactNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }

                    }
                    else
                    {
                        tbl_PurchaseDealers objRequest = new tbl_PurchaseDealers();
                        objRequest.FirmName = objReq.FirmName;
                        objRequest.FirmAddress = objReq.FirmAddress;
                        objRequest.FirmCity = objReq.FirmCity;
                        objRequest.FirmGSTNo = objReq.FirmGSTNo;
                        objRequest.VisitingCardPhoto = objReq.VisitingCardPhoto;
                        objRequest.FirmContactNo = objReq.FirmContactNo;
                        objRequest.AlternateNo = objReq.AlternateNo;
                        objRequest.Email = objReq.Email;
                        objRequest.BusinessDetails = objReq.BusinessDetails;
                        objRequest.BankAcNumber = objReq.BankAcNumber;
                        objRequest.IFSCCode = objReq.IFSCCode;
                        objRequest.BankBranch = objReq.BankBranch;
                        objRequest.BankAcNumber2 = objReq.BankAcNumber2;
                        objRequest.IFSCCode2 = objReq.IFSCCode2;
                        objRequest.BankBranch2 = objReq.BankBranch2;
                        objRequest.OwnerName = objReq.OwnerName;
                        objRequest.OwnerContactNo = objReq.OwnerContactNo;
                        objRequest.Remark = objReq.Remark;
                        objRequest.BussinessCode = "";
                        objRequest.Password = Password;
                        objRequest.IsDelete = false;
                        objRequest.VisitingCardPhoto2 = objReq.VisitingCardPhoto2;
                        objRequest.Pincode = objReq.Pincode;
                        objRequest.State = objReq.State;
                        objRequest.Status = 1;
                        objRequest.IsActive = true;
                        objRequest.CreatedBy = clsAdminSession.UserID;
                        objRequest.CreatedDate = DateTime.Now;
                        objRequest.ModifiedDate = DateTime.Now;
                        objRequest.ModifiedBy = clsAdminSession.UserID;
                        _db.tbl_PurchaseDealers.Add(objRequest);
                        _db.SaveChanges();
                        objReq.Status = 1;

                        if (objRequest.Pk_Dealer_Id.ToString().Length > 5)
                        {
                            objRequest.BussinessCode = "SSBC" + objRequest.Pk_Dealer_Id;
                        }
                        else
                        {
                            objRequest.BussinessCode = "SSBC" + objRequest.Pk_Dealer_Id.ToString("00000");
                        }
                        _db.SaveChanges();
                        try
                        {
                            string ToEmail = objReq.Email;
                            tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                            string FromEmail = objGensetting.FromEmail;
                            string Subject = "Your Registration As a Purchase Dealer Created - Shopping & Saving";
                            string bodyhtml = "Thank You For Become A Valuable Dealer Of Shopping & Saving<br/>";
                            bodyhtml += "Following Are The Login Details:<br/>";
                            bodyhtml += "===============================<br/>";
                            bodyhtml += "BusinessCode: " + objRequest.BussinessCode + "<br/>";
                            bodyhtml += "Password: " + Password + "<br/>";
                            clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                        }
                        catch (Exception e)
                        {
                            string ErrorMessage = e.Message.ToString();
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            WebClient client = new WebClient();
                            Random random = new Random();
                            int num = random.Next(111566, 999999);
                            string msg = "Thank You For Become A Valuable Purchase Dealer Of Shopping & Saving\n";
                            msg += "Login Details:\n";
                            msg += "BusinessCode:" + objRequest.BussinessCode + "\n";
                            msg += "Password:" + Password + "\n";
                            //int SmsId = (int)SMSType.DistributorReqAccepted;
                            //clsCommon objcm = new clsCommon();
                            //string msg = objcm.GetSmsContent(SmsId);
                            // msg = msg.Replace("{{MobileNo}}", objReq.MobileNo + "").Replace("{{Password}}", Password);
                            msg = HttpUtility.UrlEncode(msg);
                            //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";                            
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objReq.FirmContactNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                /// return "InvalidNumber";
                            }
                            else
                            {
                                //  return num.ToString();
                            }

                        }


                    }
                }

                ReturnMessage = "Success";
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeletePurchaseDealerRequest(long RequestId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PurchaseDealersRequest distributorreq = _db.tbl_PurchaseDealersRequest.Where(x => x.Pk_Dealer_Id == RequestId).FirstOrDefault();

                if (distributorreq == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_PurchaseDealersRequest.Remove(distributorreq);
                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_PurchaseDealers objtbl_PurchaseDealers = _db.tbl_PurchaseDealers.Where(x => x.Pk_Dealer_Id == Id).FirstOrDefault();

                if (objtbl_PurchaseDealers != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_PurchaseDealers.IsActive = true;
                    }
                    else
                    {
                        objtbl_PurchaseDealers.IsActive = false;
                    }

                    objtbl_PurchaseDealers.ModifiedBy = LoggedInUserId;
                    objtbl_PurchaseDealers.ModifiedDate = DateTime.Now;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public void Export(string StartDate = "", string EndDate = "")
        {
            ExcelPackage excel = new ExcelPackage();
            List<PurchaseDealerVM> lstDelers = new List<PurchaseDealerVM>();
            DateTime dtStart = DateTime.MinValue;
            if (!string.IsNullOrEmpty(StartDate))
            {
                dtStart = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
            }

            DateTime dtEnd = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(StartDate))
            {
                dtEnd = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);
            }

            lstDelers = (from cu in _db.tbl_PurchaseDealers
                         where !cu.IsDelete.Value && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                         select new PurchaseDealerVM
                         {
                             Pk_Dealer_Id = cu.Pk_Dealer_Id,
                             FirmName = cu.FirmName,
                             FirmCity = cu.FirmCity,
                             State = cu.State,
                             Email = cu.Email,
                             FirmContactNo = cu.FirmContactNo,
                             OwnerName = cu.OwnerName,
                             IsActive = cu.IsActive.Value,
                             Status = cu.Status.HasValue ? cu.Status.Value : 0,
                             CreatedDate = cu.CreatedDate.Value,
                             OwnerContactNo = cu.OwnerContactNo,
                             BussinessCode = cu.BussinessCode
                         }).OrderByDescending(x => x.CreatedDate).ToList();


            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "BusinessCode", "Firm Name", "Firm Contact No", "Email", "City", "State", "Owner Name", "Owner Contact No", "Created Date", "IsActive" };
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Distributors Report";
            for (var col = 1; col < arrycolmns.Length + 1; col++)
            {
                workSheet.Cells[2, col].Style.Font.Bold = true;
                workSheet.Cells[2, col].Style.Font.Size = 12;
                workSheet.Cells[2, col].Value = arrycolmns[col - 1];
                workSheet.Cells[2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[2, col].AutoFitColumns(30, 70);
                workSheet.Cells[2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[2, col].Style.WrapText = true;
            }


            int row1 = 1;
            if (lstDelers != null && lstDelers.Count() > 0)
            {
                foreach (var objj in lstDelers)
                {
                    for (int j = 1; j < arrycolmns.Length + 1; j++)
                    {
                        workSheet.Cells[row1 + 2, j].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, j].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, j].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, j].AutoFitColumns(30, 70);
                        string vl = "";
                        if (arrycolmns[j - 1] == "BusinessCode")
                        {
                            vl = objj.BussinessCode;
                        }
                        else if (arrycolmns[j - 1] == "Firm Name")
                        {
                            vl = objj.FirmName;
                        }
                        else if (arrycolmns[j - 1] == "Firm Contact No")
                        {
                            vl = objj.FirmContactNo;
                        }
                        else if (arrycolmns[j - 1] == "Email")
                        {
                            vl = objj.Email;
                        }
                        else if (arrycolmns[j - 1] == "City")
                        {
                            vl = objj.FirmCity;
                        }
                        else if (arrycolmns[j - 1] == "State")
                        {
                            vl = objj.State;
                        }
                        else if (arrycolmns[j - 1] == "Owner Name")
                        {
                            vl = objj.OwnerName;
                        }
                        else if (arrycolmns[j - 1] == "Owner Contact No")
                        {
                            vl = objj.OwnerContactNo;
                        }
                        else if (arrycolmns[j - 1] == "Created Date")
                        {
                            vl = objj.CreatedDate.ToString("dd-MMM-yyyy");
                        }
                        else if (arrycolmns[j - 1] == "IsActive")
                        {
                            vl = objj.IsActive == true ? "Active" : "Inactive";
                        }
                        workSheet.Cells[row1 + 2, j].Value = vl;

                    }



                    row1 = row1 + 1;
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                //excel.Workbook.Worksheets.MoveToStart("Summary");  //move sheet from last to first : Code by Gunjan
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Purchase Dealers.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult Detail(long Id)
        {
            PurchaseDealerVM objPurchaseDealerVM = (from cu in _db.tbl_PurchaseDealers
                                                    where !cu.IsDelete.Value && cu.Pk_Dealer_Id == Id
                                                    select new PurchaseDealerVM
                                                    {
                                                        Pk_Dealer_Id = cu.Pk_Dealer_Id,
                                                        FirmName = cu.FirmName,
                                                        FirmAddress = cu.FirmAddress,
                                                        Email = cu.Email,
                                                        FirmCity = cu.FirmCity,
                                                        FirmGSTNo = cu.FirmGSTNo,
                                                        VisitingCardPhoto = cu.VisitingCardPhoto,
                                                        FirmContactNo = cu.FirmContactNo,
                                                        AlternateNo = cu.AlternateNo,
                                                        BusinessDetails = cu.BusinessDetails,
                                                        BankAcNumber = cu.BankAcNumber,
                                                        IFSCCode = cu.IFSCCode,
                                                        BankBranch = cu.BankBranch,
                                                        BankAcNumber2 = cu.BankAcNumber2,
                                                        BankBranch2 = cu.BankBranch2,
                                                        IFSCCode2 = cu.IFSCCode2,
                                                        OwnerName = cu.OwnerName,
                                                        OwnerContactNo = cu.OwnerContactNo,
                                                        Pincode = cu.Pincode,
                                                        VisitingCardPhoto2 = cu.VisitingCardPhoto2,
                                                        Remark = cu.Remark,
                                                        BussinessCode = cu.BussinessCode,
                                                        Status = cu.Status.HasValue ? cu.Status.Value : 0,
                                                        State = cu.State,
                                                        IsActive = cu.IsActive.Value
                                                    }).FirstOrDefault();


            return View(objPurchaseDealerVM);
        }

        public ActionResult Add()
        {
            PurchaseDealerVM obj = new PurchaseDealerVM();

            try
            {
                obj.StateList = GetStatesList();
            }
            catch (Exception ex)
            {
            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult Add(PurchaseDealerVM objPurchaseDealerVM, HttpPostedFileBase VisitingCardPhotoFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    #region Validation

                    // Duplicate EmailId
                    if (!string.IsNullOrEmpty(objPurchaseDealerVM.Email))
                    {
                        var duplicateEmail = _db.tbl_PurchaseDealers.Where(x => x.Email.ToLower() == objPurchaseDealerVM.Email.ToLower()).FirstOrDefault();
                        if (duplicateEmail != null)
                        {
                            objPurchaseDealerVM.StateList = GetStatesList();

                            ModelState.AddModelError("Email", ErrorMessage.EmailExists);
                            return View(objPurchaseDealerVM);
                        }
                    }

                    // Duplicate OwnerContactNo
                    if (!string.IsNullOrEmpty(objPurchaseDealerVM.OwnerContactNo))
                    {
                        var duplicateOwnerContactNo = _db.tbl_PurchaseDealers.Where(x => x.OwnerContactNo.ToLower() == objPurchaseDealerVM.OwnerContactNo.ToLower()).FirstOrDefault();
                        if (duplicateOwnerContactNo != null)
                        {
                            objPurchaseDealerVM.StateList = GetStatesList();

                            ModelState.AddModelError("OwnerContactNo", ErrorMessage.OwnerContactNoExists);
                            return View(objPurchaseDealerVM);
                        }
                    }

                    // Duplicate FirmContactNo
                    if (!string.IsNullOrEmpty(objPurchaseDealerVM.FirmContactNo))
                    {
                        var duplicateFirmContactNo = _db.tbl_PurchaseDealers.Where(x => x.FirmContactNo.ToLower() == objPurchaseDealerVM.FirmContactNo.ToLower()).FirstOrDefault();
                        if (duplicateFirmContactNo != null)
                        {
                            objPurchaseDealerVM.StateList = GetStatesList();

                            ModelState.AddModelError("FirmContactNo", ErrorMessage.FirmContactNoExists);
                            return View(objPurchaseDealerVM);
                        }
                    }

                    #endregion 

                    string fileName = string.Empty;
                    string path = Server.MapPath(UsersDocumentsDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (VisitingCardPhotoFile != null)
                    {
                        string ext = Path.GetExtension(VisitingCardPhotoFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(VisitingCardPhotoFile.FileName);

                        fileName = Guid.NewGuid() + ext;
                        VisitingCardPhotoFile.SaveAs(path + fileName);
                    }

                    tbl_PurchaseDealers objRequest = new tbl_PurchaseDealers();
                    objRequest.FirmName = objPurchaseDealerVM.FirmName;
                    objRequest.FirmAddress = objPurchaseDealerVM.FirmAddress;
                    objRequest.FirmCity = objPurchaseDealerVM.FirmCity;
                    objRequest.FirmGSTNo = objPurchaseDealerVM.FirmGSTNo;
                    objRequest.VisitingCardPhoto = objPurchaseDealerVM.VisitingCardPhoto;
                    objRequest.FirmContactNo = objPurchaseDealerVM.FirmContactNo;
                    objRequest.AlternateNo = objPurchaseDealerVM.AlternateNo;
                    objRequest.Email = objPurchaseDealerVM.Email;
                    objRequest.BusinessDetails = objPurchaseDealerVM.BusinessDetails;
                    objRequest.BankAcNumber = objPurchaseDealerVM.BankAcNumber;
                    objRequest.IFSCCode = objPurchaseDealerVM.IFSCCode;
                    objRequest.BankBranch = objPurchaseDealerVM.BankBranch;
                    objRequest.BankAcNumber2 = objPurchaseDealerVM.BankAcNumber2;
                    objRequest.IFSCCode2 = objPurchaseDealerVM.IFSCCode2;
                    objRequest.BankBranch2 = objPurchaseDealerVM.BankBranch2;
                    objRequest.OwnerName = objPurchaseDealerVM.OwnerName;
                    objRequest.OwnerContactNo = objPurchaseDealerVM.OwnerContactNo;
                    objRequest.Remark = objPurchaseDealerVM.Remark;
                    objRequest.BussinessCode = "";
                    objRequest.IsActive = true;
                    objRequest.IsDelete = false;
                    objRequest.State = objPurchaseDealerVM.State;
                    objRequest.Status = 1;
                    objRequest.CreatedBy = clsAdminSession.UserID;
                    objRequest.CreatedDate = DateTime.Now;
                    objRequest.ModifiedDate = DateTime.Now;
                    objRequest.ModifiedBy = clsAdminSession.UserID;
                    objRequest.Password = objPurchaseDealerVM.Password;
                    objRequest.VisitingCardPhoto = fileName;
                    _db.tbl_PurchaseDealers.Add(objRequest);
                    _db.SaveChanges();

                    if (objRequest.Pk_Dealer_Id.ToString().Length > 5)
                    {
                        objRequest.BussinessCode = "SSBC" + objRequest.Pk_Dealer_Id;
                    }
                    else
                    {
                        objRequest.BussinessCode = "SSBC" + objRequest.Pk_Dealer_Id.ToString("00000");
                    }

                    _db.SaveChanges();

                    #region Send Email

                    try
                    {
                        string ToEmail = objRequest.Email;
                        tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        string FromEmail = objGensetting.FromEmail;
                        string Subject = "Your Registration As a Purchase Dealer Created - Shopping & Saving";
                        string bodyhtml = "Thank You For Become A Valuable Dealer Of Shopping & Saving<br/>";
                        bodyhtml += "Following Are The Login Details:<br/>";
                        bodyhtml += "===============================<br/>";
                        bodyhtml += "BusinessCode: " + objRequest.BussinessCode + "<br/>";
                        bodyhtml += "Password: " + objRequest.Password + "<br/>";
                        clsCommon.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                    }
                    catch (Exception e)
                    {
                        string ErrorMessage = e.Message.ToString();
                    }


                    #endregion

                    #region Send SMS

                    using (WebClient webClient = new WebClient())
                    {
                        WebClient client = new WebClient();
                        Random random = new Random();
                        int num = random.Next(111566, 999999);
                        string msg = "Thank You For Become A Valuable Purchase Dealer Of Shopping & Saving\n";
                        msg += "Login Details:\n";
                        msg += "BusinessCode:" + objRequest.BussinessCode + "\n";
                        msg += "Password:" + objRequest.Password + "\n";
                        //int SmsId = (int)SMSType.DistributorReqAccepted;
                        //clsCommon objcm = new clsCommon();
                        //string msg = objcm.GetSmsContent(SmsId);
                        // msg = msg.Replace("{{MobileNo}}", objReq.MobileNo + "").Replace("{{Password}}", Password);
                        msg = HttpUtility.UrlEncode(msg);
                        //string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + objReq.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";                            
                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", objRequest.FirmContactNo).Replace("--MSG--", msg);
                        var json = webClient.DownloadString(url);
                        if (json.Contains("invalidnumber"))
                        {
                            /// return "InvalidNumber";
                        }
                        else
                        {
                            //  return num.ToString();
                        }

                    }

                    #endregion

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objPurchaseDealerVM);
        }

        private List<SelectListItem> GetStatesList()
        {
            List<string> lstStates = _db.tbl_PincodeCityState.Select(x => x.State).Distinct().OrderBy(x => x).ToList();

            var GetStatesList = lstStates
                         .Select(o => new SelectListItem { Value = o, Text = o })
                         .OrderBy(x => x.Text).ToList();

            return GetStatesList;
        }
    }
}
