using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    public class ApplyLeaveController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ApplyLeaveController(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
        }

        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("EmployeeId");
            return string.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId);
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (GetCurrentUserId() == 0)
                return RedirectToAction("Login", "Account");

            ViewBag.LeaveTypes = new[] {"Sick Leave","Vacation","Personal Leave","Maternity Leave","Paternity Leave","Bereavement Leave"};
            return View(new LeaveRequestModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LeaveRequestModel leave)
        {
            if (leave == null)
            {
                TempData["Error"] = "Invalid form data";
                return RedirectToAction("Index");
            }

            var leaveTypes = new[] {"Sick Leave","Vacation","Personal Leave","Maternity Leave","Paternity Leave","Bereavement Leave"};
            ViewBag.LeaveTypes = leaveTypes;

            ModelState.Remove("EmployeeName");
            ModelState.Remove("Status");
            ModelState.Remove("ApprovedByName");
            ModelState.Remove("Remarks");
            ModelState.Remove("ApprovedBy");
            ModelState.Remove("LeaveId");
            ModelState.Remove("AppliedDate");
            ModelState.Remove("EmployeeId");
            ModelState.Remove("TotalDays");

            leave.EmployeeId = GetCurrentUserId();
            leave.Status = "Pending";
            leave.AppliedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _leaveRepository.ApplyLeave(leave);
                    if (result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return RedirectToAction("Index", "LeaveHistory");
                    }
                    ModelState.AddModelError("", string.IsNullOrEmpty(result.Message) ? "Failed to apply leave. Please try again." : result.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error applying leave: " + ex.Message);
                }
            }
            return View(leave);
        }
    }
}