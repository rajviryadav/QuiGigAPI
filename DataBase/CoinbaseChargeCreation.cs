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
    using System.Collections.Generic;
    
    public partial class CoinbaseChargeCreation
    {
        public long ID { get; set; }
        public string code { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> expires_at { get; set; }
        public string hosted_url { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string name { get; set; }
        public string price_bitcoin { get; set; }
        public string price_ethereum { get; set; }
        public string price_bitcoincash { get; set; }
        public string price_litecoin { get; set; }
        public string price_local { get; set; }
        public string Response { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}