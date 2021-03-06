using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagementSystem.ViewModels
{
    public class TaskViewModel
    {
        [Key]
        public int TaskID { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Required]
        [Display(Name = "Progress(%)")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public int Progress { get; set; }

        [Required]
        [Display(Name = "Deadline")]
        public DateTime Deadline { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string AssigneeId { get; set; }

        public ApplicationUser? Assignee { get; set; }

        public int ProjectCode { get; set; }

        public Project Project { get; set; }

        public static List<SelectListItem> Developers { set; get; }
        public static List<SelectListItem> Statuses { set; get; }
    }
}
