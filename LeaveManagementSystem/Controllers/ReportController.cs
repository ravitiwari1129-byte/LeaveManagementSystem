using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReportController(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }

        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("EmployeeId");
            return string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
        }

        private string GetCurrentUserRole() => HttpContext.Session.GetString("UserRole") ?? "Employee";
        private bool IsAdmin() => GetCurrentUserRole() == "Admin";

        // ========================================
        // DASHBOARD
        // ========================================
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var role = GetCurrentUserRole();
            var userName = HttpContext.Session.GetString("EmployeeName") ?? "User";

            var dashboard = _leaveRepository.GetDashboardCounts(role, userId);
            var summary = new DashboardModel();

            if (role != "Admin")
            {
                summary = _leaveRepository.GetLeaveSummary(userId);
            }

            ViewBag.Dashboard = dashboard;
            ViewBag.Summary = summary;
            ViewBag.UserName = userName;
            ViewBag.UserRole = role;

            return View("Index"); // ✅ Views/Report/Index.cshtml
        }

        // ========================================
        // LEAVE REPORT
        // ========================================
        [HttpGet]
        public IActionResult LeaveReport()
        {
            //if (!IsAdmin())
            //{
            //    return RedirectToAction("AccessDenied", "Account");
            //}

            try
            {
                var role = GetCurrentUserRole();
                var userName = HttpContext.Session.GetString("EmployeeName") ?? "User";
                var employees = _employeeRepository.GetAllEmployees();
                ViewBag.Employees = employees;
                ViewBag.userrole = role;
                return View(); // ✅ Views/Report/LeaveReport.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading leave report: " + ex.Message;
                return View();
            }
        }

        // ========================================
        // GET LEAVE REPORT DATA (AJAX)
        // ========================================
        public IActionResult GetLeaveReport(
    int[] employeeId,
    string[] status,
    DateTime? fromDate,
    DateTime? toDate)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var employeeNames = new List<string>();

                if (employeeId != null)
                {
                    foreach (var id in employeeId)
                    {
                        var emp = _employeeRepository.GetEmployeeById(id);

                        if (emp != null)
                        {
                            employeeNames.Add(emp.EmployeeName);
                        }
                    }
                }

                // Search all records first
                var leaves = _leaveRepository.SearchLeaves(
                    null,
                    null,
                    fromDate,
                    toDate,
                    "Admin",
                    null);

                // Filter employees
                if (employeeNames.Any())
                {
                    leaves = leaves
                        .Where(x => employeeNames.Contains(x.EmployeeName))
                        .ToList();
                }

                // Filter statuses
                bool isAllSelected = status != null &&
                                     status.Any(s => s.Equals("All", StringComparison.OrdinalIgnoreCase));

                if (!isAllSelected && status != null && status.Length > 0)
                {
                    leaves = leaves
                        .Where(x => status.Contains(x.Status))
                        .ToList();
                }

                return Json(new
                {
                    success = true,
                    data = leaves.Select(x => new
                    {
                        employeeName = x.EmployeeName,
                        leaveType = x.LeaveType,
                        fromDate = x.FromDate.ToString("yyyy-MM-dd"),
                        toDate = x.ToDate.ToString("yyyy-MM-dd"),
                        reason = x.Reason,
                        status = x.Status,
                        appliedDate = x.AppliedDate.ToString("yyyy-MM-dd"),
                        approvedByName = x.ApprovedByName ?? "",
                        remarks = x.Remarks ?? ""
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}