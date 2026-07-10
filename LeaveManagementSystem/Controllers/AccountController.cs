using System;
using System.Data;
using LeaveManagementSystem.Helpers;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

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
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("AccessDenied");
            }

            SignupModel model = new SignupModel();

            model.DepartmentList = _employeeRepository.GetDepartments()
                .Select(x => new SelectListItem
                {
                    Value = x.DepartmentId.ToString(),
                    Text = x.DepartmentName
                }).ToList();

            model.RoleList = _employeeRepository.GetRoles()
                .Where(x => x.RoleName != "Admin")
                .Select(x => new SelectListItem
                {
                    Value = x.RoleName,
                    Text = x.RoleName
                }).ToList();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("AccessDenied");
            }

            model.DepartmentList = _employeeRepository.GetDepartments()
                .Select(x => new SelectListItem
                {
                    Value = x.DepartmentId.ToString(),
                    Text = x.DepartmentName
                }).ToList();

            model.RoleList = _employeeRepository.GetRoles()
                .Where(x => x.RoleName != "Admin")
                .Select(x => new SelectListItem
                {
                    Value = x.RoleName,
                    Text = x.RoleName
                }).ToList();

            if (model.Skills == null || !model.Skills.Any())
            {
                ModelState.AddModelError(nameof(model.Skills), "Select at least one Skill");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                if (model.ProfileImage == null)
                {
                    ModelState.AddModelError("ProfileImage", "Profile Image is required.");
                    return View(model);
                }
                if (model.ProfileImage != null)
                {
                    var extension = Path.GetExtension(model.ProfileImage.FileName).ToLower();

                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        ModelState.AddModelError("ProfileImage", "Only JPG, JPEG and PNG files are allowed.");
                        return View(model);
                    }
                }

                if (model.ProfileImage != null)
                {
                    if (model.ProfileImage.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ProfileImage", "Image size should be less than 2 MB.");
                        return View(model);
                    }
                }

                if (model.DateOfBirth >= DateTime.Today)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of Birth must be before today.");
                    return View(model);
                }

                if (model.JoiningDate < DateTime.Today)
                {
                    ModelState.AddModelError("JoiningDate", "Joining Date cannot be past date.");
                    return View(model);
                }

                if (model.DepartmentId <= 0)
                {
                    ModelState.AddModelError("DepartmentId", "Please select Department.");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.Role))
                {
                    ModelState.AddModelError("Role", "Please select Role.");
                    return View(model);
                }

                if (model.Salary < 0)
                {
                    ModelState.AddModelError("Salary", "Salary cannot be negative.");
                    return View(model);
                }

                int age = DateTime.Today.Year - model.DateOfBirth.Year;
                if (model.DateOfBirth > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
                if (age < 18)
                {
                    ModelState.AddModelError("DateOfBirth", "Employee must be at least 18 years old.");
                    return View(model);
                }

                string imageName = null;
                if (model.ProfileImage != null)
                {
                    imageName = Guid.NewGuid() + Path.GetExtension(model.ProfileImage.FileName);
                    string folder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    string path = Path.Combine(folder, imageName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        model.ProfileImage.CopyTo(stream);
                    }
                }

                EmployeeModel employee = new EmployeeModel
                {
                    EmployeeName = model.FullName,
                    Email = model.Email,
                    Password = model.Password,
                    DepartmentId = model.DepartmentId,
                    Role = model.Role,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    ProfileImage = imageName,
                    MobileNo = model.MobileNo,
                    Salary = model.Salary,
                    JoiningDate = model.JoiningDate,
                    Address = model.Address,
                    Skills = model.Skills != null ? string.Join(",", model.Skills) : ""
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
                        ModelState.AddModelError("Password", "Password must be at least 5 characters.");
                        break;
                    case -99:
                        ModelState.AddModelError("", "An unexpected database error occurred.");
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

