

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


        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "Employee";
        }

        private bool CanApproveLeave()
        {
            var role = GetCurrentUserRole();
            return role == "Admin" || role == "Manager";
        }

        // ==================== APPLY LEAVE (Employee) ====================

        [HttpGet]
        public IActionResult Apply()
        {
            if (GetCurrentUserId() == 0)
                return RedirectToAction("Login", "Account");

            ViewBag.LeaveTypes = _leaveRepository.GetLeaveTypes();
            return View(new LeaveRequestModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(LeaveRequestModel leave)
        {
            if (leave == null)
            {
                TempData["Error"] = "Invalid form data";
                return RedirectToAction("Apply");
            }

            ViewBag.LeaveTypes = _leaveRepository.GetLeaveTypes();

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


            // ========== Date Validation ==========

            if (leave.FromDate > leave.ToDate)
            {
                ModelState.AddModelError("ToDate", "To date cannot be before from date");
            }
            if (leave.FromDate < DateTime.Today)
            {
                ModelState.AddModelError("FromDate", "Leave cannot be applied for past dates");
            }
            if (leave.ToDate < DateTime.Today)
            {
                ModelState.AddModelError("ToDate", "Leave cannot be applied for past dates");
            }

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


        // ==================== PENDING APPROVALS (Admin) ====================

        [HttpGet]
        public IActionResult Index()
        {
            if (GetCurrentUserId() == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!CanApproveLeave())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            try
            {
                var pendingLeaves = _leaveRepository.SearchLeaves(null,new List<string> { "Pending" },null,null, GetCurrentUserRole(),GetCurrentUserId());
                if (pendingLeaves == null)
                {
                    pendingLeaves = new List<LeaveRequestModel>();
                }
                return View(pendingLeaves);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading pending leaves: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }


        // ==================== APPROVE/REJECT ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveReject([FromBody] ApproveRejectRequest request)
        {
            try
            {
                if (!CanApproveLeave())
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }
                if (request == null || request.LeaveId <= 0)
                {
                    return Json(new { success = false, message = "Invalid leave request ID" });
                }
                if (string.IsNullOrEmpty(request.Status) || (request.Status != "Approved" && request.Status != "Rejected"))
                {
                    return Json(new { success = false, message = "Invalid status" });
                }

                var repoResult = _leaveRepository.ApproveRejectLeave(request.LeaveId,request.Status,GetCurrentUserId(),request.Remarks);
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


        // ==================== LEAVE DETAILS ====================

        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var leave = _leaveRepository.GetLeaveById(id);
                if (leave == null)
                {
                    TempData["Error"] = "Leave request not found";
                    return RedirectToAction("Index", "LeaveHistory");
                }
                if (!CanApproveLeave() && leave.EmployeeId != GetCurrentUserId())
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                return View(leave);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading details: " + ex.Message;
                return RedirectToAction("Index", "LeaveHistory");
            }
        }



        // ==================== LEAVE HISTORY ====================

        [HttpGet]
        public IActionResult History()
        {
            try
            {
                var leaves = _leaveRepository.SearchLeaves(null,null,null,null,GetCurrentUserRole(),GetCurrentUserId());
                return View(leaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading history: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }


        [HttpGet]
        public IActionResult ApproveLeave()
        {
            if (!CanApproveLeave())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            try
            {
                var statuses = new List<string> { "Pending" };
                var pendingLeaves = _leaveRepository.SearchLeaves(null,statuses,null,null, GetCurrentUserRole(),GetCurrentUserId());
                return View(pendingLeaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading pending leaves: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }



        // ==================== SEARCH LEAVE (AJAX) ====================

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchLeaves(List<string> employeeNames,List<string> statuses, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                if (statuses != null && statuses.Contains("All"))
                {
                    statuses = null;
                }

                var leaves = _leaveRepository.SearchLeaves(employeeNames, statuses, fromDate, toDate, GetCurrentUserRole(), GetCurrentUserId());
                
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

                return Json(new { success = true, data = result});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
