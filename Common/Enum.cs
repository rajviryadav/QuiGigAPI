using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Common
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        InProgress,
    }
    public enum PaymentFromSite
    {
        ICO,
        QuiGig,
        Coinbase,
        Indiegogo
    }
    public enum UserRoleEnum
    {
        SuperAdmin,
        SubAdmin,
        ServiceProvider,
        Customer
    }
    public enum HireStatus
    {
        Pending,
        Accepted,
        Decline,
        Canceled,
        Completed,
        Dispute
    }
    public enum ProposalStatus
    {
        Pending,
        Accepted,
        Decline,
        Completed,
        Dispute
    }
    public enum NotificationEnum
    {
        MESSAGE,
        PROJECT_REMINDER,
        PROMOTIONS,
        ALL_TEXT_NOTIFICATION,
        RECOMMENDATION,
        REMINDER,
        MATCHING_ORDERS,
        JOB_COMPLETE,
        HIRING,
        PLAN_REMINDER,
        PROMOTIONAL,
        RECEIVED_BID,
        RECOMMENDED,
        DISPUTE,
        NEW_BID
    }
    public enum NotificationCategoryEnum
    {
        EMAIL_NOTIFICATION,
        TEXT_NOTIFICATION,
        PUSH_NOTIFICATION
    }
    public enum ValueTypeEnum
    {
        Hourly,
        Monthly,
        Yearly,
        Days,
        Percentage
    }
    public enum ActivityType
    {
        MatchingOrder,
        Hire,
        PostedJob,
        Accepted,
        Completed,
        Canceled,
        Dispute
    }
    public enum ParameterUniqueNameEnum
    {
        BID_TIMER,
        ORDER_EXPIRE
    }
    public enum ProfileParameterEnum
    {
        SIGNUP,
        EMAIL_VERIFICATION,
        PROFILE_PIC
    }
}