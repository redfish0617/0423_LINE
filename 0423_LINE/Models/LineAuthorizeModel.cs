using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _0423_LINE.Models
{
    public class LineAuthorizeModel
    {
        [Required]
        public string response_type { set; get; }
        [Required]
        public string client_id { set; get; }
        [Required]
        public string redirect_uri { set; get; }
        [Required]
        public string scope { set; get; }
        [Required]
        public string state { set; get; }
        [Required]
        public string responsemode { set; get; }
    }
}