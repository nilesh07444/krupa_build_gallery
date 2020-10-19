using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace KrupaBuildGallery.Model
{
    public class clsCommon
    {
        private static string _securityKey = "KrupaGallary";

        public static string EncryptString(string PlainText)
        {
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("+", "_");
        }
        public static string DecryptString(string CipherText)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText.Replace("_", "+"));
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static void SendEmail(string To, string from, string subject, string body)
        {
            try
            {
                krupagallarydbEntities _db = new krupagallarydbEntities();
                MailMessage mailMessage = new MailMessage(
                      from, // From field
                      To, // Recipient field
                     subject, // Subject of the email message
                      body // Email message body
           );
                mailMessage.From = new MailAddress(from, "Shopping & Saving");

                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string SMTPHost = objGensetting.SMTPHost;
                string SMTpPort = objGensetting.SMTPPort;
                string SMTPEMail = objGensetting.SMTPEmail;
                string SMTPPwd = objGensetting.SMTPPwd;
                string EnablSSL = "false";
                if (objGensetting.EnableSSL == true)
                {
                    EnablSSL = "true";
                }
                mailMessage.IsBodyHtml = true;
                // System.Net.Mail.MailMessage mailMessage = (System.Net.Mail.MailMessage)mailMsg;

                /* Setting should be kept somewhere so no need to 
                   pass as a parameter (might be in web.config)       */
                using (SmtpClient client = new SmtpClient(SMTPHost, Convert.ToInt32(SMTpPort)))
                {
                    // Configure the client
                    if (EnablSSL == "true")
                    {
                        client.EnableSsl = true;
                    }
                    else
                    {
                        client.EnableSsl = false;
                    }

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(SMTPEMail, SMTPPwd);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void SaveTransaction(long ProductItemId, long OrderItemId, long OrderId, string Description, decimal Amount, long CreatedByUser, long CreadtedByAdmin, DateTime CreatedDate, string TypeOfTransaction)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            tbl_Transactions objTrans = new tbl_Transactions();
            objTrans.ProductItemId = ProductItemId;
            objTrans.OrderItemId = OrderItemId;
            objTrans.OrderId = OrderId;
            objTrans.Description = Description;
            objTrans.CreatedByUserId = CreatedByUser;
            objTrans.CreatedByAdminId = CreadtedByAdmin;
            objTrans.CreatedDate = DateTime.UtcNow;
            objTrans.TypeOfTransaction = TypeOfTransaction;
            _db.tbl_Transactions.Add(objTrans);
            _db.SaveChanges();
        }

        public void SavePaymentTransaction(long OrderItemId, long OrderId,bool IsCredit,decimal Amount,string Remarks,long TransactionBy, bool IsAdmin, DateTime TransactionDate, string ModeOfPayment)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            tbl_PaymentTransaction objTrans = new tbl_PaymentTransaction();            
            objTrans.OrderItemId = OrderItemId;
            objTrans.OrderId = OrderId;
            objTrans.Remarks = Remarks;
            objTrans.TransactionBy = TransactionBy;
            objTrans.IsAdmin = IsAdmin;
            objTrans.IsCredit = IsCredit;
            objTrans.Amount = Amount;
            objTrans.TransactionDate = TransactionDate;
            objTrans.ModeOfPayment = ModeOfPayment;
            _db.tbl_PaymentTransaction.Add(objTrans);
            _db.SaveChanges();
        }

        public static EmailMessageVM GetSampleEmailTemplate()
        {
            string HeaderContent = GetFileText("EmailTemplates\\Header.htm");
            string FooterContent = GetFileText("EmailTemplates\\Footer.htm");
            string BodyContent = GetFileText("EmailTemplates\\AccountBlocked.htm");

            string body = HeaderContent + BodyContent + FooterContent;
            string subject = "Test Email By Nilesh";
            string toEmail = "krutik.shah1310@gmail.com";

            return new EmailMessageVM()
            {
                SendEmailTo = { toEmail },
                Subject = subject,
                Body = body
            };
        }

        public static string GetFileText(string filePathName)
        {
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            string FullPath = Path.Combine(dirPath, filePathName);

            return File.ReadAllText(FullPath);
        }

        public static void SendEmail2(EmailMessageVM emailModel)
        {

            string fromEmailID = string.Empty;
            string emailFromName = string.Empty;

            try
            {
                krupagallarydbEntities _db = new krupagallarydbEntities();
                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();

                // as per the application run mode, assign proper from email address from configuration file.
                using (SmtpClient emailClient = new SmtpClient())
                {
                    emailClient.EnableSsl = objGensetting.EnableSSL == true ? true : false;
                    //emailClient.UseDefaultCredentials = false; 

                    emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    emailClient.Host = objGensetting.SMTPHost;
                    emailClient.Port = Convert.ToInt32(objGensetting.SMTPPort);
                    emailClient.Credentials = new NetworkCredential(
                        objGensetting.SMTPEmail,
                        objGensetting.SMTPPwd
                    );
                    fromEmailID = objGensetting.SMTPEmail;
                    emailFromName = "Shopping & Saving";


                    //Create SMTP message
                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(fromEmailID, emailFromName),
                        Subject = emailModel.Subject,
                        Body = emailModel.Body,
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8
                    };

                    emailModel.SendEmailTo.ForEach(x => message.To.Add(x));
                    emailModel.SendEmailCc.ForEach(x => message.CC.Add(x));
                    emailModel.SendEmailBcc.ForEach(x => message.Bcc.Add(x));

                    //Send message.
                    emailClient.Timeout = 10000;
                    emailClient.Send(message);
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

        }

        public static string GetCurrentFinancialYear()
        {
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(DateTime.UtcNow), nzTimeZone);
            int CurrentYear = dateTimeAsTimeZone.Year;
            int PreviousYear = dateTimeAsTimeZone.Year - 1;
            int NextYear = dateTimeAsTimeZone.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;
            return FinYear.Trim();
        }
    
        public string GetSmsContent(int Id)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            var objSms = _db.tbl_SMSContent.Where(o => o.SMSContentId == Id).FirstOrDefault();
            if(objSms != null)
            {
                return objSms.SMSDescription.ToString();
            }
            else
            {
                return "";
            }
        }

        public static ChtMessage GetMessageModel(tbl_ChatMessages objm)
        {
            ChtMessage objMsg = new ChtMessage();
            objMsg.ChatMeesageId = objm.ChatMeesageId;
            objMsg.FromUserId = objm.FromUserId.Value;
            objMsg.ToUserId = objm.ToUserId.Value;
            objMsg.Message = objm.Message;
            objMsg.Status = objm.Status;
            objMsg.MessageDate = CommonMethod.ConvertFromUTCNew(objm.MessageDate.Value);
            return objMsg;
        }

        public MessageRecords GetChatMessagesByUserID(long currentUserID, long toUserID, int lastMessageID = 0)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            MessageRecords obj = new MessageRecords();
            List<long> msgIdsdeleted = _db.tbl_DeletedChatMessage.Where(o => o.UserId == currentUserID).Select(x => x.MessageId.Value).ToList();
            var messages = _db.tbl_ChatMessages.Where(m => !msgIdsdeleted.Contains(m.ChatMeesageId) && (m.ToUserId == toUserID || m.FromUserId == toUserID) && (m.ToUserId == currentUserID || m.FromUserId == currentUserID)).OrderByDescending(m => m.MessageDate);
            if (lastMessageID > 0)
            {
                obj.Messages = messages.Where(m => m.ChatMeesageId < lastMessageID).Take(20).ToList().OrderBy(m => m.MessageDate).ToList();
            }
            else
            {
                obj.Messages = messages.Take(20).ToList().OrderBy(m => m.MessageDate).ToList();
            }
            obj.LastChatMessageId = obj.Messages.OrderBy(m => m.ChatMeesageId).Select(m => m.ChatMeesageId).FirstOrDefault();
            return obj;
        }
        public bool IsOnlineUser(long UserId)
        {
            bool IsOnline = false;
            krupagallarydbEntities _db = new krupagallarydbEntities();
            var lstOnlines =  _db.tbl_ChatUsers.Where(o => o.UserId == UserId && o.IsOnline == true).ToList();
            if(lstOnlines != null && lstOnlines.Count() > 0)
            {
                IsOnline = true;
            }

            return IsOnline;
        }

        public List<OnlineUserDetails> GetRecentChats(long currentUserID)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            List<tbl_AdminUsers> lstAdminss =  _db.tbl_AdminUsers.Where(o => o.AdminUserId != currentUserID && o.AdminRoleId != 2 && o.AdminRoleId != 3 && o.AdminRoleId != 4 && o.IsActive == true && o.IsDeleted == false).ToList();

            var recentMessages = _db.tbl_ChatMessages.Where(m => (m.ToUserId == currentUserID || m.FromUserId == currentUserID)).OrderByDescending(m => m.MessageDate).ToList();
            var userIds = recentMessages.Select(m => (m.ToUserId == currentUserID ? m.FromUserId : m.ToUserId)).Distinct().ToArray();
            var userIdsList = userIds.ToList();
            var messagesByUserId = recentMessages.Where(m => m.ToUserId == currentUserID && m.Status == "Sent").ToList();
            var newMessagesCount = (from p in messagesByUserId
                                    group p by p.FromUserId into g
                                    select new { FromUserID = g.Key, Count = g.Count() }).ToList();
           
            var users = (from m in lstAdminss
                         join v in userIdsList on m.AdminUserId equals v
                         select new OnlineUserDetails
                         {
                             UserID = m.AdminUserId,
                             Name = m.FirstName+" "+m.LastName,
                             ProfilePicture = m.ProfilePicture
                         }).ToList();
            users.ForEach(m =>
            {
                m.UnReadMessageCount = newMessagesCount.Where(x => x.FromUserID == m.UserID).Select(x => x.Count).FirstOrDefault();
            });
            users = users.OrderBy(d => userIdsList.IndexOf(d.UserID)).ToList();

            var users1 = (from m in lstAdminss
                          where !userIdsList.Contains(m.AdminUserId)
                         select new OnlineUserDetails
                         {
                             UserID = m.AdminUserId,
                             Name = m.FirstName + " " + m.LastName,
                             ProfilePicture = m.ProfilePicture,
                             UnReadMessageCount = 0
                         }).ToList();

            users.AddRange(users1);
            return users;
        }
    }


    enum OrderStatus
    {
        NewOrder = 1,
        Confirmed = 2,
        Dispatched = 3,
        Delivered = 4,
        Cancelled = 5,
        Return = 6,
        Replace = 7,
        Exchange = 8
    }

    enum OrderItemStatus
    {
        NewOrder = 1,
        Confirmed = 2,
        Dispatched = 3,
        Delivered = 4,
        Cancelled = 5,
        Return = 6,
        Replace = 7,
        Exchange = 8
    }

    enum SMSType
    {
        LoginOtp = 1,
        RegistrationOtp = 2,
        ForgotPwdOtp = 3,
        ChangePwdOtp = 4,
        NewUser = 5,
        NewOrderClient = 6,
        NewOrderAdmin = 7,
        ContactForm = 8,
        DistributorReqOtp = 9,
        ItemActionOtp = 10,
        ItemCancelClient = 11,
        ItemCancelAdmin = 12,
        ReturnReqAdmin = 13,
        ReplaceReqAdmin = 14,
        ExchangeReqAdmin = 15,
        RejectCashToSender = 16,
        WhenDelivered = 17,
        OtpDuringDelivery = 18,
        OtpDuringDeliveryWithCash = 19,
        AgentAssignDelieveryToStaff = 20,
        AdminLoginChangePwdOtp = 21,
        DistributorReqRejected = 22,
        DistributorReqAccepted = 23,
        CreditLimitChange = 24,
        LoginOtpForAdmin = 25,
        OrderConfirmed = 26,
        OrderDispatched = 27,
        DueAmtRemind = 28,
        ExchangeReqRejected = 29,
        ReplaceReqRejected = 30,
        ReturnReqRejected = 31,
        ReturnReqAccepted = 32,
        ReplaceReqAccepted = 33,
        ExchangeReqAccepted = 34,
        AssignItemtoStaff = 35,
        ShippingChargeSet = 36,
        AssignItemtoStaffMsgToClient = 37

    }
}