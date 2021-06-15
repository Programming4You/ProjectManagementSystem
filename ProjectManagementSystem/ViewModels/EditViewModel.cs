using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystem.ViewModels
{
    public class EditViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public static IEnumerable<SelectListItem> Roles { set; get; }
    }
}
