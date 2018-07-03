using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.ViewModel
{
    public class SearchServiceViewModel
    {
        [Required(ErrorMessage = "Please enter service name")]
        public string ServiceName { get; set; }
    }

    public class CityViewModel
    {
        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }
    }

    public class ServicesNearMeViewModel
    {
        [Required(ErrorMessage = "Please enter service id")]
        public long ServiceId { get; set; }
        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }
    }
    public class QuestionTypeViewModel
    {
        public string TypeName { get; set; }
        public string UniqueCode { get; set; }
    }

    public class YearMonthModel
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}