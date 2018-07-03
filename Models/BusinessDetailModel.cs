using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SaveBusinessDetailModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }        
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
        public string Description { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string YearEstablished { get; set; }
    }
    public class GetBusinessDetailModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string YearEstablished { get; set; }
        public bool IsActive { get; set; }
    }
    public class BusinessDetailModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string YearEstablished { get; set; }
        public bool IsActive { get; set; }
    }
}