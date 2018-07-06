using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class NotificationCatModel
    {
        public string CategoryName { get; set; }
        public string UniqueName { get; set; }
        public long CatId { get; set; }
        public List<UserNotificationModel> UserNotification { get; set; }
    }

    public class UserNotificationModel
    {
        public long UserNotificationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long NotificationCatID { get; set; }
        public string UniqueCode { get; set; }
        public bool DefaultValue { get; set; }
        public bool IsServiceProvider { get; set; }
        public bool IsCustomer { get; set; }
    }
    public class NotificationListModel
    {
        public long ID { get; set; }
        public long? JobId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string UserImage { get; set; }
        public string CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
    public class NewNotificationModel
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string UserImage { get; set; }
    }
    public class ChangeNotificationModel
    {
        [Required(ErrorMessage = "Please enter ID")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter DefaultValue")]
        public bool DefaultValue { get; set; }
    }
    public class DeactiveAccountModel
    {
        [Required(ErrorMessage = "Please enter Reason")]
        public string Reason { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }

    public class NotificationByTypeModel
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter Notification Type")]
        public int NotificationType { get; set; }
    }
}