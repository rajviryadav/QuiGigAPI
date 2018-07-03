using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class JobModel
    {
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class JobViewModel
    {
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
    }
    public class MatchingOrderModel
    {
        public long ID { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public string ServicePic { get; set; }
        public string JobExpireDate { get; set; }
        public int? ReceivedBid { get; set; }
        public string CreatedDate { get; set; }
        public bool IsProposalDone { get; set; }
        public List<MatchingOrderOptionModel> MatchingOrderOptionList { get; set; }
    }
    public class MatchingOrderOptionModel
    {
        public long OptionID { get; set; }
        public string OtherAnswer { get; set; }
        public string OptionHeading { get; set; }
        public string Placeholder { get; set; }
        public string QuestionTitle { get; set; }
        public List<OrderOptionAttachViewModel> OrderOptionAttachList { get; set; }
    }
    public class OrderOptionAttachViewModel
    {
        public long JobOptionID { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
    public class ProposalModel
    {
        public long ID { get; set; }
        public long ProposalID { get; set; }
        public long JobID { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int? Duration { get; set; }
        public string UserID { get; set; }
        public string HiredStatus { get; set; }
        public string SPStatus { get; set; }
        public string CustomerStatus { get; set; }
        public bool IsUnlimitedBidView { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
        public bool? IsBidLock { get; set; }
        public string CreatedDate { get; set; }
        public string location { get; set; }
        public int MyBidsCount { get; set; }
        public int MyGigsCount { get; set; }
        public int TotalLikeCount { get; set; }
        public string CreatedProposalDate { get; set; }
    }
    public class EditProposalModel
    {
        public long ID { get; set; }
        public string EditDeliveryDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public long JobID { get; set; }
    }
    public class HireModel
    {
        public long ID { get; set; }
        public long ProposalID { get; set; }
        public string Message { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string HiredStatus { get; set; }
        public string DeliveryDate { get; set; }
        public decimal Amount { get; set; }
        public string UserID { get; set; }
        public string SPStatus { get; set; }
        public string CustomerStatus { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
    }
    public class HireViewModel
    {
        [Required(ErrorMessage = "Please enter ProposalId")]
        public long ProposalId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class EditProposalVM
    {
        [Required(ErrorMessage = "Please enter ProposalId")]
        public long ProposalId { get; set; }
    }
    public class ReHireVM
    {
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter ToUserId")]
        public string ToUserId { get; set; }
    }
    public class AcceptRejectProposalViewModel
    {
        [Required(ErrorMessage = "Please enter Proposal Id")]
        public long ProposalId { get; set; }
        [Required(ErrorMessage = "Please enter Proposal Type")]
        public string ProposalType { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class CompletedDisputeProposalVM
    {
        [Required(ErrorMessage = "Please enter Proposal Id")]
        public long ProposalId { get; set; }
        [Required(ErrorMessage = "Please enter Proposal Type")]
        public string ProposalType { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter Status")]
        public string ProposalStatus { get; set; }
    }
    public class SaveProposalModel
    {
        [Required(ErrorMessage = "Please enter Proposal ID")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter JobID")]
        public long JobID { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter Amount")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
        [Required(ErrorMessage = "Please enter Job Expire Date")]
        public DateTime JobExpireDate { get; set; }
        [Required(ErrorMessage = "Please enter Duration")]
        public int? Duration { get; set; }
        [Required(ErrorMessage = "Please enter Start Date")]
        public DateTime? StartDate { get; set; }
    }
    public class NewOrderModel
    {
        public long ID { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? ReceivedBid { get; set; }
        public DateTime Date { get; set; }
    }
    public class SavedOrderModel
    {
        public long ID { get; set; }
        public string JobTitle { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ExpireOrderModel
    {
        public long ID { get; set; }
        public string JobTitle { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class CurrentOrderModel
    {
        public long ID { get; set; }
        public long ServiceID { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? ReceivedBid { get; set; }
        public DateTime Date { get; set; }
    }
    public class ProposalListModel
    {
        public List<ProposalModel> ProposalList { get; set; }
        public List<ProposalModel> LockedProposalList { get; set; }
        public List<ProposalModel> InvitedProposalList { get; set; }
    }
    public class ProposalUserModel
    {
        public long ID { get; set; }
        public long ProposalID { get; set; }
        public long JobID { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string UserID { get; set; }
        public string HiredStatus { get; set; }
        public string SPStatus { get; set; }
        public string CustomerStatus { get; set; }
        public bool IsUnlimitedBidView { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
    }
    public class SearchOrderModel
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please enter UserType")]
        public string UserType { get; set; }

        [Required(ErrorMessage = "Please enter GigType")]
        public string GigType { get; set; }
        public string SearchKeyword { get; set; } = "";
    }

    public class MyBidListModel
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please enter UserType")]
        public string UserType { get; set; }

        [Required(ErrorMessage = "Please enter GigType")]
        public string BidType { get; set; }
        public string SearchKeyword { get; set; } = "";
    }
    public class HiredUserModel
    {
        public long ID { get; set; }
        public long ProposalID { get; set; }
        public long JobID { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string HiredStatus { get; set; }
        public string SPStatus { get; set; }
        public string CustomerStatus { get; set; }
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
    }
    public class ActivityModel
    {
        public long ID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ActivityType { get; set; }
        public string RedirectPath { get; set; }
        public bool IsNotification { get; set; }
        public DateTime Date { get; set; }
    }
    public class GetSavedOrderVM
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        public string SearchKeyword { get; set; } = "";
    }
}