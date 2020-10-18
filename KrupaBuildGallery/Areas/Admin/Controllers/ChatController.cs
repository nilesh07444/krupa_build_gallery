using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrupaBuildGallery.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        // GET: Admin/Chat
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChatMessageWindow(long UserId)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            ViewData["objUser"] = _db.tbl_AdminUsers.Where(o => o.AdminUserId == UserId).FirstOrDefault();
            clsCommon objCl = new clsCommon();
            MessageRecords objMsgRecr = objCl.GetChatMessagesByUserID(clsAdminSession.UserID, UserId);
            ChatMessageModel objChatmsg = new ChatMessageModel();
            objChatmsg.ChatMessages = objMsgRecr.Messages.Select(x => clsCommon.GetMessageModel(x)).ToList();
            objChatmsg.LastChatMessageId = objMsgRecr.LastChatMessageId;
            objChatmsg.IsOnline = objCl.IsOnlineUser(UserId);

            ViewData["objChatmsg"] = objChatmsg;
            return PartialView("~/Areas/Admin/Views/Chat/_ChatMessageWindow.cshtml");
        }

        public ActionResult GetRecentChatUsers()
        {
            clsCommon objcl = new clsCommon();
            List<OnlineUserDetails> lstrecentChatsuser = objcl.GetRecentChats(clsAdminSession.UserID);
            ViewData["lstrecentChatsuser"] = lstrecentChatsuser;
            return PartialView("~/Areas/Admin/Views/Chat/_UsersChat.cshtml");
        }

        public ActionResult GetRecentMessages(long Id, int lastChatMessageId)
        {
            clsCommon objCl = new clsCommon();
            MessageRecords objMsgRecr = objCl.GetChatMessagesByUserID(clsAdminSession.UserID, Id,lastChatMessageId);
           
            var objmodel = new ChatMessageModel();
            objmodel.ChatMessages = objMsgRecr.Messages.Select(m => clsCommon.GetMessageModel(m)).ToList();
            objmodel.LastChatMessageId = objMsgRecr.LastChatMessageId;
            return Json(objmodel, JsonRequestBehavior.AllowGet);
        }
    }
}