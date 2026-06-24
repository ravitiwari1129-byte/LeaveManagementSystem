//using System;
//using System.ComponentModel.DataAnnotations;

//namespace LeaveManagementSystem.Models
//{
//    public class LeaveModel
//    {
//        public int LeaveId { get; set; }

//        [Required(ErrorMessage = "Employee is required")]
//        public int EmployeeId { get; set; }
//        public string EmployeeName { get; set; }

//        [Required(ErrorMessage = "Leave type is required")]
//        [Display(Name = "Leave Type")]
//        public string LeaveType { get; set; }

//        [Required(ErrorMessage = "From date is required")]
//        [DataType(DataType.Date)]
//        [Display(Name = "From Date")]
//        public DateTime FromDate { get; set; }

//        [Required(ErrorMessage = "To date is required")]
//        [DataType(DataType.Date)]
//        [Display(Name = "To Date")]
//        public DateTime ToDate { get; set; }

//        [Required(ErrorMessage = "Reason is required")]
//        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
//        public string Reason { get; set; }
//        public string Status { get; set; }
//        public DateTime AppliedDate { get; set; }
//        public int? ApprovedBy { get; set; }
//        public string ApprovedByName { get; set; }
//        public string Remarks { get; set; }
//    }


//    public class ApproveRejectRequest
//    {
//        public int LeaveId { get; set; }
//        public string Status { get; set; }  
//        public string Remarks { get; set; }
//    }

//    public class DeleteRequest
//    {
//        public int Id { get; set; }
//    }
//}



