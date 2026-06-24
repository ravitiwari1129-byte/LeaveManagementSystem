using LeaveManagementSystem.Models;
using System.Collections.Generic;

namespace LeaveManagementSystem.Repositories
{
    public interface IEmployeeRepository
    {
        List<EmployeeModel> GetAllEmployees(int? departmentId = null);
        EmployeeModel GetEmployeeById(int employeeId);
        int GetAdminCount();
        int InsertEmployee(EmployeeModel employee);
        bool UpdateEmployee(EmployeeModel employee);
        bool DeleteEmployee(int employeeId);
        List<DepartmentModel> GetDepartments();
        UserSession ValidateUser(string email, string password);
    }
}