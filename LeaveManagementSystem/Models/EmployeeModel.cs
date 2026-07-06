using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        [Display(Name = "Employee Name")]
        [RegularExpression(@"^[A-Z][a-z]+(?: [A-Z][a-z]+)*$", ErrorMessage = "Each word must start with a capital letter and contain only alphabets")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{5,20}$", ErrorMessage = "Password must contain uppercase letter, number and special character")]
        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string ProfileImage { get; set; }

        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - DateOfBirth.Year;
                if (DateOfBirth > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }

    }

    public class DashboardModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int MonthlyCount { get; set; }
        public int TotalDaysTaken { get; set; }
        public int PendingLeaves { get; set; }
        public int ApprovedLeaves { get; set; }
        public int RejectedLeaves { get; set; }
    }

}

