using System;
using System.Collections.Generic;
using System.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        private void LoadViewBagData()
        {
            ViewBag.Departments = _employeeRepository.GetDepartments();
            ViewBag.Roles = new[] { "Employee", "Manager", "Admin" };
        }



        // ==================== EMPLOYEE LIST ====================

        [HttpGet]
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");
            
            var employees = _employeeRepository.GetAllEmployees(null,null,null);
            LoadViewBagData();
            return View(employees);
        }



        // ==================== ADD EMPLOYEE ====================

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            LoadViewBagData();
            return View(new EmployeeModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeModel employee)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            ModelState.Remove("DepartmentName");
            ModelState.Remove("IsActive");
            ModelState.Remove("EmployeeId");
            if (employee.DepartmentId <= 0)
            {
                ModelState.AddModelError("DepartmentId", "Please select a valid department");
            }
            if (string.IsNullOrEmpty(employee.Role))
            {
                ModelState.AddModelError("Role", "Please select a role");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    int id = _employeeRepository.InsertEmployee(employee);

                    if (id > 0)
                    {
                        TempData["Success"] = "Employee added successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        string errorMessage = GetErrorMessage(id);
                        ModelState.AddModelError("", errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            LoadViewBagData();
            return View(employee);
        }



        // ==================== EDIT EMPLOYEE ====================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Index");
            }
            LoadViewBagData();
            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeModel employee)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            ModelState.Remove("DepartmentName");
            ModelState.Remove("IsActive");
            ModelState.Remove("Password");
            
            var existingEmployee = _employeeRepository.GetEmployeeById(employee.EmployeeId);

            if (existingEmployee != null && existingEmployee.Role == "Admin" && employee.Role != "Admin")
            {
                int adminCount = _employeeRepository.GetAdminCount();
                if (adminCount <= 1)
                {
                    ModelState.AddModelError("", "At least one Admin must exist in the system.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool result = _employeeRepository.UpdateEmployee(employee);
                    if(result)
                    {
                        TempData["Success"] = "Employee updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Employee update failed.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            LoadViewBagData();
            return View(employee);
        }



        // ==================== DELETE EMPLOYEE ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            try
            {
                if (!IsAdmin())
                    return Json(new { success = false, message = "Unauthorized" });

                if (request == null || request.Id <= 0)
                    return Json(new { success = false, message = "Invalid employee ID" });

                bool result = _employeeRepository.DeleteEmployee(request.Id);
                if (result)
                    return Json(new { success = true, message = "Employee deleted successfully" });
                else
                    return Json(new { success = false, message = "Employee not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ==================== GET EMPLOYEES (AJAX) ====================

        [HttpGet]
        public IActionResult GetEmployees(int? departmentId)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            var employees = _employeeRepository.GetAllEmployees(null,null,null);
            return Json(new { success = true, data = employees });
        }

        private string GetErrorMessage(int errorCode)
        {
            return errorCode switch
            {
                -1 => "Email already exists in the system",
                -2 => "Selected department is invalid",
                -3 => "Invalid role selected. Must be 'Admin', 'Manager' or 'Employee'",
                -4 => "Employee name cannot be empty",
                -5 => "Invalid email format",
                -6 => "Password must be at least 4 characters",
                _ => "Failed to add employee. Please try again."
            };
        }
    }
}
