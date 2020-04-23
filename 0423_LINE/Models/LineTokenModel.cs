using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _0423_LINE.Models
{
    public class LineTokenModel
    {
        [Required]
        public string code { set; get; }
        [Required]
        public string client_id { get;  set; }
        [Required]
        public string client_secret { get;  set; }
        [Required]
        public string redirect_uri { get;  set; }
        [Required]
        public string grant_type { get;  set; }
    }
}