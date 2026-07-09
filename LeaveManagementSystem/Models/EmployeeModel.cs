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

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10 digit Mobile Number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(1000, 1000000, ErrorMessage = "Salary must be between 1000 and 1000000")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Joining Date is required")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 300 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Select at least one Skill")]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Profile Image is required")]
        [Display(Name = "Profile Image")]
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

