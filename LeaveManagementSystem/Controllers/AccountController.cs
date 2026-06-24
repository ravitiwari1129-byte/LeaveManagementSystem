using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly string _connectionString;

        public AccountController(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        // ========================================
        // LOGIN - GET
        // ========================================
        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Index", "Report");
            }
            return View(new LoginModel());
        }

        // ========================================
        // LOGIN - POST (FIXED)
        // ========================================
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

                // ✅ FIXED: Redirect to Report/Index (Dashboard)
                return RedirectToAction("Index", "Report");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        // ========================================
        // SIGNUP - GET
        // ========================================
        [HttpGet]
        public IActionResult Signup()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Index", "Report");
            }
            return View(new SignupModel());
        }

        // ========================================
        // SIGNUP - POST
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupModel model)
        {
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (EmailExists(model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                int employeeId = CreateEmployeeUsingStoredProcedure(model);

                if (employeeId > 0)
                {
                    TempData["Success"] = "Account created successfully! Please login.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Failed to create account. Please try again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            return View(model);
        }

        // ========================================
        // LOGOUT
        // ========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        // ========================================
        // PRIVATE METHODS
        // ========================================
        private bool EmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("USP_CheckEmailExists", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);

                    SqlParameter resultParam = new SqlParameter("@Exists", System.Data.SqlDbType.Bit)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    command.Parameters.Add(resultParam);

                    command.ExecuteNonQuery();
                    return Convert.ToBoolean(resultParam.Value);
                }
            }
        }

        private int CreateEmployeeUsingStoredProcedure(SignupModel model)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("USP_InsertEmployee", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmployeeName", model.FullName);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@DepartmentId", 1);
                    command.Parameters.AddWithValue("@Role", "Employee");
                    command.Parameters.AddWithValue("@Password", model.Password);

                    SqlParameter outputParam = new SqlParameter("@EmployeeId", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    command.ExecuteNonQuery();

                    if (outputParam.Value != DBNull.Value)
                    {
                        return Convert.ToInt32(outputParam.Value);
                    }
                    return 0;
                }
            }
        }
    }
}