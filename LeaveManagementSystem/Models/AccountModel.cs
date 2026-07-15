using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LeaveManagementSystem.Models
{

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{5,20}$", ErrorMessage = "Password must contain uppercase letter, number and special character")]
        public string Password { get; set; }
    }

    public class UserSession
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class SignupModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50 characters")]
        [RegularExpression(@"^[A-Z][a-z]+(?: [A-Z][a-z]+)*$", ErrorMessage = "Each word must start with a capital letter and contain only alphabets")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10 digit Mobile Number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{5,20}$", ErrorMessage = "Password must contain uppercase letter, number and special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Confirm Password must be between 5 and 20 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{5,20}$", ErrorMessage = "Password must contain uppercase letter, number and special character")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(1000, 1000000, ErrorMessage = "Salary must be between 1000 and 1000000")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Joining Date is required")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        public List<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        public List<SelectListItem> RoleList { get; set; } = new();

        public List<string> Skills { get; set; } = new List<string>();

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 300 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Profile Image is required")]
        [Display(Name = "Profile Image")]
        public IFormFile ProfileImage { get; set; }

    }


    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }

}

