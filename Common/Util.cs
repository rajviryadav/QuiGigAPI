using QuiGigAPI.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QuiGigAPI.Common
{
    public class Util
    {
        public static string GetRoleIdByName(string roleName, QuiGigAPIEntities context)
        {
            var roleId = "";
            var role = context.AspNetRoles.FirstOrDefault(x => x.Name == roleName);
            if (role != null)
                roleId = role.Id;
            return roleId;
        }
        public static string GetRoleNameById(string roleId, QuiGigAPIEntities context)
        {
            var roleName = "";
            var role = context.AspNetRoles.FirstOrDefault(x => x.Id == roleId);
            if (role != null)
                roleName = role.Name;
            return roleName;
        }
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public static void SaveActivityProposal(QuiGigAPIEntities context, string message, string fromUserId, string toUserId, string activityType, long jobId, string userType, bool isNotification, bool isRead, long proposalId)
        {
            Activity activity = new Activity();
            activity.Message = message;
            activity.FromID = fromUserId;
            activity.ToID = toUserId;
            activity.ActivityType = activityType;
            activity.ProposalId = proposalId;
            activity.JobId = jobId;
            activity.IsNotification = isNotification;
            if (activityType == ProfileParameterEnum.SIGNUP.ToString())
                activity.RedirectPath = "#";
            else if (proposalId > 0)
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId + "&param2=" + proposalId;
            else
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId;
            activity.IsRead = false;
            activity.IsActive = true;
            activity.IsDelete = false;
            activity.CreatedDate = DateTime.UtcNow;
            activity.UpdatedDate = DateTime.UtcNow;
            context.Activities.Add(activity);
            context.SaveChanges();
        }
        public static void SaveActivityProposalNew(QuiGigAPIEntities context, string message, string fromUserId, string toUserId, string activityType, long jobId, string userType, bool isNotification, bool isRead, long proposalId)
        {
            Activity activity = new Activity();
            activity.Message = message;
            activity.FromID = fromUserId;
            activity.ToID = toUserId;
            activity.ActivityType = activityType;
            activity.IsNotification = isNotification;
            if (activityType == ProfileParameterEnum.SIGNUP.ToString())
                activity.RedirectPath = "#";
            activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId;
            activity.JobId = jobId;
            activity.ProposalId = proposalId;
            activity.IsRead = false;
            activity.IsActive = true;
            activity.IsDelete = false;
            activity.CreatedDate = DateTime.UtcNow;
            activity.UpdatedDate = DateTime.UtcNow;
            context.Activities.Add(activity);
            context.SaveChanges();
        }
        public static long SaveActivity(QuiGigAPIEntities context, string message, string fromUserId, string toUserId, string activityType, long jobId, string userType, bool isNotification, bool isRead)
        {
            Activity activity = new Activity();
            activity.Message = message;
            activity.FromID = fromUserId;
            activity.ToID = toUserId;
            activity.ActivityType = activityType;
            activity.IsNotification = isNotification;
            if (activityType == ProfileParameterEnum.SIGNUP.ToString())
                activity.RedirectPath = "#";
            else
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId;
            activity.JobId = jobId;
            activity.IsRead = false;
            activity.IsActive = true;
            activity.IsDelete = false;
            activity.CreatedDate = DateTime.UtcNow;
            activity.UpdatedDate = DateTime.UtcNow;
            context.Activities.Add(activity);
            context.SaveChanges();
            return activity.ID;
        }
        public static long SaveActivitySignUp(QuiGigAPIEntities context, string message, string fromUserId, string toUserId, string activityType, long jobId, string userType, bool isNotification, bool isRead)
        {
            Activity activity = new Activity();
            activity.Message = message;
            activity.FromID = fromUserId;
            activity.ToID = toUserId;
            activity.ActivityType = activityType;
            activity.IsNotification = isNotification;
            if (activityType == ProfileParameterEnum.SIGNUP.ToString())
                activity.RedirectPath = "#";
            else
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId;         
            activity.IsRead = false;
            activity.IsActive = true;
            activity.IsDelete = false;
            activity.CreatedDate = DateTime.UtcNow;
            activity.UpdatedDate = DateTime.UtcNow;
            context.Activities.Add(activity);
            context.SaveChanges();
            return activity.ID;
        }
        public static void SaveActivityHired(QuiGigAPIEntities context, string message, string fromUserId, string toUserId, string activityType, long jobId, string userType, bool isNotification, bool isRead, long proposalId, long hiredId)
        {
            Activity activity = new Activity();
            activity.Message = message;
            activity.FromID = fromUserId;
            activity.ToID = toUserId;
            activity.ActivityType = activityType;
            activity.IsNotification = isNotification;
            if (activityType == ProfileParameterEnum.SIGNUP.ToString())
                activity.RedirectPath = "#";
            else if (proposalId > 0)
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId + "&param2=" + proposalId;
            else
                activity.RedirectPath = "/order-detail?key=" + Encrypt(userType) + "&param1=" + jobId;
            activity.JobId = jobId;
            activity.ProposalId = proposalId;
            activity.HiredId = hiredId;
            activity.IsRead = false;
            activity.IsActive = true;
            activity.IsDelete = false;
            activity.CreatedDate = DateTime.UtcNow;
            activity.UpdatedDate = DateTime.UtcNow;
            context.Activities.Add(activity);
            context.SaveChanges();
        }
        public static bool GetNotificationValue(string notificationType, string notificationCat, string userId, QuiGigAPIEntities context)
        {
            bool notificationVal = false;
            try
            {
                var userNotify = context.UserNotifications.Where(x =>
                x.Notification.UniqueCode == notificationType
                && x.UserId == userId
                && x.Notification.NotificationCategory.UniqueName == notificationCat
                && x.IsActive == true && x.Notification.IsActive == true
                && x.Notification.IsDelete == false
                && x.Notification.NotificationCategory.IsActive == true
                && x.Notification.NotificationCategory.IsDelete == false
                && x.IsDelete == false
                ).FirstOrDefault();
                if (userNotify != null)
                    notificationVal = userNotify.NotificationValue;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return notificationVal;
        }
        public static string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);
            if (timeSpan <= TimeSpan.FromSeconds(60))
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            else if (timeSpan <= TimeSpan.FromMinutes(60))
                result = timeSpan.Minutes > 1 ? string.Format("{0} minutes ago", timeSpan.Minutes) : timeSpan.Minutes + " minute ago";
            else if (timeSpan <= TimeSpan.FromHours(24))
                result = timeSpan.Hours > 1 ? string.Format("{0} hours ago", timeSpan.Hours) : timeSpan.Hours + " hour ago";
            else if (timeSpan <= TimeSpan.FromDays(30))
                result = timeSpan.Days > 1 ? string.Format("{0} days ago", timeSpan.Days) : timeSpan.Days + " days ago";
            else if (timeSpan <= TimeSpan.FromDays(365))
                result = timeSpan.Days > 30 ? string.Format("{0} months ago", timeSpan.Days / 30) : timeSpan.Days / 30 + " month ago";
            else
                result = timeSpan.Days > 365 ? string.Format("{0} years ago", timeSpan.Days / 365) : timeSpan.Days / 365 + " year ago";
            return result;
        }
        public static long SaveUserPayment(QuiGigAPIEntities context, string userId, string provider, long planID, decimal durationAmount, string description, string token, string transactionId, long packgId, string paymentFrom, string paymentStatus)
        {
            long id = 0;
            try
            {
                var providerDetail = context.PaymentSubscriptions.Where(x => x.Provider.ToLower() == provider.ToLower() && x.IsActive == true).FirstOrDefault();
                if (providerDetail != null)
                {
                    UserPayment entity = new UserPayment();
                    entity.ProviderId = providerDetail.ID;
                    entity.UserID = userId;
                    if (planID > 0)
                        entity.UserPlanId = planID;
                    entity.Token = token;
                    entity.Amount = durationAmount;
                    entity.PaymentStatus = paymentStatus;
                    entity.PaymentFrom = paymentFrom;
                    if (!string.IsNullOrEmpty(transactionId))
                        entity.TransactionId = transactionId;
                    entity.CreatedDate = DateTime.UtcNow;
                    entity.Description = description;
                    if (packgId > 0)
                        entity.PackageId = packgId;
                    context.UserPayments.Add(entity);
                    context.SaveChanges();
                    id = entity.ID;
                }
            }
            catch (Exception ex)
            {
            }
            return id;
        }
        public static void SaveUserWallet(QuiGigAPIEntities context, string userId, long userPaymentId, string description, int creditAmount, int debitAmount, long packgId, string paymentStatus, string paymentFrom, decimal payAmount, int bonusCoin, string paymentGateway)
        {
            try
            {
                UserWallet entity = new UserWallet();
                entity.UserID = userId;
                entity.UserPaymentId = userPaymentId;
                entity.Description = description;
                entity.CreditAmount = creditAmount;
                entity.DebitAmount = debitAmount;
                entity.CreatedDate = DateTime.UtcNow;
                entity.PaymentFrom = paymentFrom;
                entity.PaymentStatus = paymentStatus;
                entity.PayAmount = payAmount;
                entity.BonusCoin = bonusCoin;
                if (packgId > 0)
                    entity.PackageId = packgId;
                entity.PaymentGateway = paymentGateway;
                context.UserWallets.Add(entity);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
    }
}