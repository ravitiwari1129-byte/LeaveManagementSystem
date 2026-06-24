

using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveController(ILeaveRepository leaveRepository, IEmployeeRepository employeeRepository)
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



        /// <summary>
        /// //

        [HttpGet]
        public IActionResult Index()
        {
            // Check if user is logged in
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user is Admin
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            try
            {
                // Get all pending leaves
                var pendingLeaves = _leaveRepository.SearchLeaves(
                    null,
                    new List<string> { "Pending" },
                    null,
                    null,
                    "Admin",
                    null);

                // If no data, return empty list
                if (pendingLeaves == null)
                {
                    pendingLeaves = new List<LeaveRequestModel>();
                }

                // Pass to view
                return View(pendingLeaves);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading pending leaves: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }



        /// </summary>
        /// <returns></returns>
        /// 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveReject([FromBody] ApproveRejectRequest request)
        {
            try
            {
                if (!IsAdmin())
                    return Json(new { success = false, message = "Unauthorized" });

                if (request == null || request.LeaveId <= 0)
                    return Json(new { success = false, message = "Invalid leave request ID" });

                if (string.IsNullOrEmpty(request.Status) || (request.Status != "Approved" && request.Status != "Rejected"))
                    return Json(new { success = false, message = "Invalid status" });

                var repoResult = _leaveRepository.ApproveRejectLeave(
                    request.LeaveId,
                    request.Status,
                    GetCurrentUserId(),
                    request.Remarks
                );

                if (repoResult.Success)
                {
                    return Json(new { success = true, message = repoResult.Message });
                }
                else
                {
                    return Json(new { success = false, message = repoResult.Message ?? "Failed to process" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet]
        public IActionResult LeaveDetail(int id)
        {
            try
            {
                var leave = _leaveRepository.GetLeaveById(id);
                if (leave == null)
                {
                    TempData["Error"] = "Leave request not found";
                    return RedirectToAction("Index", "LeaveHistory");
                }

                // Check if user has access to this leave
                if (!IsAdmin() && leave.EmployeeId != GetCurrentUserId())
                    return RedirectToAction("AccessDenied", "Account");

                return View(leave);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading details: " + ex.Message;
                return RedirectToAction("Index", "LeaveHistory");
            }
            //var leave = _leaveRepository.GetLeaveById(id);

            //if (leave == null)
            //{
            //    return NotFound();
            //}

            //return View(leave);
        }

      



        // ========================================
        // APPLY LEAVE
        // ========================================

        [HttpGet]
        public IActionResult Apply()
        {
            if (GetCurrentUserId() == 0)
                return RedirectToAction("Login", "Account");

            ViewBag.LeaveTypes = new[] {
                "Sick Leave",
                "Vacation",
                "Personal Leave",
                "Maternity Leave",
                "Paternity Leave",
                "Bereavement Leave"
            };

            return View(new LeaveRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(LeaveRequestModel leave)
        {
            var leaveTypes = new[] {
                "Sick Leave",
                "Vacation",
                "Personal Leave",
                "Maternity Leave",
                "Paternity Leave",
                "Bereavement Leave"
            };
            ViewBag.LeaveTypes = leaveTypes;

            // Remove fields that shouldn't be validated from user input
            ModelState.Remove("EmployeeName");
            ModelState.Remove("Status");
            ModelState.Remove("ApprovedByName");
            ModelState.Remove("Remarks");
            ModelState.Remove("ApprovedBy");
            ModelState.Remove("LeaveId");
            ModelState.Remove("AppliedDate");
            ModelState.Remove("EmployeeId");
            ModelState.Remove("TotalDays");

            // Set values from session
            leave.EmployeeId = GetCurrentUserId();
            leave.Status = "Pending";
            leave.AppliedDate = DateTime.Now;

            // Custom validations
            if (leave.FromDate == default(DateTime))
                ModelState.AddModelError("FromDate", "From date is required");
            else if (leave.FromDate < DateTime.Today)
                ModelState.AddModelError("FromDate", "Cannot apply for past dates");

            if (leave.ToDate == default(DateTime))
                ModelState.AddModelError("ToDate", "To date is required");
            else if (leave.FromDate > leave.ToDate)
                ModelState.AddModelError("ToDate", "To date must be greater than or equal to from date");

            if (string.IsNullOrEmpty(leave.LeaveType))
                ModelState.AddModelError("LeaveType", "Leave type is required");

            if (string.IsNullOrEmpty(leave.Reason))
                ModelState.AddModelError("Reason", "Reason is required");

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _leaveRepository.ApplyLeave(leave);
                    if (result.Success)
                    {
                        TempData["Success"] = string.IsNullOrEmpty(result.Message) ? "Leave applied successfully!" : result.Message;
                        return RedirectToAction("History");
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

        // ========================================
        // LEAVE HISTORY
        // ========================================

        [HttpGet]
        public IActionResult History()
        {
            try
            {
                var leaves = _leaveRepository.SearchLeaves(
                    null,
                    null,
                    null,
                    null,
                    GetCurrentUserRole(),
                    GetCurrentUserId()
                );
                return View(leaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading history: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }

        // ========================================
        // APPROVE LEAVE VIEW (Admin Only)
        // ========================================

        [HttpGet]
        public IActionResult ApproveLeave()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            try
            {
                var statuses = new List<string> { "Pending" };
                var pendingLeaves = _leaveRepository.SearchLeaves(
                    null,
                    statuses,
                    null,
                    null,
                    "Admin",
                    null
                );
                return View(pendingLeaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading pending leaves: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }


     

        // ========================================
        // SEARCH LEAVES
        // ========================================

        [HttpGet]
        public IActionResult Search() => View();

        [HttpPost]
        public IActionResult SearchLeaves(string employeeName, string status, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var employeeNames = string.IsNullOrEmpty(employeeName)
           ? null
           : new List<string> { employeeName };

                var statuses = string.IsNullOrEmpty(status)
                    ? null
                    : new List<string> { status };
                var leaves = _leaveRepository.SearchLeaves(
                    employeeNames,
                    statuses,
                    fromDate,
                    toDate,
                    GetCurrentUserRole(),
                    GetCurrentUserId()
                );
                return Json(new { success = true, data = leaves ?? new List<LeaveRequestModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
