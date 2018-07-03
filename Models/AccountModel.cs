using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Please enter name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
        public long ServiceId { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string UserRole { get; set; }
    }
    public class ConfirmEmailModel
    {
        [Required(ErrorMessage = "Please enter Code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class SocialModel
    {
        [Required(ErrorMessage = "Please enter IpAddress")]
        public string IpAddress { get; set; }
    }
    public class LocationDetailModel
    {
        public string region_name { get; set; }
        public string city { get; set; }
        public string country_name { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Please enter old password")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter userid")]
        public string UserId { get; set; }
    }
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter email address")]
        public string Email { get; set; }
    }
    public class UserInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public string Email { get; set; }
        public long TimeZoneID { get; set; }
    }

}