using LeaveManagementSystem.Models;
using System.Collections.Generic;

namespace LeaveManagementSystem.Repositories
{
    public interface ILeaveRepository
    {
        (bool Success, string Message) ApplyLeave(LeaveRequestModel leave);
        (bool Success, string Message) ApproveRejectLeave(int leaveId, string status, int approvedBy, string remarks);
        List<LeaveRequestModel> SearchLeaves(List<string> employeeNames,List<string> statuses,DateTime? fromDate,DateTime? toDate,string role,int? employeeId);
        DashboardModel GetDashboardCounts(string role, int employeeId);
        LeaveRequestModel GetLeaveById(int leaveId);
        DashboardModel GetLeaveSummary(int employeeId);
        List<LeaveTypeModel> GetLeaveTypes();
    }
}