using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SaveCertificationModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter Granted By")]
        public string GrantedBy { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
    }
    public class GetCertificationModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string GrantedBy { get; set; }
    }
    public class CertificationModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string GrantedBy { get; set; }
    }
}