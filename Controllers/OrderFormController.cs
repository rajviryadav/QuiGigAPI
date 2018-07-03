using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using QuiGig.Models;
using QuiGigAPI.Common;
using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using QuiGigAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    public class OrderFormController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        [HttpPost]
        [Route("api/GetSectionByServiceId")]
        public IHttpActionResult GetSectionByServiceId(SectionViewMOdel sectionViewModel)
        {
            bool success = false;
            var messsage = "";
            var model = new List<OrderModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                var er = errors.Errors[0].ErrorMessage;
                return Ok(new
                {
                    Success = false,
                    Message = er,
                    RUrl = "errorpage"
                });
            }
            else
            {
                try
                {
                    var query = context.OrderForms.Where(x => x.ServiceID == sectionViewModel.ServiceId && x.IsDelete == false && x.IsActive == true);
                    model = query.Select(x => new OrderModel
                    {
                        ServiceID = x.ServiceID,
                        Section = x.ID,
                        SecctionTitle = x.SectionTitle,
                        ServiceName = x.Service.ServiceName,
                        QuestionList = x.OrderFormQuestions.Where(q => q.IsDelete == false && q.IsActive == true).Select(y => new OrderQuestionModel
                        {
                            QuestionID = y.QuestionMaster.ID,
                            QuestionTitle = y.QuestionMaster.QuestionTitle,
                            QuestionImage = y.QuestionMaster.Image,
                            IsRequired = y.QuestionMaster.IsRequired,
                            QuestionTypeID = y.QuestionMaster.QuestionTypeID,
                            QuestionType_UniqueCode = y.QuestionMaster.QuestionType.UniqueCode,
                            LinerScaleLabel = y.QuestionMaster.LinerScaleLabel,
                            QuestionOptionList = y.QuestionMaster.QuestionOptions.Where(z => z.IsDelete == false && z.IsActive == true).Select(z => new QuestionOptionModel
                            {
                                OptionImage = z.Image,
                                OptionHeading = z.OptionHeading,
                                OptionID = z.ID,
                                Placeholder = z.Placeholder,
                                IsOther = z.IsOther
                            }).ToList(),
                            FileUploadList = y.QuestionMaster.FileUploadSettings.Where(p => x.IsDelete == false && p.IsActive == true).Select(p => new FileUploadModel
                            {
                                FileSize = p.AllowFileMaxSize.FileSize,
                                AllowedFileNumber = p.AllowFileMaxNumber.Number,
                                AllowedFileType = p.FileTypeID
                            }).ToList()
                        }).ToList()
                    }).ToList();
                    success = true;
                    messsage = "Get List";
                }
                catch (Exception ex)
                {
                    messsage = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/CheckValidZipCode")]
        public IHttpActionResult CheckValidZipCode(CheckValidZipCodeViewMOdel model)
        {
            bool success = false;
            var messsage = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                var er = errors.Errors[0].ErrorMessage;
                return Ok(new
                {
                    Success = false,
                    Message = er
                });
            }
            else
            {
                try
                {
                    var uCount = context.CityZipCodes.Where(x => x.ZipCode == model.ZipCode && x.LocationMaster.City == model.City).FirstOrDefault();
                    if (uCount != null)
                    {
                        success = true;
                        messsage = "Valid";
                    }
                    else
                    {
                        success = false;
                        messsage = "Invalid ZipCode";
                    }
                }
                catch (Exception ex)
                {
                    messsage = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }

        [HttpPost]
        [Route("api/NoServiceFound")]
        public IHttpActionResult NoServiceFound(NoServiceFoundModel model)
        {
            bool success = false;
            var messsage = "";
            try
            {
                string emailBody = CommonLib.GetEmailTemplateValue("NoServiceFound/Body");
                string emailSubject = CommonLib.GetEmailTemplateValue("NoServiceFound/Subject");
                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                string adminEmail = ConfigurationManager.AppSettings["NoSerivceEmail"].ToString();
                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                emailBody = emailBody.Replace("@@@Path", path);
                emailBody = emailBody.Replace("@@@Name", model.Name);
                emailBody = emailBody.Replace("@@@Email", model.Email);
                emailBody = emailBody.Replace("@@@City", model.City);
                emailBody = emailBody.Replace("@@@Service", model.Service);
                CommonLib.SendMail(strFromEmailAddress, adminEmail, emailSubject, emailBody);
                success = true;
                messsage = "Your request has been sent to admin, Our team contact with you.";
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
            });
        }

        [HttpPost]
        [Route("api/UploadOrderFormFile")]
        public IHttpActionResult UploadOrderFormFile()
        {
            bool success = false;
            var messsage = "";
            var model = new OrderFormFileUploadModel();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        var pathExist = HttpContext.Current.Server.MapPath("~/Content/images/OrderFormUpload/");
                        if (!Directory.Exists(pathExist))
                            Directory.CreateDirectory(pathExist);

                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            var filePath = pathExist + DateTime.Now.Ticks.ToString() + postedFile.FileName + extension;
                            postedFile.SaveAs(filePath);
                            model.Path = "/Content/images/OrderFormUpload/" + DateTime.Now.Ticks.ToString() + postedFile.FileName + extension;
                            model.OriginalName = postedFile.FileName + extension;
                            model.NewName = DateTime.Now.Ticks.ToString() + postedFile.FileName + extension;
                            model.AttachmentExt = extension;
                            model.AttachmentType = postedFile.ContentType;
                            model.AttachmentSize = postedFile.ContentLength;
                            success = true;
                            messsage = "Upload Successfully.";
                        }
                    }
                }
                else
                {
                    success = false;
                    messsage = "Please Upload a file.";
                }
            }
            catch (Exception ex)
            {
                success = false;
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/SaveOrderForm")]
        public IHttpActionResult SaveOrderForm(ServiceModel model)
        {
            bool success = false;
            var messsage = "";
            long jobId = 0;
            var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userExist = manager.FindById(model.UserId);
            if (userExist != null)
            {
                if (userExist.EmailConfirmed)
                {
                    var userDetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                    var userCurrentPlan = context.UserPlans.Where(x => x.UserID == model.UserId).FirstOrDefault();
                    var featurePostOrder = context.Features.Where(x => x.UniqueCode == "POST_ORDER").FirstOrDefault();
                    var featureVal = featurePostOrder.PlanDurationFeatures.FirstOrDefault();
                    if (userDetail.BidPoints >= Convert.ToInt32(featureVal.FeatureValue))
                    {
                        Util.SaveUserWallet(context, model.UserId, 0, "Post Order", 0, Convert.ToInt32(featureVal.FeatureValue), 0, PaymentStatus.Completed.ToString(), PaymentFromSite.QuiGig.ToString(), 0, 0, "Post Gig");
                        try
                        {
                            if (model.OptionList.Count() > 0)
                            {
                                #region Save Jobs
                                var parmSett = context.ParameterSettings.Where(x => x.UniqueName == ParameterUniqueNameEnum.BID_TIMER.ToString() && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                                var serviceName = context.Services.Where(x => x.ID == model.ServiceID && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                                Job job = new Job();
                                job.ServiceID = model.ServiceID;
                                if (!string.IsNullOrEmpty(model.UserId))
                                    job.UserID = model.UserId;
                                job.JobTitle = serviceName.ServiceName;
                                job.JobStatus = 1;
                                if (parmSett != null)
                                {
                                    int totalTime = Convert.ToInt32(parmSett.Value) + 5;
                                    if (parmSett.ValueType == ValueTypeEnum.Hourly.ToString())
                                        job.JobExpireDate = DateTime.UtcNow.AddHours(totalTime).AddMinutes(30);
                                    if (parmSett.ValueType == ValueTypeEnum.Monthly.ToString())
                                        job.JobExpireDate = DateTime.UtcNow.AddMonths(Convert.ToInt32(parmSett.Value));
                                    if (parmSett.ValueType == ValueTypeEnum.Yearly.ToString())
                                        job.JobExpireDate = DateTime.UtcNow.AddYears(Convert.ToInt32(parmSett.Value));
                                    if (parmSett.ValueType == ValueTypeEnum.Days.ToString())
                                        job.JobExpireDate = DateTime.UtcNow.AddDays(Convert.ToInt32(parmSett.Value));
                                }
                                job.IsActive = true;
                                job.IsDelete = false;
                                job.IsPublish = false;
                                job.CreatedDate = DateTime.UtcNow;
                                job.UpdatedDate = DateTime.UtcNow;
                                job.StateName = model.StateName;
                                job.CityName = model.CityName;
                                context.Jobs.Add(job);
                                context.SaveChanges();
                                #endregion
                                #region Save Options
                                foreach (var optionList in model.OptionList)
                                {
                                    JobQuestionOption jobQuesOption = new JobQuestionOption();
                                    jobQuesOption.OptionID = optionList.OptionID;
                                    jobQuesOption.OtherAnswer = optionList.OtherAnswer;
                                    jobQuesOption.JobID = job.ID;
                                    jobQuesOption.CreatedDate = DateTime.UtcNow;
                                    context.JobQuestionOptions.Add(jobQuesOption);
                                    context.SaveChanges();
                                    if (optionList.FileUploadList != null)
                                    {
                                        foreach (var fileList in optionList.FileUploadList)
                                        {
                                            JobOptionAttachment jobAttachment = new JobOptionAttachment();
                                            jobAttachment.JobQuestionOptionsID = jobQuesOption.ID;
                                            jobAttachment.OriginalName = fileList.OriginalName;
                                            jobAttachment.NewName = fileList.NewName;
                                            jobAttachment.Path = fileList.Path;
                                            jobAttachment.Description = fileList.Description;
                                            jobAttachment.AttachmentExt = fileList.AttachmentExt;
                                            jobAttachment.AttachmentType = fileList.AttachmentType;
                                            jobAttachment.AttachmentSize = fileList.AttachmentSize;
                                            jobAttachment.CreatedDate = DateTime.UtcNow;
                                            context.JobOptionAttachments.Add(jobAttachment);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                #endregion
                                #region Save Activity and Email send

                                Util.SaveActivity(context, "New Gigs: " + job.JobTitle, model.UserId, model.UserId, ActivityType.PostedJob.ToString(), job.ID, "Customer", true, false);

                                var userService = context.UserServices.Where(x => x.ServiceID == model.ServiceID).ToList();

                                foreach (var activity in userService)
                                {
                                    if (model.UserId != activity.UserID)
                                    {
                                        if (!string.IsNullOrEmpty(model.CityName) && !string.IsNullOrEmpty(model.StateName))
                                        {
                                            var userAddress = context.UserAddresses.Where(x => x.City.ToLower() == model.CityName.ToLower() && x.State.ToLower() == model.StateName.ToLower() && x.UserID == activity.UserID).FirstOrDefault();
                                            if (userAddress != null)
                                            {
                                                var toUserExist = UserManager.FindById(activity.UserID);
                                                if (toUserExist.EmailConfirmed)
                                                {
                                                    //check sp push notification setting
                                                    var isPushNotify = Util.GetNotificationValue(NotificationEnum.MATCHING_ORDERS.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), activity.UserID, context);

                                                    Util.SaveActivity(context, "New Gig: " + job.JobTitle, model.UserId, activity.UserID, ActivityType.PostedJob.ToString(), job.ID, "ServiceProvider", isPushNotify, false);
                                                    //check sp email notification setting
                                                    bool isEmailNotify = Util.GetNotificationValue(NotificationEnum.MATCHING_ORDERS.ToString(), NotificationCategoryEnum.EMAIL_NOTIFICATION.ToString(), activity.UserID, context);
                                                    if (isEmailNotify)
                                                    {
                                                        var fromUserExist = UserManager.FindById(model.UserId);

                                                        if (fromUserExist != null && toUserExist != null)
                                                        {
                                                            var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                                            var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                                            #region Email Sending Code
                                                            try
                                                            {
                                                                string emailBody = CommonLib.GetEmailTemplateValue("PostNewJob/Body");
                                                                string emailSubject = "QuiGig - New matching order";
                                                                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                                                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                                                emailBody = emailBody.Replace("@@@Path", path);
                                                                emailBody = emailBody.Replace("@@@JobTitle", job.JobTitle);
                                                                emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                                                emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                                                CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                messsage = ex.Message;
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion

                                jobId = job.ID;
                                success = true;
                                messsage = "Save successfully.";
                            }
                        }
                        catch (Exception ex)
                        {
                            messsage = ex.Message;
                        }
                    }
                    else
                    {
                        #region Save Jobs
                        var parmSett = context.ParameterSettings.Where(x => x.UniqueName == ParameterUniqueNameEnum.BID_TIMER.ToString() && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                        var serviceName = context.Services.Where(x => x.ID == model.ServiceID && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                        Job job = new Job();
                        job.ServiceID = model.ServiceID;
                        if (!string.IsNullOrEmpty(model.UserId))
                            job.UserID = model.UserId;
                        job.JobTitle = serviceName.ServiceName;
                        job.JobStatus = 1;
                        if (parmSett != null)
                        {
                            int totalTime = Convert.ToInt32(parmSett.Value) + 5;
                            if (parmSett.ValueType == ValueTypeEnum.Hourly.ToString())
                                job.JobExpireDate = DateTime.UtcNow.AddHours(totalTime).AddMinutes(30);
                            if (parmSett.ValueType == ValueTypeEnum.Monthly.ToString())
                                job.JobExpireDate = DateTime.UtcNow.AddMonths(Convert.ToInt32(parmSett.Value));
                            if (parmSett.ValueType == ValueTypeEnum.Yearly.ToString())
                                job.JobExpireDate = DateTime.UtcNow.AddYears(Convert.ToInt32(parmSett.Value));
                            if (parmSett.ValueType == ValueTypeEnum.Days.ToString())
                                job.JobExpireDate = DateTime.UtcNow.AddDays(Convert.ToInt32(parmSett.Value));
                        }
                        job.IsActive = false;
                        job.IsDelete = false;
                        job.IsPublish = false;
                        job.CreatedDate = DateTime.UtcNow;
                        job.UpdatedDate = DateTime.UtcNow;
                        job.StateName = model.StateName;
                        job.CityName = model.CityName;
                        context.Jobs.Add(job);
                        context.SaveChanges();
                        #endregion
                        #region Save Options
                        foreach (var optionList in model.OptionList)
                        {
                            JobQuestionOption jobQuesOption = new JobQuestionOption();
                            jobQuesOption.OptionID = optionList.OptionID;
                            jobQuesOption.OtherAnswer = optionList.OtherAnswer;
                            jobQuesOption.JobID = job.ID;
                            jobQuesOption.CreatedDate = DateTime.UtcNow;
                            context.JobQuestionOptions.Add(jobQuesOption);
                            context.SaveChanges();
                            if (optionList.FileUploadList != null)
                            {
                                foreach (var fileList in optionList.FileUploadList)
                                {
                                    JobOptionAttachment jobAttachment = new JobOptionAttachment();
                                    jobAttachment.JobQuestionOptionsID = jobQuesOption.ID;
                                    jobAttachment.OriginalName = fileList.OriginalName;
                                    jobAttachment.NewName = fileList.NewName;
                                    jobAttachment.Path = fileList.Path;
                                    jobAttachment.Description = fileList.Description;
                                    jobAttachment.AttachmentExt = fileList.AttachmentExt;
                                    jobAttachment.AttachmentType = fileList.AttachmentType;
                                    jobAttachment.AttachmentSize = fileList.AttachmentSize;
                                    jobAttachment.CreatedDate = DateTime.UtcNow;
                                    context.JobOptionAttachments.Add(jobAttachment);
                                    context.SaveChanges();
                                }
                            }
                        }
                        #endregion
                        jobId = job.ID;
                        success = false;
                        messsage = "You have not sufficent coin for post and order.";
                    }
                }
            }
            else
            {
                messsage = "Your email is not confirmed";
            }
            return Ok(new
            {
                Success = success,
                Message = messsage,
                JobId = jobId
            });
        }

        [HttpPost]
        [Route("api/UpdateJob")]
        public IHttpActionResult UpdateJob(UpdateJobModel model)
        {
            bool success = false;
            var messsage = "";
            var userDetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
            var featurePostOrder = context.Features.Where(x => x.UniqueCode == "POST_ORDER").FirstOrDefault();
            var featureVal = featurePostOrder.PlanDurationFeatures.FirstOrDefault();
            try
            {
                if (userDetail.BidPoints >= Convert.ToInt32(featureVal.FeatureValue))
                {
                    Util.SaveUserWallet(context, model.UserId, 0, "Post Order", 0, Convert.ToInt32(featureVal.FeatureValue), 0, PaymentStatus.Completed.ToString(), PaymentFromSite.QuiGig.ToString(), 0, 0, "Post Gig");
                    var jobDetail = context.Jobs.Where(x => x.ID == model.JobId && x.UserID == model.UserId).FirstOrDefault();
                    if (jobDetail != null)
                    {
                        jobDetail.IsActive = true;
                        jobDetail.IsDelete = false;
                        jobDetail.IsPublish = true;
                        context.SaveChanges();
                        success = true;
                        messsage = "Save successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                messsage = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = messsage
            });
        }
    }
}
