using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveHistoryController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;

        public LeaveHistoryController(ILeaveRepository leaveRepository)
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

        private bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }


        // ==================== LEAVE HISTORY (Current User) ====================

        [HttpGet]
        public IActionResult Index()
        {
            if (GetCurrentUserId() == 0)
                return RedirectToAction("Login", "Account");
            try
            {
                var leaves = _leaveRepository.SearchLeaves(null, null, null, null, GetCurrentUserRole(), GetCurrentUserId());
                return View(leaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading history: " + ex.Message;
                return View(new List<LeaveRequestModel>());
            }
        }



        // ==================== LEAVE HISTORY (Admin viewing specific employee) ====================

        [HttpGet]
        public IActionResult History(int id)
        {
            if (GetCurrentUserId() == 0)
                return RedirectToAction("Login", "Account");

            if (!IsAdmin() && GetCurrentUserId() != id)
                return RedirectToAction("AccessDenied", "Account");

            try
            {
                var leaves = _leaveRepository.SearchLeaves(null, null, null, null, GetCurrentUserRole(), id);
                return View("Index", leaves ?? new List<LeaveRequestModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading history: " + ex.Message;
                return View("Index", new List<LeaveRequestModel>());
            }

        }
    }
}