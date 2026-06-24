using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    public class SearchLeavesController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public SearchLeavesController(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }

        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("EmployeeId");
            return string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
        }

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "Employee";
        }

        private bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchLeaves(List<string> employeeNames,List<string> statuses,DateTime? fromDate,DateTime? toDate)
        {
            Console.WriteLine("========== SEARCH DEBUG ==========");
            Console.WriteLine("Employee Names: " +
                (employeeNames == null ? "NULL" : string.Join(",", employeeNames)));

            Console.WriteLine("Statuses: " +
                (statuses == null ? "NULL" : string.Join(",", statuses)));
            try
            {
                if (statuses != null && statuses.Contains("All"))
                {
                    statuses = null;
                }
                var leaves = _leaveRepository.SearchLeaves(
                    employeeNames,
                    statuses,
                    fromDate,
                    toDate,
                    GetCurrentUserRole(),
                    GetCurrentUserId()
                );

                var result = leaves.Select(leave => new
                {
                    employeeName = leave.EmployeeName,
                    leaveType = leave.LeaveType,
                    fromDate = leave.FromDate.ToString("yyyy-MM-dd"),
                    toDate = leave.ToDate.ToString("yyyy-MM-dd"),
                    reason = leave.Reason,
                    status = leave.Status,
                    appliedDate = leave.AppliedDate.ToString("yyyy-MM-dd")
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}