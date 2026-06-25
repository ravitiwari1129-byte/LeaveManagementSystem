

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

        public LeaveController(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
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
            var result = _leaveRepository.ApplyLeave(leave);

            if (result.Success)
            {
                return RedirectToAction("History");
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
