//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuiGigAPI.DataBase
{
    using System;
    
    public partial class MessageChat_Result2
    {
        public long JobID { get; set; }
        public string JobTitle { get; set; }
        public Nullable<bool> IsBlock { get; set; }
        public long GroupId { get; set; }
        public string BlockByID { get; set; }
        public string BlockBy { get; set; }
        public Nullable<System.DateTime> ToUserCreated { get; set; }
        public Nullable<System.DateTime> FromUserCreated { get; set; }
        public string ToPhoneNo { get; set; }
        public string ToEmail { get; set; }
        public string FromPhoneNo { get; set; }
        public string FromEmail { get; set; }
        public Nullable<int> MyGigsCount { get; set; }
        public Nullable<int> MyBidsCount { get; set; }
        public Nullable<int> ThumbsUp { get; set; }
        public Nullable<int> ThumbsDown { get; set; }
        public string JobCompleted { get; set; }
        public string HiredStatus { get; set; }
        public string ToUserName { get; set; }
        public string FromUserImage { get; set; }
        public string ToUserImage { get; set; }
        public string FromUserName { get; set; }
    }
}
