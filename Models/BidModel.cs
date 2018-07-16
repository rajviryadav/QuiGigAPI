using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class MyBidModel
    {
        public long JobID { get; set; }
        public string JobTitle { get; set; }
        public long ProposalId { get; set; }
        public int? ReceivedBid { get; set; }
        public string ServicePic { get; set; }
        public string ProposalStatus { get; set; }
        public string JobExpireDate { get; set; }
        public decimal Amount { get; set; }
        public string CreatedDate { get; set; }
    }

    public class MyGigModel
    {
        public long ID { get; set; }
        public long? ProposalId { get; set; }
        public string JobTitle { get; set; }
        public string ServicePic { get; set; }
        public int? ReceivedBid { get; set; }
        public string CreatedDate { get; set; }
        public string JobExpireDate { get; set; }
        public int? CurrentProposalStatus { get; set; }
    }
}