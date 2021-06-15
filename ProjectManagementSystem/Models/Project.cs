using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagementSystem.Models
{
    public class Project
    {
        [Key]
        [Display(Name = "Project Code")]
        public int ProjectCode { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public string ProjectManagerId { get; set; }

        public ApplicationUser ProjectManager { get; set; }

        public List<Task> Tasks { get; set; }

    }
}
