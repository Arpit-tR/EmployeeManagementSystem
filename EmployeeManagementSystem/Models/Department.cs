using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }
    }
}