using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public override string UserName { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Surname { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public List<Project> Projects { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
