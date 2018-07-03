using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.ViewModel
{
    public class SectionViewMOdel
    {
        [Required(ErrorMessage = "Please enter serviceId")]
        public long ServiceId { get; set; }
    }
    public class CheckValidZipCodeViewMOdel
    {
        [Required(ErrorMessage = "Please enter zipcode")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }
    }
}