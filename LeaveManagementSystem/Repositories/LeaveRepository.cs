using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LeaveManagementSystem.Helpers;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {

        private readonly DatabaseHelper _dbHelper;
        private readonly string _connectionString;
        public LeaveRepository(DatabaseHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public (bool Success, string Message) ApplyLeave(LeaveRequestModel leave)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeId", leave.EmployeeId);
            ht.Add("LeaveType", leave.LeaveType);
            ht.Add("FromDate", leave.FromDate);
            ht.Add("ToDate", leave.ToDate);
            ht.Add("Reason", leave.Reason ?? (object)DBNull.Value);
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_ApplyLeave", ht);
            if (dt.Rows.Count > 0)
            {
                int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                string message = dt.Rows[0]["Message"].ToString();
                return (result == 1, message);
            }
            return (false, "Failed to apply leave");
        }


        public (bool Success, string Message) ApproveRejectLeave( int leaveId, string status, int approvedBy, string remarks)
        {
            try
            {
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("LeaveId", leaveId);
                ht.Add("Status", status);
                ht.Add("ApprovedBy", approvedBy);
                ht.Add("Remarks", (object)remarks ?? DBNull.Value);
                DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_ApproveRejectLeave",ht);
                if (dt.Rows.Count > 0)
                {
                    int result = Convert.ToInt32(dt.Rows[0]["Result"]);
                    string message = dt.Rows[0]["Message"].ToString();
                    return (result == 1, message);
                }
                return (false, "Operation failed");
            }
            catch (Exception ex)
            {
                return (false, "Error: " + ex.Message);
            }
        }

        public List<LeaveRequestModel> SearchLeaves(List<string> employeeNames,List<string> statuses,DateTime? fromDate,DateTime? toDate,string role,int? employeeId)
        {
            var leaves = new List<LeaveRequestModel>();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeNames",employeeNames != null && employeeNames.Any() ? string.Join(",", employeeNames) : (object)DBNull.Value);
            ht.Add("Statuses",statuses != null && statuses.Any() ? string.Join(",", statuses) : (object)DBNull.Value);
            ht.Add("FromDate", (object)fromDate ?? DBNull.Value);
            ht.Add("ToDate", (object)toDate ?? DBNull.Value);
            ht.Add("Role", role);
            ht.Add("EmployeeId", (object)employeeId ?? DBNull.Value);
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_SearchLeaves",ht);
            foreach (DataRow row in dt.Rows)
            {
                leaves.Add(new LeaveRequestModel
                {
                    LeaveId = Convert.ToInt32(row["LeaveId"]),
                    EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                    EmployeeName = row["EmployeeName"].ToString(),
                    LeaveType = row["LeaveType"].ToString(),
                    FromDate = Convert.ToDateTime(row["FromDate"]),
                    ToDate = Convert.ToDateTime(row["ToDate"]),
                    Reason = row["Reason"].ToString(),
                    Status = row["Status"].ToString(),
                    AppliedDate = Convert.ToDateTime(row["AppliedDate"]),
                    ApprovedByName = row["ApprovedByName"]?.ToString(),
                    Remarks = row["Remarks"]?.ToString()
                });
            }
            return leaves;
        }


        public LeaveRequestModel GetLeaveById(int leaveId)
        {
            try
            {
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("LeaveId", leaveId);
                DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetLeaveById", ht);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new LeaveRequestModel
                    {
                        LeaveId = Convert.ToInt32(row["LeaveId"]),
                        EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                        EmployeeName = row["EmployeeName"].ToString(),
                        LeaveType = row["LeaveType"].ToString(),
                        FromDate = Convert.ToDateTime(row["FromDate"]),
                        ToDate = Convert.ToDateTime(row["ToDate"]),
                        Reason = row["Reason"].ToString(),
                        Status = row["Status"].ToString(),
                        AppliedDate = Convert.ToDateTime(row["AppliedDate"]),
                        ApprovedByName = row["ApprovedByName"] == DBNull.Value ? string.Empty : row["ApprovedByName"].ToString(),
                        Remarks = row["Remarks"] == DBNull.Value ? string.Empty : row["Remarks"].ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetLeaveById: {ex.Message}");
                throw;
            }
        }


        public DashboardModel GetDashboardCounts(string role,int employeeId)
        {
            Hashtable ht = new Hashtable();
            ht.Add("Role", role);
            ht.Add("EmployeeId", employeeId);
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetLeaveDashboard",ht);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new DashboardModel
                {
                    PendingCount = row["PendingCount"] != DBNull.Value ? Convert.ToInt32(row["PendingCount"]) : 0,
                    ApprovedCount = row["ApprovedCount"] != DBNull.Value ? Convert.ToInt32(row["ApprovedCount"]) : 0,
                    RejectedCount = row["RejectedCount"] != DBNull.Value ? Convert.ToInt32(row["RejectedCount"]) : 0,
                    MonthlyCount = row["MonthlyCount"] != DBNull.Value ? Convert.ToInt32(row["MonthlyCount"]) : 0
                };
            }
            return new DashboardModel();
        }


        public DashboardModel GetLeaveSummary(int employeeId)
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("EmployeeId", employeeId);
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetLeaveSummary", ht);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new DashboardModel
                {
                    PendingLeaves = row["PendingLeaves"] != DBNull.Value ? Convert.ToInt32(row["PendingLeaves"]) : 0,
                    ApprovedLeaves = row["ApprovedLeaves"] != DBNull.Value ? Convert.ToInt32(row["ApprovedLeaves"]) : 0,
                    RejectedLeaves = row["RejectedLeaves"] != DBNull.Value ? Convert.ToInt32(row["RejectedLeaves"]) : 0,
                    TotalDaysTaken = row["TotalDaysTaken"] != DBNull.Value ? Convert.ToInt32(row["TotalDaysTaken"]) : 0
                };
            }
            return new DashboardModel();
        }


        public List<LeaveTypeModel> GetLeaveTypes()
        {
            List<LeaveTypeModel> leaveTypes = new List<LeaveTypeModel>();
            Hashtable ht = new Hashtable();
            ht.Clear();     
            DataTable dt = _dbHelper.ExecuteStoredProcedure("USP_GetLeaveTypes", ht);
            foreach (DataRow row in dt.Rows)
            {
                leaveTypes.Add(new LeaveTypeModel
                {
                    LeaveTypeId = Convert.ToInt32(row["LeaveTypeId"]),
                    LeaveTypeName = row["LeaveTypeName"].ToString()
                });
            }
            return leaveTypes;
        }

    }
}

