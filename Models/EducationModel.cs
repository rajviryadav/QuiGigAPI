using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SaveEducationModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter Degree")]
        public string Degree { get; set; }
        [Required(ErrorMessage = "Please enter School")]
        public string School { get; set; }
        [Required(ErrorMessage = "Please enter Education Level")]
        public string EducationLevel { get; set; }
        [Required(ErrorMessage = "Please enter ToYear")]
        public string ToYear { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
    }   
    public class EducationModel
    {
        public long ID { get; set; }
        public string Degree { get; set; }
        public string School { get; set; }
        public string ToYear { get; set; }
        public string EducationLevel { get; set; }
    }
}