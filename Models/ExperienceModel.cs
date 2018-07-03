using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SaveExperienceModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter Company")]
        public string Company { get; set; }
        [Required(ErrorMessage = "Please enter FromMonth")]
        public string FromMonth { get; set; }
        [Required(ErrorMessage = "Please enter FromYear")]
        public string FromYear { get; set; }
        [Required(ErrorMessage = "Please enter ToMonth")]
        public string ToMonth { get; set; }
        [Required(ErrorMessage = "Please enter ToYear")]
        public string ToYear { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
        public bool CurrentlyWorking { get; set; }
        public string Description { get; set; }        
    }

    public class GetExperienceModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string FromPeriod { get; set; }
        public string ToPeriod { get; set; }
        public string CurrWorking { get; set; }
        public string Description { get; set; }
    }
    public class ExperienceModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string FromMonth { get; set; }
        public string FromYear { get; set; }
        public string ToMonth { get; set; }
        public string ToYear { get; set; }
        public bool CurrentlyWorking { get; set; }
        public string Description { get; set; }
    }
}