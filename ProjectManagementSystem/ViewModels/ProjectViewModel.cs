using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagementSystem.ViewModels
{
    public class ProjectViewModel
    {
        [Key]
        [Display(Name = "Project Code")]
        public int ProjectCode { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Project Manager is required")]
        public string ProjectManagerId { get; set; }

        public ApplicationUser ProjectManager { get; set; }

        public static List<SelectListItem> ProjectManagers { set; get; }
    }
}
