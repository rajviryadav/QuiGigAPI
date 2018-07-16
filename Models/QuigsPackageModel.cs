using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuiGigAPI.Models
{
    public class QuigsPackageModel
    {
        public long ID { get; set; }
        public decimal QuigsAmount { get; set; }
        public decimal QuigsValue { get; set; }       
    }
}