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
    
    public partial class CurrencyDefination
    {
        public long ID { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyValue { get; set; }
        public int QuigsValue { get; set; }
        public Nullable<int> BonusCoin { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedByID { get; set; }
    }
}