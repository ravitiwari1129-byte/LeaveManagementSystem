using System;
using LeaveManagementSystem.Helpers;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly DatabaseHelper _db;

        public AccountController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }

        private void SetUserSession(UserSession user)
        {
            HttpContext.Session.SetString("EmployeeId", user.EmployeeId.ToString());
            HttpContext.Session.SetString("EmployeeName", user.EmployeeName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            if (user.DepartmentId.HasValue)
            {
                HttpContext.Session.SetString("DepartmentId", user.DepartmentId.Value.ToString());
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Dashboard", "Report");
            }
            return View(new LoginModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = _employeeRepository.ValidateUser(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }
                SetUserSession(user);
                return RedirectToAction("Dashboard", "Report");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Signup()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Dashboard", "Report");
            }
            return View(new SignupModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                EmployeeModel employee = new EmployeeModel
                {
                    EmployeeName = model.FullName,
                    Email = model.Email,
                    DepartmentId = 1,
                    Role = "Employee",
                    Password = model.Password
                };
                int employeeId = _employeeRepository.InsertEmployee(employee);
                if (employeeId > 0)
                {
                    TempData["Success"] = "Account created successfully! Please login.";
                    return RedirectToAction("Login");
                }
                switch (employeeId)
                {
                    case -1:
                        ModelState.AddModelError("Email", "Email already exists.");
                        break;
                    case -2:
                        ModelState.AddModelError("", "Department not found.");
                        break;
                    case -3:
                        ModelState.AddModelError("", "Invalid role.");
                        break;
                    case -4:
                        ModelState.AddModelError("", "Employee name cannot be empty.");
                        break;
                    case -5:
                        ModelState.AddModelError("Email", "Invalid email format.");
                        break;
                    case -6:
                        ModelState.AddModelError("Password", "Password must be at least 4 characters.");
                        break;
                    default:
                        ModelState.AddModelError("", "Failed to create account.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {            
            return View();
        }
    }
}
