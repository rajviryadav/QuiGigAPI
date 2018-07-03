using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class MessageListModel
    {
        [Required(ErrorMessage = "Please enter GroupId")]
        public long GroupId { get; set; }
        [Required(ErrorMessage = "Please enter JobID")]
        public long JobID { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter MessageText")]
        public string MessageText { get; set; }
        [Required(ErrorMessage = "Please enter ToID")]
        public string ToID { get; set; }
        [Required(ErrorMessage = "Please enter IsRead")]
        public bool IsRead { get; set; }
    }
    public class ReportModel
    {
        [Required(ErrorMessage = "Please enter ReportForId")]
        public string ReportForId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter Message")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class BlockUserModel
    {
        [Required(ErrorMessage = "Please enter BlockUserID")]
        public string BlockUserID { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter GroupId")]
        public long GroupId { get; set; }
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class UnBlockUserModel
    {
        [Required(ErrorMessage = "Please enter BlockUserID")]
        public string BlockUserID { get; set; }
        [Required(ErrorMessage = "Please enter GroupId")]
        public long GroupId { get; set; }
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class StartChatWithSPViewModel
    {
        [Required(ErrorMessage = "Please enter ToUserId")]
        public string ToUserId { get; set; }
        [Required(ErrorMessage = "Please enter FromUserId")]
        public string FromUserId { get; set; }
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class StartChatWithCustomerViewModel
    {
        [Required(ErrorMessage = "Please enter FromUserId")]
        public string FromUserId { get; set; }
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class StartChatWithSPModel
    {
        public string JobTitle { get; set; }
        public long JobID { get; set; }
        public string FromUserName { get; set; }
        public string FromUserID { get; set; }
        public long GroupId { get; set; }
        public string HiredStatus { get; set; }
        public bool? IsBlock { get; set; }
        public string BlockByID { get; set; }
        public string JobCompleted { get; set; }
        public string BlockBy { get; set; }
        public List<UserMessageListModel> UserMessageList { get; set; }
    }
    public class UserMessageListModel
    {
        public string FromUserID { get; set; }
        public string MessageText { get; set; }
        public string ToUserID { get; set; }
        public string ToUserImage { get; set; }
        public string FromUserImage { get; set; }
        public string FromUserShortName { get; set; }
        public string ToUserShortName { get; set; }
    }
    public class StartChatWithCustomerModel
    {
        public long GroupId { get; set; }
        public string JobTitle { get; set; }
        public string ToUserName { get; set; }
        public string ToUserID { get; set; }
        public string ToUserImage { get; set; }
        public string FromUserName { get; set; }
        public string FromUserID { get; set; }
        public string BlockBy { get; set; }
        public bool? IsBlock { get; set; }
        public string BlockByID { get; set; }
        public string JobCompleted { get; set; }
        public List<UserMessageListModel> UserMessageList { get; set; }
    }
    public class MessageNotificationModel
    {
        public string FromUserName { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public string FromUserID { get; set; }
        public long JobID { get; set; }
        public DateTime Date { get; set; }
    }
    public class ConnectedUserModel
    {
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string UserID { get; set; }
        public string JobTitle { get; set; }
        public long JobID { get; set; }
    }
}