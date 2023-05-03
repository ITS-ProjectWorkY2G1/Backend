using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models.AuthModels.ViewModels.Auth
{
    public class AuthorizeViewModel
    {
        [Display(Name = "Application")]
        public string ApplicationName { get; set; } = string.Empty;

        [Display(Name = "Scope")]
        public string Scope { get; set; } = string.Empty;
    }
}
