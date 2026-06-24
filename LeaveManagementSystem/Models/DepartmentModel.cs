using System;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }

}

