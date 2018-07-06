using QuiGigAPI.Common;
using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using QuiGigAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    public class NotificationController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();

        [Route("api/GetUserNotificationById")]
        [HttpPost]
        public IHttpActionResult GetUserNotificationById(UserViewModel user)
        {
            var model = new List<NotificationCatModel>();
            var message = "";
            bool status = false;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.UserNotifications.Where(x => x.UserId == user.UserId && x.IsActive == true && x.IsDelete == false).GroupBy(x => x.Notification.NotificationCatID).Select(x => new NotificationCatModel
                    {
                        CategoryName = x.FirstOrDefault().Notification.NotificationCategory.CategoryName,
                        CatId = x.FirstOrDefault().Notification.NotificationCatID,
                        UserNotification = x.Select(y => new UserNotificationModel
                        {
                            UserNotificationId = y.ID,
                            Name = y.Notification.Name,
                            Description = y.Notification.Description,
                            NotificationCatID = y.NotificationID,
                            UniqueCode = y.Notification.UniqueCode,
                            DefaultValue = y.NotificationValue,
                            IsServiceProvider = y.Notification.IsServiceProvider,
                            IsCustomer = y.Notification.IsCustomer
                        }).ToList()
                    }).ToList();
                    message = "Get detail";
                    status = true;
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model
            });
        }

        [Route("api/ChangeNotification")]
        [HttpPost]
        public IHttpActionResult ChangeNotification(ChangeNotificationModel model)
        {
            bool status = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.UserNotifications.Where(x => x.ID == model.ID && x.UserId == model.UserId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (query != null)
                    {
                        query.NotificationValue = model.DefaultValue;
                        query.UpdatedDate = DateTime.UtcNow;
                        query.UpdatedByID = model.UserId;
                        context.SaveChanges();
                        status = true;
                        message = "Update Successfully.";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message
            });
        }

        [Route("api/MarkNotificationRead")]
        [HttpPost]
        public IHttpActionResult MarkNotificationRead(DeleteDetailViewModel model)
        {
            bool status = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Activities.Where(x => x.ToID == model.UserId && x.ID == model.ID && x.IsActive == true && x.IsDelete == false && x.IsNotification == true && x.IsRead == false);
                    foreach (var entity in query)
                    {
                        entity.IsRead = true;
                    }
                    context.SaveChanges();
                    status = true;
                    message = "Read Successfully.";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message
            });
        }

        [Route("api/DeactiveAccount")]
        [HttpPost]
        public IHttpActionResult DeactiveAccount(DeactiveAccountModel model)
        {
            bool status = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    DeactivatedAccount entity = new DeactivatedAccount();
                    entity.Reason = model.Reason;
                    entity.UserId = model.UserId;
                    entity.CreatedDate = DateTime.UtcNow;
                    context.DeactivatedAccounts.Add(entity);

                    var userDdetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                    userDdetail.IsActive = false;
                    userDdetail.IsDelete = true;

                    context.SaveChanges();
                    status = true;
                    message = "Account deactivated successfully.";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message
            });
        }

        [Route("api/GetAllNotificationList")]
        [HttpPost]
        public IHttpActionResult GetAllNotificationList(NotificationByTypeModel user)
        {
            bool status = false;
            var model = new List<NotificationListModel>();
            int gigsCount = 0;
            int bidsCount = 0;
            int allCount = 0;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    if (user.NotificationType == 1)
                    {
                        var list = context.Activities.Where(x => x.ToID == user.UserId && x.ActivityType == "PostedJob" && x.IsActive == true && x.IsDelete == false && x.IsNotification == true).OrderByDescending(x => x.ID).ToList();
                        model = list.Select(x => new NotificationListModel
                        {
                            ID = x.ID,
                            UserName = x.AspNetUser.UserDetails2.Select(y => y.FirstName).FirstOrDefault(),
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            UserImage = x.AspNetUser.UserDetails2.Select(y => y.ProfilePic).FirstOrDefault(),
                            Message = x.Message,
                            JobId = x.JobId,
                            IsRead = x.IsRead
                        }).ToList();
                    }
                    else if (user.NotificationType == 2)
                    {
                        var list = context.Activities.Where(x => x.ToID == user.UserId && x.ActivityType == "Hire" && x.IsActive == true && x.IsDelete == false && x.IsNotification == true).OrderByDescending(x => x.ID).ToList();
                        model = list.Select(x => new NotificationListModel
                        {
                            ID = x.ID,
                            UserName = x.AspNetUser.UserDetails2.Select(y => y.FirstName).FirstOrDefault(),
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            UserImage = x.AspNetUser.UserDetails2.Select(y => y.ProfilePic).FirstOrDefault(),
                            Message = x.Message,
                            JobId = x.JobId,
                            IsRead = x.IsRead
                        }).ToList();
                    }
                    else
                    {
                        var list = context.Activities.Where(x => x.ToID == user.UserId && x.IsActive == true && x.IsDelete == false && x.IsNotification == true).OrderByDescending(x => x.ID).ToList();
                        model = list.Select(x => new NotificationListModel
                        {
                            ID = x.ID,
                            UserName = x.AspNetUser.UserDetails2.Select(y => y.FirstName).FirstOrDefault(),
                            CreatedDate = x.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                            UserImage = x.AspNetUser.UserDetails2.Select(y => y.ProfilePic).FirstOrDefault(),
                            Message = x.Message,
                            JobId = x.JobId,
                            IsRead = x.IsRead
                        }).ToList();
                    }
                    gigsCount = context.Activities.Where(x => x.ToID == user.UserId && x.ActivityType == "PostedJob" && x.IsActive == true && x.IsDelete == false && x.IsRead == false).Count();
                    bidsCount = context.Activities.Where(x => x.ToID == user.UserId && x.ActivityType == "Hire" && x.IsActive == true && x.IsDelete == false && x.IsRead == false).Count();
                    allCount = context.Activities.Where(x => x.ToID == user.UserId && x.IsActive == true && x.IsDelete == false &&  x.IsRead == false).Count();
                    status = true;
                    message = "Get List.";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Message = message,
                Result = model,
                GigsCount = gigsCount,
                BidsCount = bidsCount,
                AllCount = allCount
            });
        }     

        [Route("api/GetNewNotification")]
        [HttpGet]
        public IHttpActionResult GetNewNotification(UserViewModel user)
        {
            bool status = false;
            var UnReadCount = 0;
            var message = "";
            var model = new List<NewNotificationModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.Activities.Where(x => x.ToID == user.UserId && x.IsActive == true && x.IsDelete == false && x.IsNotification == true).OrderByDescending(x => x.ID).Select(x => new NewNotificationModel
                    {
                        Date = x.UpdatedDate,
                        Message = x.Message,
                        UserImage = x.AspNetUser.UserDetails2.Select(y => y.ProfilePic).FirstOrDefault()
                    }).Take(5).ToList();
                    UnReadCount = context.Activities.Where(x => x.ToID == user.UserId && x.IsActive == true && x.IsDelete == false && x.IsNotification == true && x.IsRead == false).Count();
                    status = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    status = false;
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = status,
                Result = model,
                Message = message,
                UnReadCount = UnReadCount
            });
        }
    }
}
