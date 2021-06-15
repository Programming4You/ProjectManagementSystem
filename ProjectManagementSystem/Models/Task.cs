using System;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagementSystem.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        public int Progress { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public string AssigneeId { get; set; }

        public ApplicationUser Assignee { get; set; }

        public int ProjectCode { get; set; }

        public Project Project { get; set; }
    }
}
