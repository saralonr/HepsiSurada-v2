using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HepsiBuradaBot.Models
{
    public class ApiResponse
    {
        public object Data { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
    }
}