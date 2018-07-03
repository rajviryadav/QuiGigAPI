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
    public class MessageController : ApiController
    {
        QuiGigAPIEntities context = new QuiGigAPIEntities();

        [HttpPost]
        [Route("api/SaveMessage")]
        public IHttpActionResult SaveMessage(MessageListModel model)
        {
            bool success = false;
            var message = "";
            var fromUser = "";
            var fromUserImage = "";
            var toUser = "";
            var toUserImage = "";
            try
            {
                Message entity = new Message();
                entity.MessageText = model.MessageText;
                entity.FromID = model.UserId;
                entity.ToID = model.ToID;
                entity.JobID = model.JobID;
                entity.GroupId = model.GroupId;
                entity.IsRead = model.IsRead;
                entity.IsActive = true;
                entity.IsDelete = false;
                entity.CreatedDate = DateTime.UtcNow;
                context.Messages.Add(entity);
                context.SaveChanges();
                success = true;
                message = "Save successfully.";

                var fromUserDeatil = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                fromUser = fromUserDeatil.FirstName;
                fromUserImage = fromUserDeatil.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : fromUserDeatil.ProfilePic;

                var toUserDeatil = context.UserDetails.Where(x => x.UserID == model.ToID).FirstOrDefault();
                toUser = toUserDeatil.FirstName;
                toUserImage = toUserDeatil.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : toUserDeatil.ProfilePic;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                FromUserName = fromUser,
                FromUserID = model.UserId,
                FromUserImage = fromUserImage,
                ToUserName = toUser,
                ToUserImage = toUserImage
            });
        }

    
        [HttpGet]
        [Route("api/StartChatWithSP")]
        public IHttpActionResult StartChatWithSP(StartChatWithSPViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new StartChatWithSPModel();
            try
            {
                var jobDetail = context.Jobs.Where(x => x.ID == viewModel.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                var msgGroup = context.MessageGroups.Where(x => x.JobID == viewModel.JobId && x.GroupAdminID == viewModel.FromUserId && x.UniqueGroupName == ("QuiGig-Group-" + viewModel.ToUserId)).FirstOrDefault();
                if (msgGroup == null)
                {
                    msgGroup = new MessageGroup();
                    msgGroup.GroupName = jobDetail.JobTitle;
                    msgGroup.GroupAdminID = viewModel.FromUserId;
                    msgGroup.UniqueGroupName = "QuiGig-Group-" + viewModel.ToUserId;
                    msgGroup.JobID = jobDetail.ID;
                    msgGroup.IsActive = true;
                    msgGroup.CreatedDate = DateTime.UtcNow;
                    msgGroup.UpdatedDate = DateTime.UtcNow;
                    context.MessageGroups.Add(msgGroup);

                    MessageGroupMember msgGroupMember = new MessageGroupMember();
                    /// For service provider ////
                    msgGroupMember.GroupId = msgGroup.ID;
                    msgGroupMember.MemberId = viewModel.ToUserId;
                    context.MessageGroupMembers.Add(msgGroupMember);

                    /// For customer ////
                    MessageGroupMember entity = new MessageGroupMember();
                    entity.GroupId = msgGroup.ID;
                    entity.MemberId = viewModel.FromUserId;
                    context.MessageGroupMembers.Add(entity);
                    context.SaveChanges();
                }
                var proposal = context.Proposals.Where(x => x.JobID == viewModel.JobId && x.UserID == viewModel.ToUserId).FirstOrDefault();
                model = context.GetStartMessageChatWithUser(viewModel.JobId, proposal.ID, viewModel.ToUserId, viewModel.FromUserId).Select(x => new StartChatWithSPModel
                {
                    IsBlock = x.IsBlock,
                    BlockByID = x.BlockByID,
                    JobTitle = x.JobTitle,
                    BlockBy = x.BlockBy,
                    JobID = x.JobID,
                    JobCompleted = x.JobCompleted,
                    FromUserID = viewModel.FromUserId,
                    HiredStatus = x.HiredStatus,
                    GroupId = x.GroupId,
                    FromUserName = x.FromUserName,
                }).FirstOrDefault();
                try
                {
                    model.UserMessageList = context.GetUserMessageList(viewModel.JobId, viewModel.FromUserId, viewModel.ToUserId).Select(y => new UserMessageListModel
                    {
                        FromUserID = y.FromUserID,
                        MessageText = y.MessageText,
                        ToUserID = y.ToUserID,
                        FromUserImage = y.FromUserImage,
                        ToUserImage = y.ToUserImage,
                        FromUserShortName = y.FromUserShortName,
                        ToUserShortName = y.ToUserShortName
                    }).ToList();
                }
                catch (Exception ex)
                {

                }
                success = true;
                message = "Get List";
            }
            catch (Exception ex)
            {
                message = ex.InnerException.Message;
            }
            return Ok(new
            {
                Success = success,
                Message = message,
                Result = model
            });
        }

        [HttpGet]
        [Route("api/StartChatWithCustomer")]
        public IHttpActionResult StartChatWithCustomer(StartChatWithCustomerViewModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new StartChatWithCustomerModel();
            try
            {
                var jobDetail = context.Jobs.Where(x => x.ID == viewModel.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                var proposal = context.Proposals.Where(x => x.JobID == viewModel.JobId && x.UserID == viewModel.FromUserId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                model = context.MessageChat(viewModel.JobId, proposal.ID, jobDetail.UserID, viewModel.FromUserId).Select(x => new StartChatWithCustomerModel
                {
                    IsBlock = x.IsBlock,
                    BlockByID = x.BlockByID,
                    BlockBy = x.BlockBy,
                    JobTitle = x.JobTitle,
                    ToUserName = x.ToUserName,
                    ToUserID = jobDetail.UserID,
                    ToUserImage = x.ToUserImage,
                    FromUserName = x.FromUserName,
                    FromUserID = viewModel.FromUserId,
                    JobCompleted = x.JobCompleted,
                    GroupId = x.GroupId
                }).FirstOrDefault();
                try
                {
                    model.UserMessageList = context.GetUserMessageList(viewModel.JobId, jobDetail.UserID, viewModel.FromUserId).Select(y => new UserMessageListModel
                    {
                        FromUserID = y.FromUserID,
                        MessageText = y.MessageText,
                        ToUserID = y.ToUserID,
                        FromUserImage = y.FromUserImage,
                        ToUserImage = y.ToUserImage,
                        FromUserShortName = y.FromUserShortName,
                        ToUserShortName = y.ToUserShortName
                    }).ToList();
                }
                catch (Exception ex)
                {

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
                Result = model
            });
        }

        [HttpGet]
        [Route("api/MessageNotification")]
        public IHttpActionResult MessageNotification(UserViewModel viewModel)
        {
            bool success = false;
            var message = "";
            int msgCount = 0;
            var model = new List<MessageNotificationModel>();
            try
            {
                model = context.GetMessageNotification(viewModel.UserId).Select(x => new MessageNotificationModel
                {
                    FromUserName = x.FromUserName,
                    MessageText = x.MessageText,
                    FromUserID = x.FromUserID,
                    IsRead = x.IsRead,
                    JobID = x.JobID,
                    Date = x.CreatedDate,
                }).ToList();
                msgCount = model.Where(x => x.IsRead == false).Count();
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
                MsgCount = msgCount
            });
        }

        [HttpGet]
        [Route("api/GetConnectedUserByJobIdList")]
        public IHttpActionResult GetConnectedUserByJobIdList(JobModel viewModel)
        {
            bool success = false;
            var message = "";
            var model = new List<ConnectedUserModel>();
            try
            {
                model = context.GetConnectedUserByJobId(viewModel.JobId, viewModel.UserId).Select(x => new ConnectedUserModel
                {
                    JobID = x.JobID,
                    JobTitle = x.JobTitle,
                    UserID = x.UserID,
                    UserImage = x.UserImage,
                    UserName = x.UserName
                }).ToList();
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
                Result = model
            });
        }

        //[HttpGet]
        //[Route("api/GetAllConnectedUserList")]
        //public IHttpActionResult GetAllConnectedUserList()
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new List<ConnectedUserModel>();
        //    try
        //    {
        //        model = context.GetAllConnectedUserList(userId).Select(x => new ConnectedUserModel
        //        {
        //            JobID = x.JobID,
        //            JobTitle = x.JobTitle,
        //            UserID = x.UserID,
        //            UserImage = x.UserImage,
        //            UserName = x.UserName,
        //            UserShortName = x.UserShortName
        //        }).ToList();
        //        success = true;
        //        message = "Get List";
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

        //[HttpGet]
        //[Route("api/MessageConnectedUser")]
        //public IHttpActionResult MessageConnectedUser(string toUserId = "", long jobId = 0)
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new MessageChatViewModel();
        //    if (!string.IsNullOrEmpty(toUserId) && jobId > 0)
        //    {
        //        try
        //        {
        //            var fromUserId = User.Identity.GetUserId();
        //            var jobDetail = context.Jobs.Where(x => x.ID == jobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
        //            var msgGroup = context.MessageGroups.Where(x => x.JobID == jobId && x.GroupAdminID == jobDetail.UserID).FirstOrDefault();
        //            if (msgGroup == null)
        //            {
        //                msgGroup = new MessageGroup();
        //                msgGroup.GroupName = jobDetail.JobTitle;
        //                msgGroup.GroupAdminID = fromUserId;
        //                msgGroup.JobID = jobDetail.ID;
        //                msgGroup.CreatedDate = DateTime.UtcNow;
        //                msgGroup.UpdatedDate = DateTime.UtcNow;
        //                context.MessageGroups.Add(msgGroup);

        //                MessageGroupMember msgGroupMember = new MessageGroupMember();
        //                msgGroupMember.GroupId = msgGroup.ID;
        //                msgGroupMember.MemberId = fromUserId;
        //                context.MessageGroupMembers.Add(msgGroupMember);
        //                context.SaveChanges();
        //            }

        //            //var proposal = new Proposal();
        //            //proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == fromUserId && x.IsActive == true).FirstOrDefault();
        //            //if(proposal== null)
        //            //    proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == toUserId && x.IsActive == true).FirstOrDefault();
        //            //model = context.MessageChat(jobId, proposal.ID, toUserId, fromUserId).Select(x => new MessageChatViewModel
        //            //{
        //            //    IsBlock = x.IsBlock,
        //            //    BlockByID = x.BlockByID,
        //            //    JobTitle = x.JobTitle,
        //            //    BlockBy = x.BlockBy,
        //            //    JobID = x.JobID,
        //            //    JobCompleted = x.JobCompleted,
        //            //    FromUserID = fromUserId,
        //            //    HiredStatus = x.HiredStatus,
        //            //    ProposalID = proposal.ID,
        //            //    GroupId = x.GroupId,
        //            //    SPStatus = proposal.SPStatus,
        //            //    CustomerStatus = proposal.CustomerStatus,
        //            //    FromUserName = x.FromUserName,
        //            //}).FirstOrDefault();
        //            //try
        //            //{
        //            //    model.UserMessageList = context.GetUserMessageList(jobId, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //            //    {
        //            //        FromUserID = y.FromUserID,
        //            //        MessageText = y.MessageText,
        //            //        ToUserID = y.ToUserID,
        //            //        FromUserImage = y.FromUserImage,
        //            //        ToUserImage = y.ToUserImage
        //            //    }).ToList();
        //            //}
        //            //catch (Exception ex)
        //            //{

        //            //}
        //            if (Convert.ToString(HttpContext.Current.Session["CurrRoleName"]) == UserRoleEnum.ServiceProvider.ToString())
        //            {
        //                var proposal = new Proposal();
        //                proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == fromUserId && x.IsActive == true).FirstOrDefault();
        //                if (proposal == null)
        //                {
        //                    proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == toUserId && x.IsActive == true).FirstOrDefault();
        //                    model = context.MessageChat(jobId, proposal.ID, fromUserId, toUserId).Select(x => new MessageChatViewModel
        //                    {
        //                        IsBlock = x.IsBlock,
        //                        BlockByID = x.BlockByID,
        //                        JobTitle = x.JobTitle,
        //                        BlockBy = x.BlockBy,
        //                        JobID = x.JobID,
        //                        JobCompleted = x.JobCompleted,
        //                        FromUserID = fromUserId,
        //                        HiredStatus = x.HiredStatus,
        //                        ProposalID = proposal.ID,
        //                        GroupId = x.GroupId,
        //                        SPStatus = proposal.SPStatus,
        //                        CustomerStatus = proposal.CustomerStatus,
        //                        FromUserName = x.FromUserName,
        //                    }).FirstOrDefault();
        //                    try
        //                    {
        //                        model.UserMessageList = context.GetUserMessageList(jobId, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //                        {
        //                            FromUserID = y.FromUserID,
        //                            MessageText = y.MessageText,
        //                            ToUserID = y.ToUserID,
        //                            FromUserImage = y.FromUserImage,
        //                            ToUserImage = y.ToUserImage,
        //                            FromUserShortName = y.FromUserShortName,
        //                            ToUserShortName = y.ToUserShortName
        //                        }).ToList();
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }
        //                else
        //                {
        //                    model = context.MessageChat(jobId, proposal.ID, toUserId, fromUserId).Select(x => new MessageChatViewModel
        //                    {
        //                        IsBlock = x.IsBlock,
        //                        BlockByID = x.BlockByID,
        //                        JobTitle = x.JobTitle,
        //                        BlockBy = x.BlockBy,
        //                        JobID = x.JobID,
        //                        JobCompleted = x.JobCompleted,
        //                        FromUserID = fromUserId,
        //                        HiredStatus = x.HiredStatus,
        //                        ProposalID = proposal.ID,
        //                        GroupId = x.GroupId,
        //                        SPStatus = proposal.SPStatus,
        //                        CustomerStatus = proposal.CustomerStatus,
        //                        FromUserName = x.FromUserName,
        //                    }).FirstOrDefault();
        //                    try
        //                    {
        //                        model.UserMessageList = context.GetUserMessageList(jobId, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //                        {
        //                            FromUserID = y.FromUserID,
        //                            MessageText = y.MessageText,
        //                            ToUserID = y.ToUserID,
        //                            FromUserImage = y.FromUserImage,
        //                            ToUserImage = y.ToUserImage,
        //                            FromUserShortName = y.FromUserShortName,
        //                            ToUserShortName = y.ToUserShortName
        //                        }).ToList();
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == toUserId && x.IsActive == true).FirstOrDefault();
        //                model = context.GetStartMessageChatWithUser(jobId, proposal.ID, toUserId, fromUserId).Select(x => new MessageChatViewModel
        //                {
        //                    IsBlock = x.IsBlock,
        //                    BlockByID = x.BlockByID,
        //                    JobTitle = x.JobTitle,
        //                    BlockBy = x.BlockBy,
        //                    JobID = x.JobID,
        //                    JobCompleted = x.JobCompleted,
        //                    FromUserID = fromUserId,
        //                    HiredStatus = x.HiredStatus,
        //                    ProposalID = proposal.ID,
        //                    GroupId = x.GroupId,
        //                    SPStatus = proposal.SPStatus,
        //                    CustomerStatus = proposal.CustomerStatus,
        //                    FromUserName = x.FromUserName,
        //                }).FirstOrDefault();
        //                try
        //                {
        //                    model.UserMessageList = context.GetUserMessageList(jobId, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //                    {
        //                        FromUserID = y.FromUserID,
        //                        MessageText = y.MessageText,
        //                        ToUserID = y.ToUserID,
        //                        FromUserImage = y.FromUserImage,
        //                        ToUserImage = y.ToUserImage,
        //                        FromUserShortName = y.FromUserShortName,
        //                        ToUserShortName = y.ToUserShortName
        //                    }).ToList();
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }

        //            //model = context.Jobs.Where(x => x.IsDelete == false && x.IsActive == true && x.ID == jobId).Select(x => new MessageChatViewModel
        //            //{
        //            //    JobTitle = x.JobTitle,
        //            //    JobID = x.ID,
        //            //    FromUserID = fromUserId,
        //            //    ProposalID = proposal.ID,
        //            //    SPStatus = proposal.SPStatus,
        //            //    BlockByID = x.MessageGroups.Select(y=>y.MessageGroupMembers.Where(z=>z.MemberId == toUserId).Select(z=>z.BlockByID).FirstOrDefault()).FirstOrDefault(),
        //            //    CustomerStatus = proposal.CustomerStatus,
        //            //    HiredStatus = context.Hireds.Where(z => z.ProposalID == proposal.ID).Select(z => z.HiredStatus).FirstOrDefault(),
        //            //    GroupId = context.MessageGroupMembers.Where(y => y.MemberId == fromUserId).Select(g => g.GroupId).FirstOrDefault(),
        //            //    UserMessageList = x.Messages.Where(y => (y.FromID.Contains(fromUserId) || y.FromID.Contains(toUserId)) && (y.ToID.Contains(fromUserId) || y.ToID.Contains(toUserId))).Select(y => new UserMessageListViewModel
        //            //    {
        //            //        FromUserID = y.FromID,
        //            //        MessageText = y.MessageText,
        //            //        ToUserID = y.ToID,
        //            //        FromUserImage = y.AspNetUser.UserDetails2.Where(z => z.UserID == y.FromID).Select(z => (z.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : z.ProfilePic)).FirstOrDefault(),
        //            //        ToUserImage = y.AspNetUser1.UserDetails2.Where(z => z.UserID == y.ToID).Select(z => (z.ProfilePic == null ? "/Content/images/profile-progress-avtar.svg" : z.ProfilePic)).FirstOrDefault(),
        //            //    }).ToList()
        //            //}).FirstOrDefault();
        //            success = true;
        //            message = "Get List";
        //        }
        //        catch (Exception ex)
        //        {
        //            message = ex.Message;
        //        }
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        Result = model
        //    });
        //}

        [HttpPost]
        [Route("api/ReportUser")]
        public IHttpActionResult ReportUser(ReportModel model)
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
                    MessageReport report = new MessageReport();
                    report.ReportById = model.UserId;
                    report.ReportForId = model.ReportForId;
                    report.Message = model.Message;
                    report.JobId = model.JobId;
                    report.ReportToId = "2F078069-5797-4AE3-AEAB-FD11B9D71E2C";
                    report.IsActive = true;
                    report.IsDelete = false;
                    report.CreatedDate = DateTime.UtcNow;
                    report.UpdatedDate = DateTime.UtcNow;
                    context.MessageReports.Add(report);
                    context.SaveChanges();
                    success = true;
                    message = "Email sent to admin successfully.";

                    #region Email Sending Code
                    try
                    {
                        string emailBody = CommonLib.GetEmailTemplateValue("ReportUser/Body");
                        string emailSubject = "QuiGig - Report for user";
                        string strFromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["FromAddress"].ToString();
                        string adminEmail = System.Configuration.ConfigurationManager.AppSettings["NoSerivceEmail"].ToString();
                        var fromUserDetail = context.UserDetails.Where(x => x.UserID == model.UserId).FirstOrDefault();
                        var ReportForDetail = context.UserDetails.Where(x => x.UserID == model.ReportForId).FirstOrDefault();
                        var path = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority;
                        emailBody = emailBody.Replace("@@@Path", path);
                        emailBody = emailBody.Replace("@@@Message", model.Message);
                        emailBody = emailBody.Replace("@@@FromUserName", fromUserDetail.FirstName);
                        emailBody = emailBody.Replace("@@@ReportForUser", ReportForDetail.FirstName);
                        CommonLib.SendMail(strFromEmailAddress, adminEmail, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion
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
            });
        }

        [HttpPost]
        [Route("api/BlockUser")]
        public IHttpActionResult BlockUser(BlockUserModel model)
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
                    var member = context.MessageGroups.FirstOrDefault(x => x.JobID == model.JobId && x.ID == model.GroupId);
                    var groupMember = context.MessageGroupMembers.FirstOrDefault(x => x.MemberId == model.BlockUserID && x.GroupId == member.ID);
                    if (groupMember != null)
                    {
                        groupMember.IsBlock = true;
                        groupMember.BlockByID = model.UserId;
                        context.SaveChanges();
                        success = true;
                        message = "Block user successfully.";
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
            });
        }

        [HttpPost]
        [Route("api/UnBlockUser")]
        public IHttpActionResult UnBlockUser(UnBlockUserModel model)
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
                    var member = context.MessageGroups.FirstOrDefault(x => x.JobID == model.JobId && x.ID == model.GroupId);
                    var groupMember = context.MessageGroupMembers.FirstOrDefault(x => x.MemberId == model.BlockUserID && x.GroupId == member.ID);
                    if (groupMember != null)
                    {
                        groupMember.IsBlock = false;
                        groupMember.BlockByID = null;
                        context.SaveChanges();
                        success = true;
                        message = "Un block user successfully.";
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
            });
        }


        [HttpPost]
        [Route("api/NewProjectReHire")]
        public IHttpActionResult NewProjectReHire(JobViewModel model)
        {
            bool success = false;
            var message = "";
            long serviceid = 0;
            var reHireUserId = "";
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).FirstOrDefault();
                message = errors.Errors[0].ErrorMessage;
            }
            else
            {
                try
                {
                    var jobDetail = context.Jobs.Where(x => x.ID == model.JobId && x.IsActive == true && x.IsDelete == false).FirstOrDefault();
                    serviceid = jobDetail.ServiceID;
                    reHireUserId = jobDetail.UserID;
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
                ServiceId = serviceid,
                ReHireUserId = reHireUserId
            });
        }

        //[HttpGet]
        //[Route("api/GetConnectedUserMessageList")]
        //public IHttpActionResult GetConnectedUserMessageList(long jobId = 0, string toUserId = "")
        //{
        //    bool success = false;
        //    var message = "";
        //    var model = new MessageChatViewModel();
        //    try
        //    {
        //        var fromUserId = User.Identity.GetUserId();
        //        if (jobId == 0)
        //        {
        //            if (Convert.ToString(HttpContext.Current.Session["CurrRoleName"]) == UserRoleEnum.ServiceProvider.ToString())
        //            {
        //                var jobDetail = context.GetJobDetail(jobId, fromUserId, toUserId).FirstOrDefault();
        //                var proposal = context.Proposals.Where(x => x.JobID == jobDetail.JobID && x.UserID == fromUserId).FirstOrDefault();
        //                model = context.GetStartMessageChatWithUser(jobDetail.JobID, proposal.ID, fromUserId, toUserId).Select(x => new MessageChatViewModel
        //                {
        //                    IsBlock = x.IsBlock,
        //                    BlockByID = x.BlockByID,
        //                    JobTitle = x.JobTitle,
        //                    BlockBy = x.BlockBy,
        //                    JobID = x.JobID,
        //                    JobCompleted = x.JobCompleted,
        //                    FromUserID = fromUserId,
        //                    HiredStatus = x.HiredStatus,
        //                    ToUserName = x.FromUserName,
        //                    SPStatus = proposal.SPStatus,
        //                    CustomerStatus = proposal.CustomerStatus,
        //                    ProposalID = proposal.ID,
        //                    GroupId = x.GroupId
        //                }).FirstOrDefault();
        //                model.UserMessageList = context.GetUserMessageList(jobDetail.JobID, toUserId, fromUserId).Select(y => new UserMessageListViewModel
        //                {
        //                    FromUserID = y.FromUserID,
        //                    MessageText = y.MessageText,
        //                    FromUserImage = y.FromUserImage,
        //                    ToUserImage = y.ToUserImage,
        //                    ToUserID = y.ToUserID
        //                }).ToList();
        //            }
        //            else
        //            {
        //                var jobDetail = context.GetJobDetail(jobId, toUserId, fromUserId).FirstOrDefault();
        //                var proposal = context.Proposals.Where(x => x.JobID == jobDetail.JobID && x.UserID == toUserId).FirstOrDefault();
        //                model = context.GetMessageChatWithUser(jobDetail.JobID, proposal.ID, toUserId, fromUserId).Select(x => new MessageChatViewModel
        //                {
        //                    IsBlock = x.IsBlock,
        //                    BlockByID = x.BlockByID,
        //                    JobTitle = x.JobTitle,
        //                    BlockBy = x.BlockBy,
        //                    JobID = x.JobID,
        //                    JobCompleted = x.JobCompleted,
        //                    FromUserID = fromUserId,
        //                    HiredStatus = x.HiredStatus,
        //                    ToUserName = x.ToUserName,
        //                    GroupId = x.GroupId,
        //                    ProposalID = proposal.ID,
        //                    SPStatus = proposal.SPStatus,
        //                    CustomerStatus = proposal.CustomerStatus
        //                }).FirstOrDefault();
        //                model.UserMessageList = context.GetUserMessageList(jobDetail.JobID, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //                {
        //                    FromUserID = y.FromUserID,
        //                    MessageText = y.MessageText,
        //                    FromUserImage = y.FromUserImage,
        //                    ToUserImage = y.ToUserImage,
        //                    ToUserID = y.ToUserID
        //                }).ToList();
        //            }
        //        }
        //        else
        //        {
        //            var proposal = context.Proposals.Where(x => x.JobID == jobId && x.UserID == toUserId).FirstOrDefault();
        //            model = context.GetStartMessageChatWithUser(jobId, proposal.ID, toUserId, fromUserId).Select(x => new MessageChatViewModel
        //            {
        //                IsBlock = x.IsBlock,
        //                BlockByID = x.BlockByID,
        //                JobTitle = x.JobTitle,
        //                BlockBy = x.BlockBy,
        //                JobID = x.JobID,
        //                JobCompleted = x.JobCompleted,
        //                FromUserID = fromUserId,
        //                HiredStatus = x.HiredStatus,
        //                GroupId = x.GroupId,
        //                FromUserName = x.FromUserName,
        //                ToUserName = x.FromUserName,
        //            }).FirstOrDefault();
        //            model.UserMessageList = context.GetUserMessageList(jobId, fromUserId, toUserId).Select(y => new UserMessageListViewModel
        //            {
        //                FromUserID = y.FromUserID,
        //                MessageText = y.MessageText,
        //                ToUserID = y.ToUserID,
        //                FromUserImage = y.FromUserImage,
        //                ToUserImage = y.ToUserImage
        //            }).ToList();
        //        }

        //        success = true;
        //        message = "Get List";
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.InnerException.Message;
        //    }
        //    return Ok(new
        //    {
        //        Success = success,
        //        Message = message,
        //        Result = model
        //    });
        //}

        [HttpPost]
        [Route("api/MarkMessageRead")]
        public IHttpActionResult MarkMessageRead(UserViewModel user)
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
                    var messageList = context.Messages.Where(x => x.ToID == user.UserId && x.IsRead == false).ToList();
                    if (messageList.Count() > 0)
                    {
                        foreach (var item in messageList)
                            item.IsRead = true;
                        context.SaveChanges();
                        success = true;
                        message = "Save successfully.";
                    }
                    else
                    {
                        success = true;
                        message = "New new message found!";
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
    }
}
