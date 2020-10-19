var chat = $.connection.chatHub;

chat.client.addNewChatMessage = function (messageRow, fromUserId, toUserId, fromUserName, fromUserProfilePic, toUserName, toUserProfilePic) {
    var currentUserId = $('#hdfLoggedInUserID').val();
    var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
    if (currentChatUserID == fromUserId || currentChatUserID == toUserId) {
        var d = new Date(messageRow.MessageDate + ' GMT');
        var n = d.toLocaleTimeString();
        var n1 = d.toLocaleDateString();      
        var dtt = new Date(n1);
        var msgdtt = dtt.customFormat("#D#-#MMM#-#YYYY#") + " " + n;
        createNewMessageBlock(fromUserName, fromUserProfilePic,msgdtt, messageRow.Message, (currentUserId == fromUserId ? 'right' : 'left'), messageRow.ChatMeesageId, messageRow.Status);
        if (currentUserId != fromUserId) {
            var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
            setTimeout(function () {
                UpdateChatMessageStatus(messageRow.ChatMeesageId, currentChatUserID);
            }, 100);
        }
    }
    if (currentUserId != fromUserId) {
        var windowActive = $('#hdfWindowIsActiveOrNot').val();
        if (windowActive == 'False') {
            document.title = "Message received from " + fromUserName;
        }
    }
    addChatMessageCount(currentUserId, fromUserId, fromUserName, fromUserProfilePic, toUserId, toUserName, toUserProfilePic)
}
chat.client.userIsTyping = function (fromUserId) {
    var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
    if (currentChatUserID == fromUserId) {
        $(".clswriting").css("opacity", "1");
        $(document).find('.clswriting').html('writing...');
        setTimeout(function () {
        //    $(document).find('.clswriting').html('');
            $(".clswriting").css("opacity", "0");
        }, 1000);
    }
}
chat.client.updateMessageStatusInChatWindow = function (messageID, currentUserID, fromUserID) {
    if (messageID > 0) {
        var message = $(document).find('span[class="chat-message-status"][data-chat-message-id="' + messageID + '"]');
        if (message.length > 0) {
            $(message).text('Viewed');
        }
    }
    else {
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        if (currentChatUserID == currentUserID) {
            var messages = $(document).find('span[class="chat-message-status"]');
            $(messages).each(function (index, item) {
                $(item).text('Viewed');
            });
        }
    }
}
chat.client.refershOnlineUsr = function (usrids) {
    setTimeout(function () {
        $(".chtrecentusr .list-group-item .avatar").removeClass("avatar-state-success");
        $(".chat-header-user .avatar").removeClass("avatar-state-success");
        if (usrids != "") {
            var arryusrid = usrids.split('^');
            if (arryusrid != null && arryusrid.length > 0) {
                for (var jj = 0; jj < arryusrid.length; jj++) {
                    var usrrid = arryusrid[jj];
                    $(".chtrecentusr .list-group-item[data-user-id=" + usrrid + "] .avatar").addClass("avatar-state-success");
                    $(".chat-header-user[data-onuserid=" + usrrid + "] .avatar").addClass("avatar-state-success");
                }
            }
        }
    },800);    
}

