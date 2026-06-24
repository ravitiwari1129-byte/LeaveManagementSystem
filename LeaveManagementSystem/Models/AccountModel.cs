//using System;
//using System.ComponentModel.DataAnnotations;


//namespace LeaveManagementSystem.Models
//{
//    public class AccountModel
//    {
//    }

//    public class DashboardModel
//    {
//        public int PendingCount { get; set; }
//        public int ApprovedCount { get; set; }
//        public int RejectedCount { get; set; }
//        public int MonthlyCount { get; set; }
//        public int TotalDaysTaken { get; set; }
//        public int PendingLeaves { get; set; }
//        public int ApprovedLeaves { get; set; }
//        public int RejectedLeaves { get; set; }
//    }

//    public class ErrorViewModel
//    {
//        public string RequestId { get; set; }
//        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
//        public string ErrorMessage { get; set; }
//        public string StackTrace { get; set; }
//    }

//    public class LoginModel
//    {
//        [Required(ErrorMessage = "Email is required")]
//        [EmailAddress(ErrorMessage = "Invalid email format")]
//        public string Email { get; set; }

//        [Required(ErrorMessage = "Password is required")]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }
//    }

//    public class UserSession
//    {
//        public int EmployeeId { get; set; }
//        public string EmployeeName { get; set; }
//        public string Email { get; set; }
//        public string Role { get; set; }
//        public int? DepartmentId { get; set; }
//    }

//    public class SignupModel
//    {
//        [Required(ErrorMessage = "Full name is required")]
//        [Display(Name = "Full Name")]
//        public string FullName { get; set; }

//        [Required(ErrorMessage = "Email is required")]
//        [EmailAddress(ErrorMessage = "Invalid email format")]
//        public string Email { get; set; }

//        [Required(ErrorMessage = "Password is required")]
//        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }

//        [Required(ErrorMessage = "Confirm password is required")]
//        [Compare("Password", ErrorMessage = "Passwords do not match")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm Password")]
//        public string ConfirmPassword { get; set; }
//        public string Role { get; set; } = "Employee";
//    }

//}











  

