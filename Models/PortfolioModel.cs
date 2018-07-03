using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class SavePortfolioModel
    {
        [Required(ErrorMessage = "Please enter Id")]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter Image")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Please enter userId")]
        public string UserID { get; set; }
    }
    public class PortfolioModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}