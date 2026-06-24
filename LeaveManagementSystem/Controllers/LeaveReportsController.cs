using System;
using System.Collections.Generic;
using System.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveReportsController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public LeaveReportsController(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }
        private string GetCurrentUserRole() => HttpContext.Session.GetString("UserRole") ?? "Employee";
        private bool IsAdmin() => GetCurrentUserRole() == "Admin";
        [HttpGet]
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
            var employees = _employeeRepository.GetAllEmployees();
            ViewBag.Employees = employees;
            ViewBag.Statuses = new[] { "All", "Pending", "Approved", "Rejected" };
            return View();
        }

        [HttpPost]
        public IActionResult GetLeaveReport(int? employeeId, string status, DateTime? fromDate, DateTime? toDate)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                // Get employee name if employeeId is provided
                string employeeName = null;
                if (employeeId.HasValue && employeeId.Value > 0)
                {
                    var emp = _employeeRepository.GetEmployeeById(employeeId.Value);
                    employeeName = emp?.EmployeeName;
                }

                // Handle "All" status - pass null to SP
                List<string> statuses = null;

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    statuses = new List<string> { status };
                }

                var leaves = _leaveRepository.SearchLeaves(
                    string.IsNullOrEmpty(employeeName)
                        ? null
                        : new List<string> { employeeName },

                    statuses,

                    fromDate,
                    toDate,
                    "Admin",
                    employeeId
                );

                // Format response data
                var result = new List<object>();
                foreach (var leave in leaves)
                {
                    result.Add(new
                    {
                        leaveId = leave.LeaveId,
                        employeeName = leave.EmployeeName,
                        leaveType = leave.LeaveType,
                        fromDate = leave.FromDate.ToString("yyyy-MM-dd"),
                        toDate = leave.ToDate.ToString("yyyy-MM-dd"),
                        reason = leave.Reason,
                        status = leave.Status,
                        appliedDate = leave.AppliedDate.ToString("yyyy-MM-dd"),
                        approvedByName = leave.ApprovedByName ?? "",
                        remarks = leave.Remarks ?? "",
                    });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
