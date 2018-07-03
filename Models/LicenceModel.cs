using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SaveLicenceModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter Licence No")]
        public string LicenceNo { get; set; }
        [Required(ErrorMessage = "Please enter Service Name")]
        public string ServiceName { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
        [Required(ErrorMessage = "Please enter Issuer")]
        public string Issuer { get; set; }
        [Required(ErrorMessage = "Please enter ExpirationDate")]
        public string ExpirationDate { get; set; }
    }
    public class LicenceModel
    {
        public long ID { get; set; }
        public string LicenceNo { get; set; }
        public string ServiceName { get; set; }
        public string Issuer { get; set; }
        public string ExpirationDate { get; set; }
    }
}