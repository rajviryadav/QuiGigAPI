using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.ViewModel
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class DeleteViewModel
    {
        [Required(ErrorMessage = "Please enter ID")]
        public long ID { get; set; }
    }
    public class DeleteDetailViewModel
    {
        [Required(ErrorMessage = "Please enter ID")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class UserServiceList
    {
        [Required(ErrorMessage = "Please enter ServiceId")]
        public long ServiceId { get; set; }
        [Required(ErrorMessage = "Please enter IsSelected")]
        public bool IsSelected { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }

    public class UserLocationList
    {
        [Required(ErrorMessage = "Please enter StateId")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Please enter CityId")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Please enter IsSelected")]
        public bool IsSelected { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }  

    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please upload file")]
        public HttpPostedFileBase FileName { get; set; }
    }

    public class UserAddressViewModel
    {
        public long ID { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string CityWithState { get; set; }
    }
}