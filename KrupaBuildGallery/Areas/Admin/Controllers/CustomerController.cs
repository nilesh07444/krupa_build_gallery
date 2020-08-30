using ConstructionDiary.Models;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class CustomerController : Controller
    {
        private readonly krupagallarydbEntities _db;
        public CustomerController()
        {
            _db = new krupagallarydbEntities();
        }
        // GET: Admin/Customer
        public ActionResult Index(int Status = -1,string StartDate = "",string EndDate = "")
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
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

                lstClientUser = (from cu in _db.tbl_ClientUsers
                             join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                             where !cu.IsDelete && cu.ClientRoleId == 1 && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                             select new ClientUserVM
                             {
                                 ClientUserId = cu.ClientUserId,
                                 FirstName = cu.FirstName,
                                 LastName = cu.LastName,
                                 UserName = cu.UserName,
                                 Email = cu.Email,
                                 Password = cu.Password,
                                 RoleId = cu.ClientRoleId,
                                 CompanyName = cu.CompanyName,
                                 ProfilePic = cu.ProfilePicture,
                                 MobileNo = cu.MobileNo,
                                 IsActive = cu.IsActive,
                                 City = co.City,
                                 State = co.State,
                                 AddharCardNo = co.Addharcardno,
                                 PanCardNo = co.Pancardno,
                                 CreatedDate = co.CreatedDate,
                                 GSTNo = co.GSTno
                             }).OrderByDescending(x => x.CreatedDate).ToList();
                if (Status == 1)
                {
                    List<long> clientuserids = lstClientUser.Select(o => o.ClientUserId).ToList();
                    List<long> pendingshippingdistri = _db.tbl_Orders.Where(o => o.ShippingStatus == 1 && clientuserids.Contains(o.ClientUserId)).Select(o => o.ClientUserId).Distinct().ToList();
                    lstClientUser = lstClientUser.Where(o => pendingshippingdistri.Contains(o.ClientUserId)).ToList();
                }
                ViewBag.Status = Status;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstClientUser);
        }

        public void Export(int Status = -1, string StartDate = "", string EndDate = "")
        {
            ExcelPackage excel = new ExcelPackage();
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
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

            lstClientUser = (from cu in _db.tbl_ClientUsers
                             join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                             where !cu.IsDelete && cu.ClientRoleId == 1 && cu.CreatedDate >= dtStart && cu.CreatedDate <= dtEnd
                             select new ClientUserVM
                             {
                                 ClientUserId = cu.ClientUserId,
                                 FirstName = cu.FirstName,
                                 LastName = cu.LastName,
                                 UserName = cu.UserName,
                                 Email = cu.Email,
                                 Password = cu.Password,
                                 RoleId = cu.ClientRoleId,
                                 CompanyName = cu.CompanyName,
                                 ProfilePic = cu.ProfilePicture,
                                 MobileNo = cu.MobileNo,
                                 IsActive = cu.IsActive,
                                 City = co.City,
                                 State = co.State,
                                 AddharCardNo = co.Addharcardno,
                                 PanCardNo = co.Pancardno,
                                 GSTNo = co.GSTno,
                                 CreatedDate = co.CreatedDate
                             }).OrderBy(x => x.FirstName).ToList();
            if (Status == 1)
            {
                List<long> clientuserids = lstClientUser.Select(o => o.ClientUserId).ToList();
                List<long> pendingshippingdistri = _db.tbl_Orders.Where(o => o.ShippingStatus == 1 && clientuserids.Contains(o.ClientUserId)).Select(o => o.ClientUserId).Distinct().ToList();
                lstClientUser = lstClientUser.Where(o => pendingshippingdistri.Contains(o.ClientUserId)).ToList();
            }
         
            StringBuilder sb = new StringBuilder();
            string[] arrycolmns = new string[] { "First Name", "Last Name", "Mobile Number", "Email", "City","State","Created Date","IsActive"};
            var workSheet = excel.Workbook.Worksheets.Add("Report");
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Size = 20;
            workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            workSheet.Cells[1, 1].Value = "Customer Report";
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
            if (lstClientUser != null && lstClientUser.Count() > 0)
            {
                foreach (var objj in lstClientUser)
                {
                    for(int j=1;j< arrycolmns.Length+1;j++)
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
                        if(arrycolmns[j-1] == "First Name")
                        {
                            vl = objj.FirstName;
                        }
                        else if(arrycolmns[j - 1] == "Last Name")
                        {
                            vl = objj.LastName;
                        }
                        else if (arrycolmns[j - 1] == "Mobile Number")
                        {
                            vl = objj.MobileNo;
                        }
                        else if (arrycolmns[j - 1] == "Email")
                        {
                            vl = objj.Email;
                        }
                        else if (arrycolmns[j - 1] == "City")
                        {
                            vl = objj.City;
                        }
                        else if (arrycolmns[j - 1] == "State")
                        {
                            vl = objj.State;
                        }
                        else if (arrycolmns[j - 1] == "Created Date")
                        {
                            vl = objj.CreatedDate.ToString("dd-MMM-yyyy");
                        }
                        else if (arrycolmns[j - 1] == "IsActive")
                        {
                            vl = objj.IsActive == true ? "Active":"Inactive";
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
                Response.AddHeader("content-disposition", "attachment;  filename=Customers.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_ClientUsers objtbl_ClientUsers = _db.tbl_ClientUsers.Where(x => x.ClientUserId == Id).FirstOrDefault();

                if (objtbl_ClientUsers != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_ClientUsers.IsActive = true;
                    }
                    else
                    {
                        objtbl_ClientUsers.IsActive = false;
                    }

                    objtbl_ClientUsers.UpdatedBy = LoggedInUserId;
                    objtbl_ClientUsers.UpdatedDate = DateTime.UtcNow;

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

        public ActionResult Detail(long Id)
        {
            ClientUserVM objClientUserVM = (from cu in _db.tbl_ClientUsers
                                            join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                            where !cu.IsDelete && cu.ClientUserId == Id
                                            select new ClientUserVM
                                            {
                                                ClientUserId = cu.ClientUserId,
                                                FirstName = cu.FirstName,
                                                LastName = cu.LastName,
                                                UserName = cu.UserName,
                                                Email = cu.Email,
                                                Password = cu.Password,
                                                RoleId = cu.ClientRoleId,
                                                CompanyName = cu.CompanyName,
                                                ProfilePic = cu.ProfilePicture,
                                                MobileNo = cu.MobileNo,
                                                IsActive = cu.IsActive,
                                                City = co.City,
                                                State = co.State,
                                                AddharCardNo = co.Addharcardno,
                                                PanCardNo = co.Pancardno,
                                                GSTNo = co.GSTno == "" ? "N/A" : co.GSTno,
                                                ShipAddress = co.ShipAddress,
                                                ShipCity = co.ShipCity,
                                                ShipPostalCode = co.ShipPostalcode,
                                                ShipState = co.ShipState,
                                                Refrence = cu.Reference                                                
                                            }).FirstOrDefault();

            List<OrderVM> lstOrders = new List<OrderVM>();
            lstOrders = (from p in _db.tbl_Orders
                         where p.ClientUserId == Id
                         select new OrderVM
                         {
                             OrderId = p.OrderId,
                             ClientUserId = p.ClientUserId,
                             OrderAmount = p.OrderAmount,
                             OrderShipCity = p.OrderShipCity,
                             OrderShipState = p.OrderShipState,
                             OrderShipAddress = p.OrderShipAddress,
                             OrderPincode = p.OrderShipPincode,
                             OrderShipClientName = p.OrderShipClientName,
                             OrderShipClientPhone = p.OrderShipClientPhone,
                             OrderStatusId = p.OrderStatusId,
                             OrderAmountDue = p.AmountDue.Value,
                             PaymentType = p.PaymentType,
                             OrderDate = p.CreatedDate
                         }).OrderByDescending(x => x.OrderDate).ToList();
            if (lstOrders != null && lstOrders.Count() > 0)
            {
                lstOrders.ForEach(x => x.OrderStatus = GetOrderStatus(x.OrderStatusId));
            }
            objClientUserVM.OrderList = lstOrders;

            //List<PointVM> lstPoints = new List<PointVM>();
            //lstPoints = (from p in _db.tbl_PointDetails
            //             where p.ClientUserId == Id
            //             select new PointVM
            //             {
            //                PointId = p.PointId,
            //                Points = p.Points.Value,
            //                UsedPoints = p.UsedPoints.Value,
            //                ExpiryDate = p.ExpiryDate.Value
            //             }).OrderBy(x => x.ExpiryDate).ToList();
            //objClientUserVM.PointsList = lstPoints;
            return View(objClientUserVM);
        }

        public ActionResult Points(long Id)
        {
            List<PointVM> lstPoints = new List<PointVM>();
            lstPoints = (from p in _db.tbl_PointDetails
                         where p.ClientUserId == Id
                         select new PointVM
                         {
                             PointId = p.PointId,
                             Points = p.Points.Value,
                             UsedPoints = p.UsedPoints.Value,
                             ExpiryDate = p.ExpiryDate.Value
                         }).OrderBy(x => x.ExpiryDate).ToList();

            var objclient = _db.tbl_ClientUsers.Where(o => o.ClientUserId == Id).FirstOrDefault();
            ViewBag.CustomerName = objclient.FirstName + " " + objclient.LastName;
            ViewBag.ClientUserId = Id;
            return View(lstPoints);
        }
        public string GetOrderStatus(long orderstatusid)
        {
            return Enum.GetName(typeof(OrderStatus), orderstatusid);
        }

        [HttpPost]
        public string DeletePoints(int PointId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PointDetails objPoint = _db.tbl_PointDetails.Where(x => x.PointId == PointId).FirstOrDefault();

                if (objPoint == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_PointDetails.Remove(objPoint);
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
        public string AddPoints(int ClientUserId,string Points)
        {
            string ReturnMessage = "";

            try
            {
                tbl_PointDetails objPoints = new tbl_PointDetails();
                objPoints.ClientUserId = ClientUserId;
                objPoints.ExpiryDate = DateTime.UtcNow.AddMonths(6);
                objPoints.Points = Convert.ToDecimal(Points);
                objPoints.UsedPoints = 0;
                objPoints.CreatedBy = clsAdminSession.UserID;
                objPoints.CreatedDate = DateTime.UtcNow;
                _db.tbl_PointDetails.Add(objPoints);
                _db.SaveChanges();
             
                ReturnMessage = "success";
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult PointAssignment()
        {
            return View();
        }

        public ActionResult GetPointAssignmentClientList(string Year, string MinAmount, string MaxAmount)
        {
            List<ClientUserVM> lstClientUser = new List<ClientUserVM>();
            List<ClientUserVM> lstClientUser1 = new List<ClientUserVM>();
            List<ClientUserVM> lstnew = new List<ClientUserVM>();
            try
            {
                string[] yr = Year.Split('-');
                decimal minamt = Convert.ToDecimal(MinAmount);
                decimal maxamt = Convert.ToDecimal(MaxAmount);
                DateTime dtStart = new DateTime(Convert.ToInt32(yr[0]), 4, 1);
                DateTime dtEnd = new DateTime(Convert.ToInt32(yr[1]), 3, 31);
                lstClientUser = (from cu in _db.tbl_ClientUsers
                                 join co in _db.tbl_ClientOtherDetails on cu.ClientUserId equals co.ClientUserId
                                 where !cu.IsDelete && cu.ClientRoleId == 1
                                 select new ClientUserVM
                                 {
                                     ClientUserId = cu.ClientUserId,
                                     FirstName = cu.FirstName,
                                     LastName = cu.LastName,
                                     UserName = cu.UserName,
                                     Email = cu.Email,
                                     Password = cu.Password,
                                     RoleId = cu.ClientRoleId,
                                     CompanyName = cu.CompanyName,
                                     ProfilePic = cu.ProfilePicture,
                                     MobileNo = cu.MobileNo,
                                     IsActive = cu.IsActive,
                                     City = co.City,
                                     State = co.State,
                                     AddharCardNo = co.Addharcardno,
                                     PanCardNo = co.Pancardno,
                                     GSTNo = co.GSTno
                                 }).OrderBy(x => x.FirstName).ToList();

                List<long> clientuserids = lstClientUser.Select(o => o.ClientUserId).ToList();              
                lstnew = (from pp in _db.tbl_Orders
                          where pp.CreatedDate >= dtStart && pp.CreatedDate <= dtEnd && clientuserids.Contains(pp.ClientUserId)
                          select new ClientUserVM
                          {
                              ClientUserId = pp.ClientUserId,
                              TotalAmountOfOrderPlaced = pp.OrderAmount
                          }).GroupBy(c => c.ClientUserId).Select(c => new ClientUserVM { ClientUserId = c.Key, TotalAmountOfOrderPlaced = c.Sum(d => d.TotalAmountOfOrderPlaced) }).ToList();

                lstClientUser1 = (from cu in lstnew
                                  join co in lstClientUser on cu.ClientUserId equals co.ClientUserId
                                  where cu.TotalAmountOfOrderPlaced >= minamt && cu.TotalAmountOfOrderPlaced <= maxamt
                                  select new ClientUserVM
                                  {
                                      ClientUserId = co.ClientUserId,
                                      FirstName = co.FirstName,
                                      LastName = co.LastName,
                                      UserName = co.UserName,
                                      Email = co.Email,
                                      Password = co.Password,
                                      RoleId = co.RoleId,
                                      CompanyName = co.CompanyName,
                                      ProfilePic = co.ProfilePic,
                                      MobileNo = co.MobileNo,
                                      IsActive = co.IsActive,
                                      City = co.City,
                                      State = co.State,
                                      AddharCardNo = co.AddharCardNo,
                                      PanCardNo = co.PanCardNo,
                                      TotalAmountOfOrderPlaced = cu.TotalAmountOfOrderPlaced,
                                      GSTNo = co.GSTNo
                                  }).OrderBy(x => x.FirstName).ToList();

                lstClientUser1.ForEach(x => x.PointRemaining = GetPointsRemaining(x.ClientUserId));
                ViewBag.Year = Year;
                ViewBag.MinAmt = MinAmount;
                ViewBag.MaxAmt = MaxAmount;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return PartialView("_PointAssignmentData",lstClientUser1);
        }
        public decimal GetPointsRemaining(long UserId)
        {
            DateTime dtNow = DateTime.UtcNow;
            long clientusrrId = UserId;
            List<tbl_PointDetails> lstpoints = _db.tbl_PointDetails.Where(o => o.ClientUserId == clientusrrId && o.ExpiryDate >= dtNow && o.Points.Value > o.UsedPoints.Value).ToList().OrderBy(x => x.ExpiryDate).ToList();
            decimal pointreamining = 0;
            if (lstpoints != null && lstpoints.Count() > 0)
            {
                pointreamining = lstpoints.Sum(x => (x.Points - x.UsedPoints).Value);
            }
            return pointreamining;
        }

        [HttpPost]
        public string AddMultiplePoints(string ClientUserIds, string Points)
        {
            string ReturnMessage = "";

            try
            {
                string[] arrystrclienuserid = ClientUserIds.Split(',');
                if(arrystrclienuserid != null && arrystrclienuserid.Count() > 0)
                {
                    foreach(string strusrid in arrystrclienuserid)
                    {
                        tbl_PointDetails objPoints = new tbl_PointDetails();
                        objPoints.ClientUserId = Convert.ToInt64(strusrid);
                        objPoints.ExpiryDate = DateTime.UtcNow.AddMonths(6);
                        objPoints.Points = Convert.ToDecimal(Points);
                        objPoints.UsedPoints = 0;
                        objPoints.CreatedBy = clsAdminSession.UserID;
                        objPoints.CreatedDate = DateTime.UtcNow;
                        _db.tbl_PointDetails.Add(objPoints);
                        _db.SaveChanges();
                    }
                }                

                ReturnMessage = "success";

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }
    }
}