$.connection.transports.longPolling.supportsKeepAlive = function () {
    return false;
}
$.connection.hub.qs = "UserID=" + $('#hdfLoggedInUserID').val();
//$.connection.hub.start({ transport: ['longPolling', 'webSockets'], waitForPageLoad: false }).done(function () {
$.connection.hub.start().done(function () {
    //refreshUserNotificationCounts($('#hdfLoggedInUserID').val());
   // refreshOnlineUsers();
    refreshRecentChats();
});
$.connection.hub.disconnected(function () {
    setTimeout(function () {
        $.connection.hub.start();
    }, 5000); // Restart connection after 5 seconds.
});
$(document).ready(function () {
    $('#hdfWindowIsActiveOrNot').val('True');
    $(window).blur(function () {
        $('#hdfWindowIsActiveOrNot').val('False');
    });
    $(window).focus(function () {
        $('#hdfWindowIsActiveOrNot').val('True');
        document.title = "Chat";
    });

});
function sendResponseToRequest(userid, requestResponse, loggedInUserID) {
    chat.server.sendResponseToRequest(userid, requestResponse, loggedInUserID);
}
function sendFriendRequest(userID, loggedInUserID) {
    chat.server.sendRequest(userID, loggedInUserID);
}
function refreshUserNotificationCounts(loggedInUserID) {
    chat.server.refreshNotificationCounts(loggedInUserID);
}
function changeUserNotificationStatus(notificationID) {
    chat.server.changeNotitficationStatus(notificationID, $('#hdfLoggedInUserID').val());
}
function refreshOnlineUsers() {
    $(document).find('.online-friends').load('/User/_OnlineFriends', function () {
        var recentChats = $(document).find('.recent-chats').find('a');
        $(recentChats).each(function (cIndex, cItem) {
            changeUserOnlineStatus(cItem);
        });
        var friends = $('.user-friends');
        if (friends.length > 0) {
            var friendList = $(friends).find('li');
            console.log(friendList);
            $(friendList).each(function (cIndex, cItem) {
                changeUserOnlineStatus(cItem);
            });
        }
    });
}
function changeUserOnlineStatus(cItem) {
    $(cItem).find('img').removeClass('online-user-profile-pic');
    var userID = $(cItem).attr('data-user-id');
    var onlineItem = $(document).find('.online-friends').find('a[data-user-id="' + userID + '"]');
    if (onlineItem.length > 0) {
        $(cItem).find('img').addClass('online-user-profile-pic');
    }
}
function initiateChat(toUserID) {
    $("#dvChatwindow").html('');
    $("#dvChatwindow").load('/admin/Chat/ChatMessageWindow?UserId=' + toUserID, function () {
        //$("#divBody").animate({ "opacity": "show", top: "100" }, 500);
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        UpdateChatMessageStatus(0, currentChatUserID);
        var recentChat = $(document).find('.chtrecentusr').find('[data-user-id="' + toUserID + '"]');
        if (recentChat.length > 0) {
            $(recentChat).find('.bdgcnt').hide();
            $(recentChat).find('.bdgcnt').html("0");
            $(recentChat).find('h5').removeClass("text-primary");
            
            //var badge = $(recentChat).find('span');
           // if ($(badge).hasClass('chat-message-count') && !$(badge).hasClass('hide')) {
                //$(badge).text('');
               // $(badge).addClass('hide');
            //}
        }
    });
}
function createNewMessageBlockHtml(name, profilePicture, createOn, message, align, chatMessageID, status) {
    if (align == "right") {
        var htmll = $("#dvrightmsg").html().replace("--MSGID--", chatMessageID).replace("--MSGID--", chatMessageID).replace("--USRNAME--", name).replace("--MSG--", message).replace("--MSGDATE--", createOn);            
        return htmll;
    }
    else {
        var htmll = $("#dvleftmsg").html().replace("--MSGID--", chatMessageID).replace("--MSGID--", chatMessageID).replace("--USRNAME--", name).replace("--MSG--", message).replace("--MSGDATE--", createOn);
        return htmll;
    }
   
}
function sendChatMessage() {
    var fromUserID = $('#hdfLoggedInUserID').val();
    var fromUserName = $('#hdfLoggedInUserName').val();
    var fromUserPrifilePic = $('#hdfLoggedInUserProfilePicture').val();
    var chatMessage = $(document).find('.txt-chat-message').val();
    var toUserID = $(document).find('.hdf-current-chat-user-id').val();
    var toUserName = $(document).find('.hdf-current-chat-user-name').val();
    var toUserProfilePic = $(document).find('.hdf-current-chat-user-profile-picture').val();
    if (chatMessage != null && chatMessage != '') {
        chat.server.sendMessage(fromUserID, toUserID, chatMessage, fromUserName, fromUserPrifilePic, toUserName, toUserProfilePic);
        $(document).find('.txt-chat-message').val('');
    }
}
function createNewMessageBlock(name, profilePicture, createOn, message, align, chatMessageID, status) {
    $(document).find('.chat-body .messages').append(createNewMessageBlockHtml(name, profilePicture, createOn, message, align, chatMessageID, status));
    $(document).find("div.chat-body").animate({ scrollTop: $(document).find("div.chat-body")[0].scrollHeight }, 500);
}
function sendUserTypingStatus() {
    var toUserID = $(document).find('.hdf-current-chat-user-id').val();
    var fromUserID = $('#hdfLoggedInUserID').val();
    chat.server.sendUserTypingStatus(toUserID, fromUserID);
}
function refreshRecentChats() {
    $('.chtrecentusr').load('/admin/Chat/GetRecentChatUsers', function () {
        if ($(".chtrecentusr .list-group-item").length > 0) {
            var userID = $(".chtrecentusr .list-group-item:eq(0)").attr("data-user-id");
            initiateChat(userID);
        }
    });
}
function addChatMessageCount(currentUserId, fromUserId, fromUserName, fromUserProfilePic, toUserId, toUserName, toUserProfilePic) {
    debugger;
    var recentChatWindow = $(document).find('.chtrecentusr');
    var recentChatItem = $(recentChatWindow).find('[data-user-id="' + ((currentUserId != fromUserId) ? fromUserId : toUserId) + '"]');
    if (recentChatItem.length > 0) {
        $(recentChatItem).parent().prepend(recentChatItem);
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        if (currentUserId != fromUserId && (currentChatUserID != fromUserId)) {
            $(recentChatItem).find('h5').addClass("text-primary");
            var messageCountItem = $(recentChatItem).find('.bdgcnt');
            var count = messageCountItem.html();
            if (count == "0") {
                $(messageCountItem).html("1");
                $(messageCountItem).show();
            }
            else {
                $(messageCountItem).text(parseInt(count) + 1);
                $(messageCountItem).show();
            }
        }
    }
    else {
        var html = '';
        if (currentUserId != fromUserId) {
            var uName = fromUserName.split(' ');
            html = '<a href="javascript:;" data-user-id="' + fromUserId + '" class="list-group-item chat-user"><img src="' + fromUserProfilePic + '" class="profilePictureCircle online-user-profile-pic" />&nbsp;&nbsp;&nbsp;' + uName[0] + '<span class="custom-badge chat-message-count" data-user-id="' + fromUserId + '">1</span></a>';
        }
        else {
            var uName = toUserName.split(' ');
            html = '<a href="javascript:;" data-user-id="' + toUserId + '" class="list-group-item chat-user"><img src="' + toUserProfilePic + '" class="profilePictureCircle online-user-profile-pic" />&nbsp;&nbsp;&nbsp;' + uName[0] + '<span class="custom-badge chat-message-count hide" data-user-id="' + toUserId + '"></span></a>';
        }
        if ($('.no-recent-chats').length > 0) {
            $('.no-recent-chats').remove();
        }
        $(recentChatWindow).prepend(html);
    }
}
function UpdateChatMessageStatus(messageID, fromUserID) {
    var currentUserID = $('#hdfLoggedInUserID').val();
    chat.server.updateMessageStatus(messageID, currentUserID, fromUserID);
}
function GetOldMessages() {
    var isOldMessageExsit = $(document).find('.hdf-old-messages-exist');
    if ($(isOldMessageExsit).val() == "True") {
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        var lastMessageID = $(document).find('.hdf-last-chat-message-id').val();
        console.log($(document).find("div.right-chat-panel").scrollTop())
        $.get('/admin/Chat/GetRecentMessages?Id=' + currentChatUserID + '&lastChatMessageId=' + lastMessageID, function (messages) {
            if (messages.ChatMessages.length > 0) {
                $(isOldMessageExsit).val((messages.ChatMessages.length < 20 ? "False" : "True"));
                $(document).find('.hdf-last-chat-message-id').val(messages.LastChatMessageId);
                var html = '';
                var currentUserId = $('#hdfLoggedInUserID').val();
                var fromUserName = $('#hdfLoggedInUserName').val();
                var fromUserPrifilePic = $('#hdfLoggedInUserProfilePicture').val();
                var chatMessage = $(document).find('.txt-chat-message').val();
                var toUserID = $(document).find('.hdf-current-chat-user-id').val();
                var toUserName = $(document).find('.hdf-current-chat-user-name').val();
                var toUserProfilePic = $(document).find('.hdf-current-chat-user-profile-picture').val();
                $(messages.ChatMessages).each(function (index, item) {
                    if (item.FromUserId == currentUserId) {
                        html += createNewMessageBlockHtml(fromUserName, fromUserPrifilePic, item.MessageDate, item.Message, "right", item.ChatMeesageId, item.Status);
                    }
                    else {
                        html += createNewMessageBlockHtml(toUserName, toUserProfilePic, item.MessageDate, item.Message, "left", item.ChatMeesageId, item.Status);
                    }
                });
                var firstMsg = $('.chat-body .messages div.message-item:first');
                $(document).find('.chat-body .messages').prepend(html);
                $(document).find("div.chat-body").scrollTop(firstMsg.offset().top);
            }
            else {
                $(isOldMessageExsit).val("False");
            }
        });
    }
}
function deletemsg(msgid) {
    $(".message-item[data-chat-message-id=" + msgid + "]").remove();
    var fromUserID = $('#hdfLoggedInUserID').val();
    chat.server.deletemsg(fromUserID, msgid);
}
Date.prototype.customFormat = function (formatString) {

    var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
    var dateObject = this;
    if (dateObject != "Invalid Date") {
        YY = ((YYYY = dateObject.getFullYear()) + "").slice(-2);
        MM = (M = dateObject.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = dateObject.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][dateObject.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);

        h = (hhh = dateObject.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = hhh < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = dateObject.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = dateObject.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    }
}