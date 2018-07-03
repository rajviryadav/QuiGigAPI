using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGig.Models
{
    public class OrderModel
    {
        public long ServiceID { get; set; }
        public long Section { get; set; }
        public string SecctionTitle { get; set; }
        public string ServiceName { get; set; }
        public List<OrderQuestionModel> QuestionList { get; set; }
    }
    public class OrderQuestionModel
    {
        public long QuestionID { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionImage { get; set; }
        public string LinerScaleLabel { get; set; }
        public bool IsRequired { get; set; }
        public long QuestionTypeID { get; set; }
        public string QuestionType_UniqueCode { get; set; }
        public List<QuestionOptionModel> QuestionOptionList { get; set; }
        public List<FileUploadModel> FileUploadList { get; set; }
    }

    public class QuestionOptionModel
    {
        public long OptionID { get; set; }
        public string OptionHeading { get; set; }
        public bool IsOther { get; set; }
        public string Placeholder { get; set; }
        public string OptionImage { get; set; }
    }
    public class FileUploadModel
    {
        public string FileSize { get; set; }
        public string AllowedFileNumber { get; set; }
        public string AllowedFileType { get; set; }
    }

    public class ServiceListModel
    {
        [Required(ErrorMessage = "Please enter userid")]
        public string UserId { get; set; }
        public long ReHireUserId { get; set; }
        [Required(ErrorMessage = "Please enter serviceId")]
        public long ServiceID { get; set; }
        public List<OptionListModel> OptionList { get; set; }
    }

    public class ServiceModel
    {
        public long ReHireUserId { get; set; }
        [Required(ErrorMessage = "Please enter serviceId")]       
        public long ServiceID { get; set; }
        [Required(ErrorMessage = "Please enter StateName")]
        public string StateName { get; set; }
        [Required(ErrorMessage = "Please enter CityName")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
        public List<OptionListModel> OptionList { get; set; }
    }

    public class UpdateJobModel
    {
        [Required(ErrorMessage = "Please enter JobId")]
        public long JobId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class OptionListModel
    {
        [Required(ErrorMessage = "Please enter OptionID")]
        public long OptionID { get; set; }
        public string OtherAnswer { get; set; }
        public List<FileUploadListModel> FileUploadList { get; set; }
    }
    public class FileUploadListModel
    {
        public long JobQuestionOptionsID { get; set; }
        public string OriginalName { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string AttachmentExt { get; set; }
        public string AttachmentType { get; set; }
        public decimal AttachmentSize { get; set; }
    }
    public class NoServiceFoundModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your city")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter your service name")]
        public string Service { get; set; }
    }   
    public class ExploreCategoriesModel
    {
        public string ParentService { get; set; }
        public List<ParentServiceListModel> ParentServiceList { get; set; }
        public List<ParentServiceListModel> ServiceList { get; set; }
    }
    public class ParentServiceListModel
    {
        public long ID { get; set; }
        public string ServiceName { get; set; }
    }    
   
    public class ExploreCitiesDetailModel
    {
        public long? ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceIcon { get; set; }
        public string Url { get; set; }
    }
    public class ExploreServiceDetailModel
    {
        public string ServiceName { get; set; }
        public string CityDescription { get; set; }
        public List<UserDetailModel> PopularSPList { get; set; }
        public List<OtherSPCityModel> SPAvailableOtherCity { get; set; }
    }
    public class UserDetailModel
    {
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
        public string CreatedDate { get; set; }
        public int? HiredCount { get; set; }
    }    
    public class OtherSPCityModel
    {
        public string CityName { get; set; }
    }
    public class StateViewModel
    {
        [Required(ErrorMessage = "Please enter StateId")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class StateModel
    {
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int ID { get; set; }
    }
    public class CityModel
    {
        public string CityName { get; set; }
        public int ID { get; set; }
        public bool IsSelected { get; set; }
    }
    public class CityViewModel
    {
        [Required(ErrorMessage = "Please enter CityName")]
        public string CityName { get; set; }
    }
    public class ServiceViewModel
    {
        public string ServiceName { get; set; }
    }
    public class CityServiceViewModel
    {
        [Required(ErrorMessage = "Please enter ServiceId")]
        public long ServiceId { get; set; }
        [Required(ErrorMessage = "Please enter CityName")]
        public string CityName { get; set; }
    }
    public class HomePageServiceListModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int OrderNo { get; set; }
    }
    public class HomePageChildServiceModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
    public class HomePageViewModel
    {
        [Required(ErrorMessage = "Please enter ParentId")]
        public long ParentId { get; set; }
        [Required(ErrorMessage = "Please enter UserId")]
        public string UserId { get; set; }
    }
    public class SearchCityModel
    {
        public long StateId { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string CityWithCode { get; set; }
    }
}