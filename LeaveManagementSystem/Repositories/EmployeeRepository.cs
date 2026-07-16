using System.Collections;
using System.Data;
using LeaveManagementSystem.Helpers;
using LeaveManagementSystem.Models;
using Microsoft.Data.SqlClient;

namespace LeaveManagementSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public EmployeeRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }


        public List<EmployeeModel> GetAllEmployees(int? departmentId ,string Role,string userName)
        {
            var employees = new List<EmployeeModel>();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("DepartmentId", (object)departmentId ?? (object)DBNull.Value);
            ht.Add("Role",Role);
            ht.Add("UserName", userName);

            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetEmployees", ht);
            foreach (DataRow row in dt.Rows)
            {
                employees.Add(new EmployeeModel
                {
                    EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                    EmployeeName = row["EmployeeName"].ToString(),
                    Email = row["Email"].ToString(),
                    DepartmentId = row["DepartmentId"] != DBNull.Value ? Convert.ToInt32(row["DepartmentId"]) : 0,
                    DepartmentName = row["DepartmentName"] == DBNull.Value ? string.Empty : row["DepartmentName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? string.Empty : row["Role"].ToString(),
                    IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                    DateOfBirth = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : DateTime.MinValue,
                    Gender = row["Gender"] == DBNull.Value ? "" : row["Gender"].ToString(),
                    ProfileImage = row["ProfileImage"] == DBNull.Value ? "" : row["ProfileImage"].ToString(),
                    MobileNo = row["MobileNo"] == DBNull.Value ? "" : row["MobileNo"].ToString(),
                    Salary = row["Salary"] != DBNull.Value ? Convert.ToDecimal(row["Salary"]) : 0,
                    JoiningDate = row["JoiningDate"] != DBNull.Value ? Convert.ToDateTime(row["JoiningDate"]) : DateTime.MinValue,
                    Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
                    Skills = row["Skills"] == DBNull.Value ? "" : row["Skills"].ToString(),
                });
            }
            return employees;
        }


        public EmployeeModel GetEmployeeById(int employeeId)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeId", employeeId);
            DataSet ds = _dbHelper.ExecuteDataSet("USP_GetEmployeeById", ht);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                return new EmployeeModel
                {
                    EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                    EmployeeName = row["EmployeeName"] == DBNull.Value ? string.Empty : row["EmployeeName"].ToString(),
                    Email = row["Email"] == DBNull.Value ? string.Empty : row["Email"].ToString(),
                    Password = row["Password"] == DBNull.Value ? string.Empty : row["Password"].ToString(),
                    DepartmentId = row["DepartmentId"] != DBNull.Value ? Convert.ToInt32(row["DepartmentId"]) : 0,
                    DepartmentName = row["DepartmentName"] == DBNull.Value ? string.Empty : row["DepartmentName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? string.Empty : row["Role"].ToString(),
                    IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                    DateOfBirth = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : DateTime.MinValue,
                    Gender = row["Gender"] == DBNull.Value ? "" : row["Gender"].ToString(),
                    ProfileImage = row["ProfileImage"] == DBNull.Value ? "" : row["ProfileImage"].ToString(),
                    MobileNo = row["MobileNo"] == DBNull.Value ? string.Empty : row["MobileNo"].ToString(),
                    Salary = row["Salary"] != DBNull.Value ? Convert.ToDecimal(row["Salary"]) : 0,
                    JoiningDate = row["JoiningDate"] != DBNull.Value ? Convert.ToDateTime(row["JoiningDate"]) : DateTime.MinValue,
                    Address = row["Address"] == DBNull.Value ? string.Empty : row["Address"].ToString(),
                    Skills = row["Skills"] == DBNull.Value ? string.Empty : row["Skills"].ToString()
                };
            }
            return null;
        }


        public int GetAdminCount()
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            object result = _dbHelper.ExecuteScalar("USP_GetAdminCount", ht);
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }


        public int InsertEmployee(EmployeeModel employee)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeName", employee.EmployeeName);
            ht.Add("Email", employee.Email);
            ht.Add("DepartmentId", employee.DepartmentId);
            ht.Add("Role", employee.Role);
            ht.Add("Password", employee.Password);
            ht.Add("DateOfBirth", employee.DateOfBirth);
            ht.Add("Gender", employee.Gender);
            if (string.IsNullOrEmpty(employee.ProfileImage))
            {
                employee.ProfileImage = "";
            }
            ht.Add("ProfileImage", employee.ProfileImage);
            ht.Add("MobileNo", employee.MobileNo);
            ht.Add("Salary", employee.Salary);
            ht.Add("JoiningDate", employee.JoiningDate);
            ht.Add("Address", employee.Address);
            ht.Add("Skills", employee.Skills);
            return _dbHelper.ExecuteWithOutputParameter("USP_InsertEmployee",ht,"@EmployeeId");
        }

        public bool UpdateEmployee(EmployeeModel employee)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();

            ht.Add("EmployeeId", employee.EmployeeId);
            ht.Add("EmployeeName", employee.EmployeeName);
            ht.Add("Email", employee.Email);
            ht.Add("Password", employee.Password);   // <-- Add this line
            ht.Add("DepartmentId", employee.DepartmentId);
            ht.Add("Role", employee.Role);
            ht.Add("DateOfBirth", employee.DateOfBirth);
            ht.Add("Gender", employee.Gender);

            if (string.IsNullOrEmpty(employee.ProfileImage))
            {
                employee.ProfileImage = "";
            }

            ht.Add("ProfileImage", employee.ProfileImage);
            ht.Add("MobileNo", employee.MobileNo);
            ht.Add("Salary", employee.Salary);
            ht.Add("JoiningDate", employee.JoiningDate);
            ht.Add("Address", employee.Address);
            ht.Add("Skills", employee.Skills);

            int result = _dbHelper.ExecuteWithOutputParameter(
                "USP_UpdateEmployee",
                ht,
                "@Result");

            return result == 1;
        }

        public bool DeleteEmployee(int employeeId)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeId", employeeId);
            int result = _dbHelper.ExecuteWithOutputParameter("USP_DeleteEmployee", ht,"@Result");
            return result == 1;
        }


        public List<DepartmentModel> GetDepartments()
        {
            var departments = new List<DepartmentModel>();
            Hashtable ht = new Hashtable();
            ht.Clear();
            DataSet ds = _dbHelper.ExecuteDataSet("USP_GetDepartments", ht);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    departments.Add(new DepartmentModel
                    {
                        DepartmentId = row["DepartmentId"] != DBNull.Value ? Convert.ToInt32(row["DepartmentId"]) : 0,
                        DepartmentName = row["DepartmentName"] != DBNull.Value ? row["DepartmentName"].ToString() : string.Empty
                    });
                }
            }
            return departments;
        }


        public UserSession ValidateUser(string email, string password)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("Email", email);

            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_ValidateUser", ht);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string storedPassword = row["Password"].ToString();
                if (!string.Equals(storedPassword, password, StringComparison.Ordinal))
                {
                    return null;
                }
                return new UserSession
                {
                    EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                    EmployeeName = row["EmployeeName"] == DBNull.Value ? string.Empty : row["EmployeeName"].ToString(),
                    Email = row["Email"] == DBNull.Value ? string.Empty : row["Email"].ToString(),
                    Role = row["Role"] == DBNull.Value ? string.Empty : row["Role"].ToString(),
                    DepartmentId = row["DepartmentId"] != DBNull.Value ? Convert.ToInt32(row["DepartmentId"]) : (int?)null
                };
            }
            return null;
        }

        public List<RoleModel> GetRoles()
        {
            List<RoleModel> roles = new List<RoleModel>();

            Hashtable ht = new Hashtable();
            ht.Clear();
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetRoles", ht);

            foreach (DataRow row in dt.Rows)
            {
                roles.Add(new RoleModel
                {
                    RoleId = Convert.ToInt32(row["RoleId"]),
                    RoleName = row["RoleName"].ToString()
                });
            }

            return roles;
        }
        public bool ToggleUserStatus(int employeeId, bool isActive)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@EmployeeId", employeeId);
            ht.Add("@IsActive", isActive);

            int result = _dbHelper.ExecuteWithOutputParameter(
                "USP_ToggleUserStatus",
                ht,
                "@Result");

            return result == 1;
        }

    }
}
