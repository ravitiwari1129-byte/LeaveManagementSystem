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
            ViewBag.Roles = _employeeRepository.GetRoles();
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
            ModelState.Remove("ProfileImage");
            ModelState.Remove("ExistingProfileImage");
            if (employee.DepartmentId <= 0)
            {
                ModelState.AddModelError("DepartmentId", "Please select a valid department");
            }
            if (string.IsNullOrEmpty(employee.Role))
            {
                ModelState.AddModelError("Role", "Please select a role");
            }
            if (employee.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Profile Image is required.");
            }
            if (employee.DateOfBirth >= DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth must be before today.");
            }

            employee.Skills = employee.SelectedSkills != null ? string.Join(",", employee.SelectedSkills) : "";

            int age = DateTime.Today.Year - employee.DateOfBirth.Year;

            if (employee.DateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                ModelState.AddModelError("DateOfBirth", "Employee must be at least 18 years old.");
            }
            if (employee.JoiningDate < DateTime.Today)
            {
                ModelState.AddModelError("JoiningDate", "Joining Date cannot be a past date.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (employee.ImageFile != null)
                    {
                        var extension = Path.GetExtension(employee.ImageFile.FileName).ToLower();

                        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        {
                            ModelState.AddModelError("ImageFile", "Only JPG, JPEG and PNG files are allowed.");
                            LoadViewBagData();
                            return View(employee);
                        }

                        if (employee.ImageFile.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "Image size should be less than 2 MB.");
                            LoadViewBagData();
                            return View(employee);
                        }

                        string imageName = Guid.NewGuid().ToString() + extension;

                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        string path = Path.Combine(folder, imageName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            employee.ImageFile.CopyTo(stream);
                        }

                        employee.ProfileImage = imageName;
                    }
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
            ModelState.Remove("ProfileImage");
            ModelState.Remove("ExistingProfileImage");

            if (employee.DateOfBirth >= DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth must be before today.");
            }

            employee.Skills = employee.SelectedSkills != null ? string.Join(",", employee.SelectedSkills) : "";

            int age = DateTime.Today.Year - employee.DateOfBirth.Year;

            if (employee.DateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                ModelState.AddModelError("DateOfBirth", "Employee must be at least 18 years old.");
            }

            if (employee.JoiningDate < DateTime.Today)
            {
                ModelState.AddModelError("JoiningDate", "Joining Date cannot be a past date.");
            }
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
                    if (employee.ImageFile != null)
                    {
                        var extension = Path.GetExtension(employee.ImageFile.FileName).ToLower();

                        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        {
                            ModelState.AddModelError("ImageFile", "Only JPG, JPEG and PNG files are allowed.");
                            LoadViewBagData();
                            return View(employee);
                        }

                        if (employee.ImageFile.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "Image size should be less than 2 MB.");
                            LoadViewBagData();
                            return View(employee);
                        }

                        string imageName = Guid.NewGuid().ToString() + extension;

                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        string path = Path.Combine(folder, imageName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            employee.ImageFile.CopyTo(stream);
                        }

                        employee.ProfileImage = imageName;
                    }
                    else if (existingEmployee != null)
                    {
                        employee.ProfileImage = existingEmployee.ProfileImage;
                    }
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

            var employees = _employeeRepository.GetAllEmployees(departmentId, null,null);
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
                -6 => "Password must be between 5 and 20 characters",
                _ => "Failed to add employee. Please try again."
            };
        }
    }
}
