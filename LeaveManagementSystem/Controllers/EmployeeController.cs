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
        private readonly string _connectionString;

        public EmployeeController(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        private void LoadViewBagData()
        {
            ViewBag.Departments = _employeeRepository.GetDepartments();
            ViewBag.Roles = new[] { "Employee", "Admin" };
        }

        // ========================================
        // GET: Employee/Index
        // ========================================
        [HttpGet]
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var employees = GetAllEmployees();
            LoadViewBagData();
            return View(employees);
        }

        // ========================================
        // GET: Employee/Create
        // ========================================
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            LoadViewBagData();
            return View(new EmployeeModel());
        }

        // ========================================
        // POST: Employee/Create - HARD BINDING
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeModel employee)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            ModelState.Remove("DepartmentName");
            ModelState.Remove("IsActive");
            ModelState.Remove("EmployeeId");

            // Custom validation
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
                    int id = InsertEmployee(employee);

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

        // ========================================
        // GET: Employee/Edit
        // ========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var employee = GetEmployeeById(id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Index");
            }

            LoadViewBagData();
            return View(employee);
        }

        // ========================================
        // POST: Employee/Edit
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeModel employee)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            ModelState.Remove("DepartmentName");
            ModelState.Remove("IsActive");
            ModelState.Remove("Password");

            if (_employeeRepository == null)
            {
                throw new Exception("_employeeRepository is NULL");
            }

            if (employee == null)
            {
                throw new Exception("employee is NULL");
            }

            // Existing employee ki details nikalo
            var existingEmployee = _employeeRepository.GetEmployeeById(employee.EmployeeId);

            // Agar current Admin ko Employee banaya ja raha hai
            if (existingEmployee != null &&
                existingEmployee.Role == "Admin" &&
                employee.Role != "Admin")
            {
                int adminCount = _employeeRepository.GetAdminCount();
                if (adminCount <= 1)
                {
                    ModelState.AddModelError("",
                        "System mein kam se kam ek Admin hona zaroori hai.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UpdateEmployee(employee);
                    TempData["Success"] = "Employee updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            LoadViewBagData();

            return View(employee);
        }

        // ========================================
        // POST: Employee/Delete
        // ========================================
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

                bool result = DeleteEmployee(request.Id);

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

        // ========================================
        // GET: Employee/GetEmployees
        // ========================================
        [HttpGet]
        public IActionResult GetEmployees(int? departmentId)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            var employees = GetAllEmployees(departmentId);
            return Json(new { success = true, data = employees });
        }

        // ========================================
        // PRIVATE METHODS - HARD BINDING
        // ========================================

        private List<EmployeeModel> GetAllEmployees(int? departmentId = null)
        {
            var employees = new List<EmployeeModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("USP_GetEmployees", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", (object)departmentId ?? DBNull.Value);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new EmployeeModel
                        {
                            EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            Email = reader["Email"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            Role = reader["Role"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }
            return employees;
        }

        private EmployeeModel GetEmployeeById(int employeeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("USP_GetEmployeeById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EmployeeModel
                        {
                            EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            Email = reader["Email"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            Role = reader["Role"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return null;
        }

        private int InsertEmployee(EmployeeModel employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("USP_InsertEmployee", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Parameters in EXACT order as stored procedure
                cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                cmd.Parameters.AddWithValue("@Role", employee.Role);
                cmd.Parameters.AddWithValue("@Password", employee.Password);

                // Output parameter
                SqlParameter outputParam = new SqlParameter("@EmployeeId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;
            }
        }

        private void UpdateEmployee(EmployeeModel employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("USP_UpdateEmployee", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
                cmd.Parameters.AddWithValue("@Role", employee.Role);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool DeleteEmployee(int employeeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("USP_DeleteEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    // Return value from procedure
                    SqlParameter returnParam = new SqlParameter();
                    returnParam.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnParam);

                    cmd.ExecuteNonQuery();

                    int result = (int)returnParam.Value;

                    return result == 1;
                }
            }
        }

        private List<DepartmentModel> GetDepartments()
        {
            var departments = new List<DepartmentModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("USP_GetDepartments", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new DepartmentModel
                        {
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            DepartmentName = reader["DepartmentName"].ToString()
                        });
                    }
                }
            }
            return departments;
        }


        private string GetErrorMessage(int errorCode)
        {
            return errorCode switch
            {
                -1 => "Email already exists in the system",
                -2 => "Selected department is invalid",
                -3 => "Invalid role selected. Must be 'Admin' or 'Employee'",
                -4 => "Employee name cannot be empty",
                -5 => "Invalid email format",
                -6 => "Password must be at least 4 characters",
                _ => "Failed to add employee. Please try again."
            };
        }
    }

}