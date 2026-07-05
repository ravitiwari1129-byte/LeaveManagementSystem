using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
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

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "Employee";
        }


        private bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }



        // ==================== DASHBOARD SCREEN ====================

        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var role = GetCurrentUserRole();
            var userName = HttpContext.Session.GetString("EmployeeName") ?? "User";

            DashboardModel dashboard = _leaveRepository.GetDashboardData(role, userId);

            ViewBag.Dashboard = dashboard;
            ViewBag.UserName = userName;
            ViewBag.UserRole = role;

            return View("~/Views/Dashboard/Index.cshtml");
        }



        // ==================== REPORTS SCREEN ====================

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var role = GetCurrentUserRole();
                var userName = HttpContext.Session.GetString("EmployeeName") ?? "User";
                var employees = _employeeRepository.GetAllEmployees(null, role, userName);
                ViewBag.Employees = employees;
                ViewBag.UserRole = role;
                ViewBag.UserName = userName;
                return View();
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Error loading reports: " + ex.Message;
                return View();
            }
        }



        // ==================== LEAVE REPORT (AJAX) ====================

        [HttpPost]
        public IActionResult GetLeaveReport(int[] employeeId,string[] status,DateTime? fromDate,DateTime? toDate)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var employeeNames = new List<string>();
                if (employeeId != null && employeeId.Any())
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
                var leaves = _leaveRepository.SearchLeaves(employeeNames, status?.ToList(), fromDate,toDate,GetCurrentUserRole(),GetCurrentUserId());
                bool isAllSelected = status != null && status.Any(s => s.Equals("All", StringComparison.OrdinalIgnoreCase));
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