using System;
using System.Collections.Generic;
using System.Configuration;
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
                MailMessage mailMessage = new MailMessage(
                      from, // From field
                      To, // Recipient field
                     subject, // Subject of the email message
                      body // Email message body
           );
                string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
                string SMTpPort = ConfigurationManager.AppSettings["SMTPPort"];
                string SMTPEMail = ConfigurationManager.AppSettings["SMTPEmail"];
                string SMTPPwd = ConfigurationManager.AppSettings["SMTPPwd"];
                mailMessage.IsBodyHtml = true;
                // System.Net.Mail.MailMessage mailMessage = (System.Net.Mail.MailMessage)mailMsg;

                /* Setting should be kept somewhere so no need to 
                   pass as a parameter (might be in web.config)       */
                using (SmtpClient client = new SmtpClient(SMTPHost, Convert.ToInt32(SMTpPort)))
                {
                    // Configure the client
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(SMTPEMail, SMTPPwd);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                }
            }
            catch(Exception e)
            {

            }           
        }

    }

    enum OrderStatus
    {
        NewOrder = 1,
        Confirmed = 2,
        Dispatched = 3
    }
}