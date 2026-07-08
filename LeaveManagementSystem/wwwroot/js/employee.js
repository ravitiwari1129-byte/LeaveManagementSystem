// ========================================
// EMPLOYEE MANAGEMENT JS - PROPER WORKING
// ========================================

let gridApi;

const columnDefs = [
    {
        field: "ProfileImage",
        headerName: " Profile Image",
        minWidth: 120,
        sortable: false,
        filter: false,
        cellRenderer: function (params) {
            if (!params.data.ProfileImage || params.data.ProfileImage.trim() === "") {
                return `<span style="color:#999;">No Image</span>`;
            }
            return ` <img src="/uploads/${params.data.ProfileImage}" width="45" height="45" style="border-radius:50%;object-fit:cover;" onerror="this.outerHTML='<span style=&quot;color:#999;&quot;>No Image</span>'" /> `;
        }
    },
    {
        field: "EmployeeName",
        headerName: "Employee Name",
        sortable: true,
        filter: true,
        floatingFilter: true,
        flex: 2,
        minWidth: 150
    },
    {
        field: "Email",
        headerName: "Email",
        sortable: true,
        filter: true,
        floatingFilter: true,
        flex: 2,
        minWidth: 200
    },
    {
        field: "DepartmentName",
        headerName: "Department",
        sortable: true,
        filter: true,
        floatingFilter: true,
        flex: 1.5,
        minWidth: 120
    },
    {
        field: "Role",
        headerName: "Role",
        sortable: true,
        filter: true,
        floatingFilter: true,
        flex: 1,
        minWidth: 100
    },
    {
        field: "Status", headerName: "Status", sortable: true, filter: true, floatingFilter: true, flex: 1, minWidth: 100,
        cellRenderer: function (params) {
            var status = params.value;
            var badgeClass = status === 'Active' ? 'badge-approved' : 'badge-rejected';
            return '<span class="badge ' + badgeClass + '">' + status + '</span>';
        }
    },
    {
        field: "Actions",
        headerName: "Actions",
        sortable: false,
        filter: false,
        flex: 1.5,
        minWidth: 130,
        cellRenderer: function (params) {
            var empId = params.data.EmployeeId;
            return '<div class="action-links">' +
                '<a href="/Employee/Edit/' + empId + '" class="btn btn-warning" style="padding: 4px 10px; font-size: 12px; margin-right: 5px;">Edit</a>' +
                '<button onclick="deleteEmployee(' + empId + ')" class="btn btn-danger" style="padding: 4px 10px; font-size: 12px;">Delete</button>' +
                '</div>';
        }
    }
];

window.exportEmployeeManagement = function () {
    console.log("Button clicked");

    if (gridApi) {
        console.log("Export starting");

        gridApi.exportDataAsCsv({
            fileName: "EmployeeManagement.csv"
        });

        console.log("Export completed");
    }
    else {
        alert("Grid not initialized.");
    }
};

function initGrid() {
    var gridDiv = document.getElementById('employeeGrid');
    if (gridDiv && typeof agGrid !== 'undefined') {
        gridApi = agGrid.createGrid(gridDiv, {
            columnDefs: columnDefs,
            defaultColDef: { sortable: true, filter: true, resizable: true },
            rowData: window.employeeData || [],
            domLayout: 'normal',
            animateRows: true
        });
        setTimeout(function () {
            if (gridApi) gridApi.sizeColumnsToFit();
        }, 100);
    }
}

// ========================================
// DELETE FUNCTION 
// ========================================
window.deleteEmployee = function (employeeId) {
    if (!confirm('Are you sure you want to delete this employee?\n\nNote: All leave requests of this employee will also be deleted.')) {
        return;
    }
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    fetch('/Employee/Delete/' + employeeId, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({ id: employeeId })
    })
        .then(function (response) {
            return response.json();
        })
        .then(function (data) {
            if (data.success) {
                // Update local data
                var updatedData = window.employeeData.filter(function (emp) {
                    return emp.EmployeeId !== employeeId;
                });
                window.employeeData = updatedData;
                // Update grid
                if (gridApi) {
                    gridApi.setGridOption('rowData', updatedData);
                }
                // Update stats
                updateStats();
                // Show success message
                showMessage('Employee deleted successfully', 'success');
            } else {
                showMessage(data.message || 'Error deleting employee', 'error');
            }
        })
        .catch(function (error) {
            console.error('Error:', error);
            showMessage('Error deleting employee. Please try again.', 'error');
        });
};

function updateStats() {
    var totalCount = window.employeeData.length;
    var activeCount = window.employeeData.filter(function (e) { return e.IsActive; }).length;
    var departmentCount = new Set(window.employeeData.map(function (e) { return e.DepartmentName; })).size;

    var statNumbers = document.querySelectorAll('.stats-grid .stat-number');
    if (statNumbers[0]) statNumbers[0].textContent = totalCount;
    if (statNumbers[1]) statNumbers[1].textContent = activeCount;
    if (statNumbers[2]) statNumbers[2].textContent = departmentCount;
}

function showMessage(message, type) {
    var className = type === 'success' ? 'alert-success' : 'alert-error';
    var alertHtml = '<div class="alert ' + className + '" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 250px; padding: 12px;">' + message + '</div>';
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    setTimeout(function () {
        var alert = document.querySelector('.alert');
        if (alert) alert.remove();
    }, 3000);
}

// Initialize
document.addEventListener('DOMContentLoaded', initGrid);
window.addEventListener('resize', function () {
    setTimeout(function () {
        if (gridApi) gridApi.sizeColumnsToFit();
    }, 100);
});

