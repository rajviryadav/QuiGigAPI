using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class UserDetailModel
    {
        public string UserName { get; set; }
        public string UserShortName { get; set; }
        public string ProfilePic { get; set; }
        public string ProfessionalDescription { get; set; }
        public string ProfessionalTitle { get; set; }
        public string CurrentPlan { get; set; }
        public int BidPoints { get; set; }
        public int Percentage { get; set; }
        public int ThumbsDown { get; set; }
        public int ThumbsUp { get; set; }
    }

    public class UserAddressModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter Location")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Please enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter PhoneNo")]
        public string PhoneNo { get; set; }
        [Required(ErrorMessage = "Please enter Postal Code")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Please enter CountryId")]        
        public int? CountryID { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        public string Country { get; set; }
    }
    public class AboutUserModel
    {
        [Required(ErrorMessage = "Please enter Professional Title")]
        public string ProfessionalTitle { get; set; }
        [Required(ErrorMessage = "Please enter Professional Description")]
        public string ProfessionalDescription { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserName { get; set; }
     
    }    
    public class ChangeEmailModel
    {
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }

    }
    public class UpdateProfileModel
    {
        [Required(ErrorMessage = "Please enter Current Role")]
        public string CurrentRole { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }

    }
    public class ReviewRatingModel
    {
        public string Review { get; set; }
        public int Rating { get; set; }
        public bool ThumbsValue { get; set; }
        public string WorkAgain { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }
        public string UserImage { get; set; }
    }
    public class LocationModel
    {
        public string StateCode { get; set; }
        public long StateId { get; set; }
        public string StateName { get; set; }
        public List<ProfileLocationCityVM> CityList { get; set; }
    }
    public class ProfileLocationCityVM
    {
        public string CityName { get; set; }
        public long CityId { get; set; }
        public bool Selected { get; set; }
    }

    public class UserLocationAddressViewModel
    {
        [Required(ErrorMessage = "Please enter User Location")]
        public List<UserLocationVM> UserLocation { get; set; }
    }

    public class UserLocationVM
    {       
        public int? CityId { get; set; }
        public int? StateId { get; set; }
    }

    public class UserLocationDeleteModel
    {
        [Required(ErrorMessage = "Please enter CityId")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
    }
}