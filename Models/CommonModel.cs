using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SearchServiceModel
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
    public class SearchCityModel
    {
        public long ID { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }

    public class OrderFormFileUploadModel
    {
        public string OriginalName { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public string AttachmentExt { get; set; }
        public string AttachmentType { get; set; }
        public int AttachmentSize { get; set; }
    }

}