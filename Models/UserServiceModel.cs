using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class UserServiceModel
    {
        public List<UserServiceVM> ServiceIDList { get; set; }
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserID { get; set; }
    }
    public class UserServiceVM
    {
        public long ServiceID { get; set; }
    }   
    public class UserChildServiceList
    {
        public string Name { get; set; }
        public long? ID { get; set; }
    }
    public class DeleteUserService
    {
        [Required(ErrorMessage = "Please enter UserID")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter ID")]
        public long? ID { get; set; }
    }
}