using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using KrupaBuildGallery.Model;
using KrupaBuildGallery.ViewModel;
using Microsoft.AspNet.SignalR;

namespace KrupaBuildGallery.hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnected()
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            var userID = Context.QueryString["UserID"];
            if (userID != null)
            {
                int uId = Convert.ToInt32(userID);
                tbl_ChatUsers objCU = new tbl_ChatUsers();
                objCU.ConnectionId = Context.ConnectionId;
                objCU.UserId = Convert.ToInt64(userID);
                objCU.IsOnline = true;
                objCU.CreatedOn = DateTime.UtcNow;
                _db.tbl_ChatUsers.Add(objCU);
                _db.SaveChanges();
                RefereshOnline();
                // RefreshOnlineUsers(uId);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            var userID = Context.QueryString["UserID"];
            if (userID != null)
            {
                int uId = Convert.ToInt32(userID);
                tbl_ChatUsers objCU = _db.tbl_ChatUsers.Where(o => o.UserId == uId && o.ConnectionId == Context.ConnectionId).FirstOrDefault();
                objCU.IsOnline = false;
                _db.SaveChanges();
                //RefreshOnlineUsers(uId);
                RefereshOnline();
            }
            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(int fromUserId, int toUserId, string message, string fromUserName, string fromUserProfilePic, string toUserName, string toUserProfilePic)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            tbl_ChatMessages objChtmsg = new tbl_ChatMessages();
            objChtmsg.FromUserId = fromUserId;
            objChtmsg.ToUserId = toUserId;
            objChtmsg.Message = message;
            objChtmsg.Status = "Sent";
            objChtmsg.MessageDate = DateTime.Now.ToUniversalTime();
            objChtmsg.UpdateDate = DateTime.Now.ToUniversalTime();
            _db.tbl_ChatMessages.Add(objChtmsg);
            _db.SaveChanges();

            ChtMessage objMsg = new ChtMessage();
            objMsg.ChatMeesageId = objChtmsg.ChatMeesageId;
            objMsg.FromUserId = fromUserId;
            objMsg.ToUserId = toUserId;
            objMsg.Message = message;
            objMsg.Status = "Sent";
            objMsg.MessageDate = objChtmsg.MessageDate.Value.ToString("dd-MMM-yyyy hh:mm tt");

            long[] arryIds = new long[] { fromUserId, toUserId };
            List<long> lstUsrid = arryIds.ToList();
            List<string> lstcnctIds  = _db.tbl_ChatUsers.Where(m => lstUsrid.Contains(m.UserId.Value) && m.IsOnline == true).Select(m => m.ConnectionId).ToList();
            Clients.Clients(lstcnctIds).AddNewChatMessage(objMsg, fromUserId, toUserId, fromUserName, fromUserProfilePic, toUserName, toUserProfilePic);
            RefereshOnline();
        }

        public void RefereshOnline()
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            DateTime dtPrev = DateTime.UtcNow.AddDays(-2);
            List<long> lstusrIds = _db.tbl_ChatUsers.Where(m => m.IsOnline == true && m.CreatedOn > dtPrev).Select(m => m.UserId.Value).Distinct().ToList();
            string strUserIds = "";
            if (lstusrIds != null && lstusrIds.Count() > 0)
            {
                strUserIds = string.Join("^", lstusrIds);
            }

            Clients.All.RefershOnlineUsr(strUserIds);
        }

        public void SendUserTypingStatus(long toUserID,long fromUserID)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            List<string> connectionIds = _db.tbl_ChatUsers.Where(m => m.UserId.Value == toUserID && m.IsOnline == true).Select(m => m.ConnectionId).ToList();
            if (connectionIds.Count > 0)
            {
                Clients.Clients(connectionIds).UserIsTyping(fromUserID);
            }
        }
        public void UpdateMessageStatus(int messageID, int currentUserID, int fromUserID)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            if (messageID > 0)
            {
                var unreadMessages = _db.tbl_ChatMessages.Where(m => m.ChatMeesageId == messageID).FirstOrDefault();
                if (unreadMessages != null)
                {
                    unreadMessages.Status = "Viewed";                    
                    _db.SaveChanges();
                }                
            }
            else
            {
                var unreadMessages = _db.tbl_ChatMessages.Where(m => m.Status == "Sent" && m.ToUserId == currentUserID && m.FromUserId == fromUserID).ToList();
                unreadMessages.ForEach(m =>
                {
                    m.Status = "Viewed";                
                });
                _db.SaveChanges();              
            }
            long[] arryIds = new long[] { currentUserID, fromUserID };
            List<long> lstUsrid = arryIds.ToList();
            List<string> lstcnctIds = _db.tbl_ChatUsers.Where(m => lstUsrid.Contains(m.UserId.Value) && m.IsOnline == true).Select(m => m.ConnectionId).ToList();
            Clients.Clients(lstcnctIds).UpdateMessageStatusInChatWindow(messageID, currentUserID, fromUserID);
        }
        public void deletemsg(long fromUserID,long MsgId)
        {
            krupagallarydbEntities _db = new krupagallarydbEntities();
            tbl_DeletedChatMessage objmsg = new tbl_DeletedChatMessage();
            objmsg.UserId = fromUserID;
            objmsg.MessageId = MsgId;
            objmsg.DeletedDate = DateTime.UtcNow;
            _db.tbl_DeletedChatMessage.Add(objmsg);
            _db.SaveChanges();
        }
    }
}