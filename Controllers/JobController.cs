using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuiGigAPI.Common;
using QuiGigAPI.DataBase;
using QuiGigAPI.Models;
using QuiGigAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace QuiGigAPI.Controllers
{
    public class JobController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();
        private ApplicationUserManager _userManager;
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

        [HttpPost]
        [Route("api/GetMatchingJobById")]
        public IHttpActionResult GetMatchingJobById(JobModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new MatchingOrderModel();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Jobs.Where(x => x.ID == viewModel.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();                   
                    model.ID = query.ID;
                    model.JobTitle = query.JobTitle;
                    model.CreatedDate = Convert.ToDateTime(query.UpdatedDate).ToString("MM/dd/yyyy HH:mm:ss");
                    model.JobExpireDate = Convert.ToDateTime(query.JobExpireDate).ToString("MM/dd/yyyy HH:mm:ss");
                    model.JobDescription = "";
                    model.ServicePic = query.Service.Service1.ServicePic;
                    model.MatchingOrderOptionList = query.JobQuestionOptions.Select(z => new MatchingOrderOptionModel
                    {
                        QuestionTitle = z.QuestionOption.QuestionMaster.QuestionTitle,
                        OptionID = z.OptionID,
                        OtherAnswer = z.OtherAnswer,
                        OptionHeading = z.QuestionOption.OptionHeading,
                        Placeholder = z.QuestionOption.Placeholder,
                        OrderOptionAttachList = z.JobOptionAttachments.Select(a => new OrderOptionAttachViewModel
                        {
                            JobOptionID = a.JobQuestionOptionsID,
                            NewName = a.NewName,
                            Path = a.Path,
                            Description = a.Description
                        }).ToList(),
                    }).ToList();

                    var proposal = context.Proposals.Where(x => x.JobID == viewModel.JobId && x.UserID == viewModel.UserId && x.IsActive == true).FirstOrDefault();
                    if (proposal != null)
                        model.IsProposalDone = true;
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/GetSavedProposalByJobId")]
        public IHttpActionResult GetSavedProposalByJobId(JobModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new ProposalModel();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Proposals.Where(x => x.JobID == viewModel.JobId && x.UserID == viewModel.UserId && (x.ProposalStatus == ProposalStatus.Pending.ToString() || x.ProposalStatus == ProposalStatus.Accepted.ToString()) && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new ProposalModel
                    {
                        ID = x.ID,
                        Description = x.Description,
                        DeliveryDate = x.DeliveryDate,
                        Amount = x.Amount,
                        HiredStatus = x.Hireds.Where(y => y.ProposalID == x.ID).Select(y => y.HiredStatus).FirstOrDefault(),
                        SPStatus = x.SPStatus,
                        CustomerStatus = x.CustomerStatus
                    }).FirstOrDefault();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/GetHiredDetailByProposalId")]
        public IHttpActionResult GetHiredDetailByProposalId(HireViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new HireModel();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Hireds.Where(x => x.ProposalID == viewModel.ProposalId && x.HiredStatus == HireStatus.Pending.ToString() && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new HireModel
                    {
                        ID = x.ID,
                        ProposalID = x.ProposalID,
                        Message = x.HiredMessage,
                        UpdatedDate = x.UpdatedDate,
                        HiredStatus = x.HiredStatus,
                        DeliveryDate = Convert.ToDateTime(x.Proposal.DeliveryDate).ToString("MM/dd/yyyy HH:mm:ss"),
                        Amount = x.Proposal.Amount,
                        UserID = x.StatusByUserID,
                        CustomerStatus = x.Proposal.CustomerStatus,
                        SPStatus = x.Proposal.SPStatus,
                        ProfilePic = x.AspNetUser.UserDetails2.Where(h => h.UserID == x.StatusByUserID).Select(h => h.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : h.ProfilePic).FirstOrDefault(),
                        UserName = x.AspNetUser.UserDetails2.Where(y => y.UserID == x.StatusByUserID).Select(y => y.FirstName).FirstOrDefault(),
                    }).FirstOrDefault();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }


        [HttpPost]
        [Route("api/AcceptRejectProposal")]
        public IHttpActionResult AcceptRejectProposal(AcceptRejectProposalViewModel viewModel)
        {
            bool success = false;
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
                    var hire = context.Hireds.Where(x => x.ProposalID == viewModel.ProposalId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (hire != null)
                    {
                        hire.HiredStatus = viewModel.ProposalType;
                        hire.UpdatedDate = DateTime.UtcNow;
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";
                    }
                    var proposal = context.Proposals.Where(x => x.ID == viewModel.ProposalId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (proposal != null)
                    {
                        proposal.ProposalStatus = viewModel.ProposalType;
                        proposal.UpdatedDate = DateTime.UtcNow;
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";
                    }

                    var postedJob = context.Jobs.Where(x => x.ID == proposal.JobID).FirstOrDefault();
                    if (postedJob != null)
                    {
                        //check sp push notification setting
                        Util.SaveActivity(context, "You have " + viewModel.ProposalType + " an offer for the order " + postedJob.JobTitle, viewModel.UserId, viewModel.UserId, viewModel.ProposalType, postedJob.ID, "ServiceProvider", false, false);
                        //check push notification setting                    
                        var isPushNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), postedJob.UserID, context);
                        Util.SaveActivity(context, "Offer " + viewModel.ProposalType + " for the order " + postedJob.JobTitle, viewModel.UserId, postedJob.UserID, viewModel.ProposalType, postedJob.ID, "Customer", isPushNotify, false);

                        //check email notification setting
                        bool isEmalNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), Convert.ToString(NotificationCategoryEnum.EMAIL_NOTIFICATION), postedJob.UserID, context);
                        if (isEmalNotify)
                        {
                            var fromUserExist = UserManager.FindById(viewModel.UserId);
                            var toUserExist = UserManager.FindById(postedJob.UserID);
                            if (fromUserExist != null && toUserExist != null)
                            {
                                var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                #region Email Sending Code
                                try
                                {
                                    string emailBody = CommonLib.GetEmailTemplateValue("AcceptRejectProposal/Body");
                                    string emailSubject = "QuiGig - Invitation " + viewModel.ProposalType;
                                    string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                    try
                                    {
                                        var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                        emailBody = emailBody.Replace("@@@Path", path);
                                        emailBody = emailBody.Replace("@@@JobTitle", postedJob.JobTitle);
                                        emailBody = emailBody.Replace("@@@ProposalType", viewModel.ProposalType);
                                        emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                        emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                        CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                #endregion
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/CompletedDisputeProposal")]
        public IHttpActionResult CompletedDisputeProposal(CompletedDisputeProposalVM model)
        {
            bool success = false;
            var message = "";
            var rUrl = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var proposal = context.Proposals.Where(x => x.ID == model.ProposalId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (proposal != null)
                    {
                        if (model.ProposalStatus == "SPStatus")
                            proposal.SPStatus = model.ProposalType;
                        else
                            proposal.CustomerStatus = model.ProposalType;
                        proposal.UpdatedDate = DateTime.UtcNow;
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";

                        string userType = "";
                        if (model.ProposalStatus == "SPStatus")
                        {
                            userType = "Customer";
                            Util.SaveActivity(context, "Order " + model.ProposalType + " " + proposal.Job.JobTitle, model.UserId, proposal.Job.UserID, model.ProposalType, proposal.JobID, userType, false, false);
                            bool isNotify = Util.GetNotificationValue(NotificationEnum.JOB_COMPLETE.ToString(), NotificationCategoryEnum.EMAIL_NOTIFICATION.ToString(), proposal.Job.UserID, context);
                            if (isNotify)
                            {
                                var fromUserExist = UserManager.FindById(model.UserId);
                                var toUserExist = UserManager.FindById(proposal.Job.UserID);
                                if (fromUserExist != null && toUserExist != null)
                                {
                                    var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                    var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                    #region Email Sending Code
                                    try
                                    {
                                        string emailBody = CommonLib.GetEmailTemplateValue("AcceptRejectProposal/Body");
                                        string emailSubject = "QuiGig - Order " + model.ProposalId;
                                        string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                        try
                                        {
                                            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                            emailBody = emailBody.Replace("@@@Path", path);
                                            emailBody = emailBody.Replace("@@@JobTitle", proposal.Job.JobTitle);
                                            emailBody = emailBody.Replace("@@@ProposalType", model.ProposalType);
                                            emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                            emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                            CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    #endregion
                                }
                            }
                            rUrl = "review-rating?parm1=" + proposal.JobID + "&parm2=" + proposal.Job.UserID;
                        }
                        else
                        {
                            userType = "ServiceProvider";
                            Util.SaveActivity(context, "Order " + model.ProposalType + " " + proposal.Job.JobTitle, model.UserId, proposal.UserID, model.ProposalType, proposal.JobID, userType, false, false);
                            bool isNotify = Util.GetNotificationValue(NotificationEnum.JOB_COMPLETE.ToString(), NotificationCategoryEnum.EMAIL_NOTIFICATION.ToString(), proposal.UserID, context);
                            if (isNotify)
                            {
                                var fromUserExist = UserManager.FindById(model.UserId);
                                var toUserExist = UserManager.FindById(proposal.UserID);
                                if (fromUserExist != null && toUserExist != null)
                                {
                                    var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                    var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();
                                    #region Email Sending Code
                                    try
                                    {
                                        string emailBody = CommonLib.GetEmailTemplateValue("AcceptRejectProposal/Body");
                                        string emailSubject = "QuiGig - Order " + model.ProposalId;
                                        string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                        try
                                        {
                                            var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                            emailBody = emailBody.Replace("@@@Path", path);
                                            emailBody = emailBody.Replace("@@@JobTitle", proposal.Job.JobTitle);
                                            emailBody = emailBody.Replace("@@@ProposalType", model.ProposalType);
                                            emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                            emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                            CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    #endregion
                                }
                            }
                            rUrl = "review-rating?parm1=" + proposal.JobID + "&parm2=" + proposal.UserID;
                        }

                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                RUrl = rUrl
            });
        }

        [HttpPost]
        [Route("api/SaveProposal")]
        public IHttpActionResult SaveProposal(SaveProposalModel model)
        {
            bool success = false;
            var message = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                if (model.JobExpireDate >= DateTime.UtcNow)
                {
                    try
                    {
                        if (model.ID == 0)
                        {
                            var userDetail = context.UserDetails.Where(x => x.UserID == model.UserID).FirstOrDefault();
                            var userCurrentPlan = context.UserPlans.Where(x => x.UserID == model.UserID).FirstOrDefault();
                            var featurePostOrder = context.Features.Where(x => x.UniqueCode == "BID").FirstOrDefault();
                            var featureVal = featurePostOrder.PlanDurationFeatures.FirstOrDefault();
                            if (userDetail.BidPoints >= Convert.ToInt32(featureVal.FeatureValue))
                                Util.SaveUserWallet(context, model.UserID, 0, "Post Bid", 0, Convert.ToInt32(featureVal.FeatureValue), 0, PaymentStatus.Completed.ToString(), PaymentFromSite.QuiGig.ToString(), 0, 0, "Post Bid");
                            else
                            {
                                return Ok(new
                                {
                                    Success = false,
                                    Message = "You have not sufficent quigs for post a bid"
                                });
                            }
                        }
                        var totalProposalCount = context.Proposals.Where(x => x.JobID == model.JobID && x.CustomerStatus != ProposalStatus.Completed.ToString() && x.SPStatus != ProposalStatus.Completed.ToString() && x.IsActive == true && x.IsDelete == false).Count();
                        var proposal = context.Proposals.Where(x => x.ID == model.ID && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                        proposal = proposal == null ? new Proposal() : proposal;
                        proposal.JobID = model.JobID;
                        proposal.UserID = model.UserID;
                        proposal.Amount = model.Amount;
                        proposal.Description = model.Description;
                        proposal.Duration = model.Duration;
                        proposal.DeliveryDate = Convert.ToDateTime(model.StartDate).AddDays(Convert.ToInt32(model.Duration));
                        proposal.StartDate = model.StartDate;
                        proposal.BidPoint = 0;

                        proposal.ProposalStatus = ProposalStatus.Pending.ToString();
                        proposal.IsActive = true;
                        proposal.IsDelete = false;
                        proposal.CreatedDate = DateTime.UtcNow;
                        proposal.UpdatedDate = DateTime.UtcNow;
                        if (proposal.ID == 0)
                        {
                            if (totalProposalCount >= 3)
                                proposal.IsBidLock = false;
                            else
                                proposal.IsBidLock = true;
                            proposal.CreatedDate = DateTime.UtcNow;
                            proposal.UpdatedDate = DateTime.UtcNow;
                            context.Proposals.Add(proposal);
                        }
                        else
                        {
                            proposal.UpdatedDate = DateTime.UtcNow;
                        }
                        //var savedOrder = context.SavedOrders.Where(x => x.JobID == model.JobID && x.UserId == userId).FirstOrDefault();
                        //if (savedOrder != null)
                        //    context.SavedOrders.Remove(savedOrder);
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";
                        if (model.ID == 0)
                        {
                            #region Send Notification and email

                            var proposalCount = context.Proposals.Where(x => x.JobID == model.JobID && x.CustomerStatus != ProposalStatus.Completed.ToString() && x.SPStatus != ProposalStatus.Completed.ToString() && x.IsActive == true && x.IsDelete == false).Count();
                            var postedJob = context.Jobs.Where(x => x.ID == model.JobID && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                            if (postedJob != null)
                            {
                                bool isUserPushNotify = Util.GetNotificationValue(NotificationEnum.RECEIVED_BID.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), model.UserID, context);
                                if (proposalCount > 3)
                                    Util.SaveActivityProposal(context, "Your bid is not in top 3 go to project & unlock, So customer can view your bid & proposal", model.UserID, model.UserID, ActivityType.Hire.ToString(), model.JobID, "ServiceProvider", isUserPushNotify, false, proposal.ID);
                                else
                                    Util.SaveActivityProposal(context, "Congratulations! Your bid is in the First Three!", model.UserID, model.UserID, ActivityType.Hire.ToString(), model.JobID, "ServiceProvider", isUserPushNotify, false, proposal.ID);

                                Util.SaveActivityProposal(context, "Bid Posted: " + postedJob.JobTitle, model.UserID, model.UserID, ActivityType.Hire.ToString(), model.JobID, "ServiceProvider", isUserPushNotify, false, proposal.ID);

                                // //check user push notification setting
                                bool isPushNotify = Util.GetNotificationValue(NotificationEnum.RECEIVED_BID.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), postedJob.UserID, context);
                                Util.SaveActivity(context, "Recieved a Bid:" + postedJob.JobTitle, model.UserID, postedJob.UserID, ActivityType.Hire.ToString(), postedJob.ID, "Customer", isPushNotify, false);

                                //check user email notification setting
                                bool isUsernotify = Util.GetNotificationValue(NotificationEnum.RECEIVED_BID.ToString(), Convert.ToString(NotificationCategoryEnum.EMAIL_NOTIFICATION), proposal.Job.UserID, context);
                                if (isUsernotify)
                                {
                                    var fromUserExist = UserManager.FindById(model.UserID);
                                    var toUserExist = UserManager.FindById(postedJob.UserID);
                                    if (fromUserExist != null && toUserExist != null)
                                    {
                                        var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                        var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                        #region Email Sending Code
                                        try
                                        {
                                            string emailBody = CommonLib.GetEmailTemplateValue("SendProposal/Body");
                                            string emailSubject = "QuiGig - Received new bid";
                                            string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                            try
                                            {
                                                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                                emailBody = emailBody.Replace("@@@Path", path);
                                                emailBody = emailBody.Replace("@@@JobTitile", postedJob.JobTitle);
                                                emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                                emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                                CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                }
                else
                    message = "Job has been expired";
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/SaveHire")]
        public IHttpActionResult SaveHire(HireViewModel model)
        {
            bool success = false;
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
                    Hired hire = new Hired();
                    hire.ProposalID = model.ProposalId;
                    hire.HiredMessage = "You have received an offer for the job";
                    hire.HiredDate = DateTime.UtcNow;
                    hire.HiredStatus = HireStatus.Pending.ToString();
                    hire.StatusByUserID = model.UserId;
                    hire.IsActive = true;
                    hire.IsDelete = false;
                    hire.CreatedDate = DateTime.UtcNow;
                    hire.UpdatedDate = DateTime.UtcNow;
                    context.Hireds.Add(hire);
                    context.SaveChanges();
                    success = true;
                    message = "Save successfully.";
                    var proposal = context.Proposals.Where(x => x.ID == model.ProposalId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();

                    if (proposal != null)
                    {
                        // //check user push notification setting
                        bool isPushNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), proposal.UserID, context);
                        Util.SaveActivity(context, "Received a offer for the order " + proposal.Job.JobTitle, model.UserId, proposal.UserID, ActivityType.Accepted.ToString(), proposal.JobID, "ServiceProvider", isPushNotify, false);

                        // //check user email notification setting
                        bool isNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), Convert.ToString(NotificationCategoryEnum.EMAIL_NOTIFICATION), proposal.UserID, context);
                        if (isNotify)
                        {
                            var fromUserExist = UserManager.FindById(model.UserId);
                            var toUserExist = UserManager.FindById(proposal.UserID);
                            if (fromUserExist != null && toUserExist != null)
                            {
                                var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                #region Email Sending Code
                                try
                                {
                                    string emailBody = CommonLib.GetEmailTemplateValue("HireOffer/Body");
                                    string emailSubject = "QuiGig - Offer received";
                                    string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                    var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                    emailBody = emailBody.Replace("@@@Path", path);
                                    emailBody = emailBody.Replace("@@@JobTitle", proposal.Job.JobTitle);
                                    emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                    emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                    CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                }
                                catch (Exception ex)
                                {
                                    message = ex.Message;
                                }
                                #endregion
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/CancelHire")]
        public IHttpActionResult CancelHire(HireViewModel model)
        {
            bool success = false;
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
                    var hire = context.Hireds.Where(x => x.ProposalID == model.ProposalId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (hire != null)
                    {
                        context.Hireds.Remove(hire);
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";

                        var proposal = context.Proposals.Where(x => x.ID == hire.ProposalID && x.IsActive == true && x.IsDelete == false).FirstOrDefault();

                        if (proposal != null)
                        {
                            // //check user push notification setting
                            bool isPushNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), Convert.ToString(NotificationCategoryEnum.PUSH_NOTIFICATION), proposal.UserID, context);
                            Util.SaveActivity(context, "Cancel an offer for the order " + proposal.Job.JobTitle, model.UserId, proposal.UserID, ActivityType.Canceled.ToString(), proposal.JobID, "ServiceProvider", isPushNotify, false);


                            // //check user email notification setting
                            bool isNotify = Util.GetNotificationValue(NotificationEnum.HIRING.ToString(), NotificationCategoryEnum.EMAIL_NOTIFICATION.ToString(), proposal.UserID, context);
                            if (isNotify)
                            {
                                var fromUserExist = UserManager.FindById(model.UserId);
                                var toUserExist = UserManager.FindById(proposal.UserID);
                                if (fromUserExist != null && toUserExist != null)
                                {
                                    var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                                    var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                                    #region Email Sending Code
                                    try
                                    {
                                        string emailBody = CommonLib.GetEmailTemplateValue("CancelOffer/Body");
                                        string emailSubject = "QuiGig - Cancel invitation";
                                        string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                        var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                        emailBody = emailBody.Replace("@@@Path", path);
                                        emailBody = emailBody.Replace("@@@JobTitle", proposal.Job.JobTitle);
                                        emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                        emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                        CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                                    }
                                    catch (Exception ex)
                                    {
                                        message = ex.Message;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/GetHireProposalUser")]
        public IHttpActionResult GetHireProposalUser(JobModel viewModel)
        {
            bool success = false;
            bool showMoreCount = false;
            bool isUnlimitedBidView = false;
            var message = "";
            var model = new ProposalListModel();
            try
            {
                var jobDetail = context.Jobs.Where(x => x.ID == viewModel.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                if (jobDetail != null)
                {
                    //var query = context.GetProposalUserList(jobId);
                    //model.InvitedProposalList = query.Where(x => x.IsInvited == 1).Select(y => new ProposalViewModel
                    //{
                    //    ID = y.JobID,
                    //    Description = y.Description,
                    //    DeliveryDate = y.DeliveryDate,
                    //    Amount = y.Amount.Value,
                    //    JobID = y.JobID,
                    //    ProposalID = y.ProposalID.Value,
                    //    UserID = y.UserID,
                    //    CustomerStatus = y.CustomerStatus,
                    //    IsUnlimitedBidView = y.IsUnlimitedBidView,
                    //    SPStatus = y.SPStatus,
                    //    HiredStatus = y.HiredStatus,
                    //    Duration = Convert.ToInt32(y.Duration),
                    //    ProfilePic = y.ProfilePic,
                    //    CreatedDate = Util.TimeAgo(Convert.ToDateTime(y.CreatedDate)),
                    //    StartDate = Convert.ToDateTime(y.StartDate),
                    //    UserName = y.UserName,
                    //}).ToList();

                    //var userDetail = context.UserDetails.Where(x => x.UserID == userId).FirstOrDefault();

                    var proposalList = context.GetProposalUserList(viewModel.JobId);
                    //if (userDetail.BidPoints == 0 || userDetail.BidPoints <= 50)
                    //    proposalList.Take(3);
                    model.ProposalList = proposalList.Where(x => x.IsInvited == 0 && x.IsBidLock == true).Select(y => new ProposalModel
                    {
                        Description = y.Description,
                        DeliveryDate = y.DeliveryDate,
                        Amount = y.Amount.Value,
                        JobID = y.JobID,
                        ProposalID = y.ProposalID.Value,
                        UserID = y.UserID,
                        location = y.location,
                        CustomerStatus = y.CustomerStatus,
                        IsUnlimitedBidView = y.IsUnlimitedBidView,
                        SPStatus = y.SPStatus,
                        HiredStatus = y.HiredStatus,
                        CreatedDate = Util.TimeAgo(Convert.ToDateTime(y.CreatedDate)),
                        StartDate = Convert.ToDateTime(y.StartDate),
                        ProfilePic = y.ProfilePic,
                        IsBidLock = y.IsBidLock,
                        Duration = Convert.ToInt32(y.Duration),
                        UserName = y.UserName,
                        MyGigsCount = Convert.ToInt32(y.MyGigsCount),
                        MyBidsCount = Convert.ToInt32(y.MyBidsCount),
                        TotalLikeCount = (Convert.ToInt32(y.ThumbsUp) - Convert.ToInt32(y.ThumbsDown)) / 100,
                        CreatedProposalDate = Convert.ToDateTime(y.CreatedProposalDate).ToString("MM/dd/yyyy HH:mm:ss"),
                    }).ToList();
                    var lockedProposalList = context.GetProposalUserList(viewModel.JobId);
                    model.LockedProposalList = lockedProposalList.Where(x => x.IsInvited == 0 && x.IsBidLock == false).Select(y => new ProposalModel
                    {
                        Description = y.Description,
                        DeliveryDate = y.DeliveryDate,
                        Amount = y.Amount.Value,
                        JobID = y.JobID,
                        ProposalID = y.ProposalID.Value,
                        UserID = y.UserID,
                        CustomerStatus = y.CustomerStatus,
                        IsUnlimitedBidView = y.IsUnlimitedBidView,
                        SPStatus = y.SPStatus,
                        HiredStatus = y.HiredStatus,
                        CreatedDate = Util.TimeAgo(Convert.ToDateTime(y.CreatedDate)),
                        StartDate = Convert.ToDateTime(y.StartDate),
                        ProfilePic = y.ProfilePic,
                        IsBidLock = y.IsBidLock,
                        location = y.location,
                        Duration = Convert.ToInt32(y.Duration),
                        UserName = y.UserName,
                        MyGigsCount = Convert.ToInt32(y.MyGigsCount),
                        MyBidsCount = Convert.ToInt32(y.MyBidsCount),
                        TotalLikeCount = (Convert.ToInt32(y.ThumbsUp) - Convert.ToInt32(y.ThumbsDown)) / 100,
                        CreatedProposalDate = Convert.ToDateTime(y.CreatedProposalDate).ToString("MM/dd/yyyy HH:mm:ss")
                    }).ToList();
                }
                success = true;
                message = "Get List";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model,
                ShowMoreCount = showMoreCount,
                IsUnlimitedBidView = isUnlimitedBidView
            });
        }

        [HttpPost]
        [Route("api/GetHiredUser")]
        public IHttpActionResult GetHiredUser(JobViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<HiredUserModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Hireds.Where(x => x.Proposal.JobID == viewModel.JobId && (x.HiredStatus == HireStatus.Pending.ToString() || x.HiredStatus == HireStatus.Accepted.ToString()) && (x.Proposal.CustomerStatus == null || x.Proposal.CustomerStatus == ProposalStatus.Completed.ToString()) && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new HiredUserModel
                    {
                        ID = x.ID,
                        ProposalID = x.ProposalID,
                        Description = x.Proposal.Description,
                        DeliveryDate = x.Proposal.DeliveryDate,
                        SPStatus = x.Proposal.SPStatus,
                        CustomerStatus = x.Proposal.CustomerStatus,
                        Amount = x.Proposal.Amount,
                        JobID = x.Proposal.JobID,
                        HiredStatus = x.HiredStatus,
                        ProfilePic = x.Proposal.AspNetUser.UserDetails2.Where(h => h.UserID == x.Proposal.UserID).Select(h => h.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : h.ProfilePic).FirstOrDefault(),
                        UserName = x.Proposal.AspNetUser.UserDetails2.Where(y => y.UserID == x.Proposal.UserID).Select(y => y.FirstName).FirstOrDefault(),
                    }).ToList();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/GetAllActivityList")]
        public IHttpActionResult GetAllActivityList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<ActivityModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Activities.Where(x => x.ToID == viewModel.UserId && x.IsActive == true && x.IsDelete == false).OrderByDescending(x => x.ID);
                    model = query.Select(x => new ActivityModel
                    {
                        ID = x.ID,
                        UserName = x.AspNetUser.UserDetails2.Where(y => y.UserID == x.FromID).Select(y => y.FirstName).FirstOrDefault(),
                        Date = x.UpdatedDate,
                        Message = x.Message,
                        ActivityType = x.ActivityType,
                        IsNotification = x.IsNotification,
                        RedirectPath = x.RedirectPath
                    }).ToList();

                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        //[HttpGet]
        //[Route("api/SeeMoreBid")]
        //public IHttpActionResult SeeMoreBid(long jobId)
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new List<MatchingOrderViewModel>();
        //    try
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var userDetail = context.UserDetails.Where(x => x.UserID == userId).FirstOrDefault();
        //        if (userDetail.BidPoints > Convert.ToInt64(Folder.ViewBidMinCoin))
        //        {
        //            int availableBidCoin = userDetail.BidPoints - Convert.ToInt32(Folder.ViewBidMinCoin);
        //            userDetail.BidPoints = availableBidCoin;
        //            userDetail.UpdatedDate = DateTime.UtcNow;
        //            context.SaveChanges();

        //            var jobDetail = context.Jobs.Where(x => x.ID == jobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
        //            if (jobDetail != null)
        //            {
        //                jobDetail.IsUnlimitedBidView = true;
        //                context.SaveChanges();
        //            }
        //            message = "GetList";
        //        }
        //        else
        //            message = "PurchaseCoin";
        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        Result = model
        //    });
        //}

        //[HttpPost]
        //[Route("api/SaveCoin")]
        //public IHttpActionResult SaveCoin(long jobId, int amount)
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new List<MatchingOrderViewModel>();
        //    try
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var userDetail = context.UserDetails.Where(x => x.UserID == userId).FirstOrDefault();
        //        int availableBidCoin = CalculateBidCoin(amount, Convert.ToInt32(Folder.ViewBidMinCoin));
        //        userDetail.BidPoints = (userDetail.BidPoints + availableBidCoin);
        //        userDetail.UpdatedDate = DateTime.UtcNow;
        //        context.SaveChanges();

        //        var jobDetail = context.Jobs.Where(x => x.ID == jobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
        //        if (jobDetail != null)
        //        {
        //            jobDetail.IsUnlimitedBidView = true;
        //            context.SaveChanges();
        //        }
        //        message = "Successfuly purchase coin";
        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        Result = model
        //    });
        //}

        //public int CalculateBidCoin(int amount, int coin)
        //{
        //    int bidCoin = 0;
        //    try
        //    {
        //        bidCoin = (amount * 100) - coin;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return bidCoin;
        //}

        [HttpPost]
        [Route("api/EditSavedProposal")]
        public IHttpActionResult EditSavedProposal(EditProposalVM viewModel)
        {
            bool success = false;
            var message = "";
            var model = new EditProposalModel();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.Proposals.Where(x => x.ID == viewModel.ProposalId && x.IsActive == true && x.IsDelete == false);
                    model = query.Select(x => new EditProposalModel
                    {
                        ID = x.ID,
                        JobID = x.JobID,
                        EditDeliveryDate = x.DeliveryDate.ToString(),
                        Description = x.Description,
                        Amount = x.Amount
                    }).FirstOrDefault();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/ReHireService")]
        public IHttpActionResult ReHireService(ReHireVM viewModel)
        {
            bool success = false;
            var message = "";
            var rUrl = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var postedJob = context.Jobs.Where(x => x.ID == viewModel.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();

                    var invitedUser = context.InvitedUsers.Where(x => x.JobID == postedJob.ID && x.UserID == viewModel.ToUserId).FirstOrDefault();
                    if (invitedUser == null)
                    {
                        invitedUser = new InvitedUser();
                        invitedUser.JobID = postedJob.ID;
                        invitedUser.UserID = viewModel.ToUserId;
                        invitedUser.IsActive = true;
                        invitedUser.IsDelete = false;
                        invitedUser.CreatedDate = DateTime.UtcNow;
                        invitedUser.UpdatedDate = DateTime.UtcNow;
                        context.InvitedUsers.Add(invitedUser);
                        context.SaveChanges();
                        Util.SaveActivity(context, "Received re invitation for the order " + postedJob.JobTitle, viewModel.UserId, viewModel.ToUserId, ActivityType.Accepted.ToString(), postedJob.ID, "ServiceProvider", true, true);
                        var fromUserExist = UserManager.FindById(viewModel.UserId);
                        var toUserExist = UserManager.FindById(viewModel.ToUserId);

                        var fromUserDetail = context.UserDetails.Where(x => x.UserID == fromUserExist.Id).FirstOrDefault();
                        var toUserDetail = context.UserDetails.Where(x => x.UserID == toUserExist.Id).FirstOrDefault();

                        if (fromUserExist != null && toUserExist != null)
                        {
                            #region Email Sending Code
                            try
                            {
                                string emailBody = CommonLib.GetEmailTemplateValue("HireOffer/Body");
                                string emailSubject = "QuiGig - Re Hire Invitation received";
                                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                emailBody = emailBody.Replace("@@@Path", path);
                                emailBody = emailBody.Replace("@@@JobTitle", postedJob.JobTitle);
                                emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                            }
                            catch (Exception ex)
                            {
                                message = ex.Message;
                            }
                            #endregion


                            #region Send Email to customer
                            Util.SaveActivity(context, "You have successfully invited " + toUserDetail.FirstName + " for your project " + postedJob.JobTitle, viewModel.UserId, viewModel.UserId, ActivityType.Accepted.ToString(), postedJob.ID, "Customer", true, true);
                            try
                            {
                                string emailBody = CommonLib.GetEmailTemplateValue("ReHireInvitation/Body");
                                string emailSubject = "QuiGig - Re Hire Invitation";
                                string strFromEmailAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                                var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                                emailBody = emailBody.Replace("@@@Path", path);
                                emailBody = emailBody.Replace("@@@JobTitle", postedJob.JobTitle);
                                emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                                emailBody = emailBody.Replace("@@@ToUserName", toUserDetail.FirstName);
                                CommonLib.SendMail(strFromEmailAddress, toUserExist.Email, emailSubject, emailBody);
                            }
                            catch (Exception ex)
                            {
                                message = ex.Message;
                            }
                            #endregion
                        }
                        success = true;
                        message = "You have successfully Re Hire this service";
                        rUrl = "/dashboard";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                RUrl = rUrl
            });
        }

        [HttpPost]
        [Route("api/GetExistingPostedOrderList")]
        public IHttpActionResult GetExistingPostedOrderList(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<NewOrderModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var query = context.GetExistingPostedOrderList(viewModel.UserId);
                    model = query.Select(x => new NewOrderModel
                    {
                        ID = x.ID,
                        JobTitle = x.JobTitle,
                        JobDescription = "",
                        Date = x.UpdatedDate
                    }).ToList();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpPost]
        [Route("api/SavedOrder")]
        public IHttpActionResult SavedOrder(JobModel model)
        {
            bool success = false;
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
                    var query = context.Jobs.Where(x => x.ID == model.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    if (query != null)
                    {
                        var savedOrder = context.SavedOrders.Where(x => x.JobID == query.ID && x.UserId == model.UserId).FirstOrDefault();
                        if (savedOrder == null)
                        {
                            savedOrder = new SavedOrder();
                            savedOrder.UserId = model.UserId;
                            savedOrder.JobID = query.ID;
                            savedOrder.CreatedDate = query.CreatedDate;
                            savedOrder.JobExpireDate = query.JobExpireDate;
                            context.SavedOrders.Add(savedOrder);
                            context.SaveChanges();
                            success = true;
                            message = "Saved order successfully";
                        }
                        else
                            message = "Already saved order";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message
            });
        }

        [HttpPost]
        [Route("api/GetSavedOrderList")]
        public IHttpActionResult GetSavedOrderList(GetSavedOrderVM viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<SavedOrderModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.GetSavedOrderList(viewModel.UserId, viewModel.SearchKeyword).Select(x => new SavedOrderModel()
                    {
                        ID = x.ID,
                        JobTitle = x.JobTitle,
                        CreatedDate = x.CreatedDate
                    }).ToList();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpGet]
        [Route("api/GetExpireOrderList")]
        public IHttpActionResult GetExpireOrderList(GetSavedOrderVM viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<ExpireOrderModel>();
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    model = context.GetExpireOrderList(viewModel.UserId, viewModel.SearchKeyword).Select(x => new ExpireOrderModel()
                    {
                        ID = x.ID,
                        JobTitle = x.JobTitle,
                        UpdatedDate = x.UpdatedDate
                    }).ToList();
                    success = true;
                    message = "Get List";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        //[HttpPost]
        //[Route("api/PurchaseCoin")]
        //public IHttpActionResult PurchaseCoin(PurchaseCoinViewModel model)
        //{
        //    bool success = false;
        //    var message = "";
        //    var rUrl = "";
        //    var userId = User.Identity.GetUserId();
        //    if (model.Provider == ProviderEnum.PayPal.ToString())
        //    {
        //        string retMsg = "";
        //        string token = "";
        //        bool ret = PayPalController.ShortcutExpressCheckout(Convert.ToString(model.Amount), ref token, ref retMsg, "/ReturnPurchaseCoin");
        //        if (ret)
        //        {
        //            rUrl = retMsg;
        //            Util.SaveUserPayment(context, userId, model.Provider, 0, model.Amount, "Purchase Coin", token, "");
        //            success = true;
        //            message = "Save successfully.";
        //        }
        //    }
        //    if (model.Provider == ProviderEnum.Stripe.ToString())
        //    {
        //        var customers = new StripeCustomerService();
        //        var charges = new StripeChargeService();

        //        var customer = customers.Create(new StripeCustomerCreateOptions
        //        {
        //            Email = model.StripeEmail,
        //            SourceToken = model.StripeToken
        //        });

        //        var charge = charges.Create(new StripeChargeCreateOptions
        //        {
        //            Amount = (model.Amount * 100),
        //            Description = "Purchase Coin",
        //            Currency = "usd",
        //            CustomerId = customer.Id
        //        });
        //        long id = Util.SaveUserPayment(context, userId, model.Provider, 0, model.Amount, "Purchase Coin", model.StripeToken, "");
        //        if (id > 0)
        //            Util.SaveUserWallet(context, userId, id, "Purchase Coin", (model.Amount * 100), 0);
        //        context.SaveChanges();
        //        success = true;
        //        message = "Save successfully.";
        //        rUrl = "/dashboard";
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        RUrl = rUrl
        //    });
        //}
    }
}
