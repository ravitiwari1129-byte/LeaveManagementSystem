// ========================================
// LEAVE MANAGEMENT JS - COMPLETE WORKING
// ========================================

(function () {
    'use strict';

    let gridApi = null;
    let currentLeaveId = null;
    let currentAction = null;
    function calculateDays(fromDateStr, toDateStr) {
        try {
            var fromDate = new Date(fromDateStr);
            var toDate = new Date(toDateStr);
            return Math.ceil(Math.abs(toDate - fromDate) / (1000 * 60 * 60 * 24)) + 1;
        } catch (error) {
            return 1;
        }
    }

// ========================================
// HISTORY GRID (Leave History Page)
// ========================================

const historyColumnDefs = [
    {
        field: "LeaveType",
        headerName: "Leave Type",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        field: "FromDate",
        headerName: "From Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        field: "ToDate",
        headerName: "To Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        headerName: "Days",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 100,
        cellStyle: { textAlign: "center", fontWeight: "bold" },
        valueGetter: function (params) {
            return calculateDays(params.data.FromDateRaw, params.data.ToDateRaw);
        }
    },
    {
        field: "Reason",
        headerName: "Reason",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 200,
        tooltipField: "FullReason",
        cellRenderer: function (params) {
            var reason = params.value || "";
            return reason.length > 50 ? reason.substring(0, 50) : reason;
        }
    },
    {
        field: "Status",
        headerName: "Status",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130,
        cellRenderer: function (params) {
            var status = params.value;
            var badgeClass = status === "Pending" ? "badge-pending" :
                (status === "Approved" ? "badge-approved" : "badge-rejected");
            return '<span class="badge ' + badgeClass + '">' + status + "</span>";
        }
    },
    {
        field: "AppliedDate",
        headerName: "Applied Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        headerName: "Action",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 100,
        cellRenderer: function (params) {
            return '<a href="/Leave/Details/' + params.data.LeaveId + '" class="btn btn-warning" style="padding: 4px 10px; font-size: 11px;">View</a>';
        }
    }
];

function initHistoryGrid() {
    var gridDiv = document.getElementById("leaveHistoryGrid");
    if (gridDiv && typeof agGrid !== "undefined" && window.leaveHistoryData) {
        gridApi = agGrid.createGrid(gridDiv, {
            columnDefs: historyColumnDefs,
            defaultColDef: { sortable: true, filter: true, resizable: true },
            rowData: window.leaveHistoryData,
            domLayout: "normal",
            onGridReady: function (params) {
                if (window.presetStatus) {
                    setTimeout(function () {
                        params.api.setFilterModel({
                            Status: {
                                type: 'equals',
                                filter: window.presetStatus
                            }
                        });
                        params.api.onFilterChanged();
                        console.log("Auto-filtered by:", window.presetStatus);
                        window.presetStatus = null;
                    }, 500);
                }
            }
        });
        console.log("History Grid initialized with", window.leaveHistoryData?.length, "records");
        updateHistoryStats();
        setTimeout(function () { if (gridApi) gridApi.sizeColumnsToFit(); }, 100);
    }
}

function updateHistoryStats() {
    if (!window.leaveHistoryData) return;

    var pendingCount = window.leaveHistoryData.filter(function (item) {
        return item.Status === "Pending";
    }).length;

    var approvedCount = window.leaveHistoryData.filter(function (item) {
        return item.Status === "Approved";
    }).length;

    var rejectedCount = window.leaveHistoryData.filter(function (item) {
        return item.Status === "Rejected";
    }).length;

    var totalDays = 0;
    for (var i = 0; i < window.leaveHistoryData.length; i++) {
        if (window.leaveHistoryData[i].Status === "Approved") {
            totalDays += calculateDays(
                window.leaveHistoryData[i].FromDateRaw,
                window.leaveHistoryData[i].ToDateRaw
            );
        }
    }

    var pendingElem = document.getElementById("pendingCount");
    var approvedElem = document.getElementById("approvedCount");
    var rejectedElem = document.getElementById("rejectedCount");
    var totalElem = document.getElementById("totalDays");

    if (pendingElem) pendingElem.textContent = pendingCount;
    if (approvedElem) approvedElem.textContent = approvedCount;
    if (rejectedElem) rejectedElem.textContent = rejectedCount;
    if (totalElem) totalElem.textContent = totalDays;
}

// ========================================
// APPROVE LEAVE GRID (Pending Leaves)
// ========================================

const pendingColumnDefs = [
    {
        field: "EmployeeName",
        headerName: "Employee",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 160
    },
    {
        field: "LeaveType",
        headerName: "Leave Type",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        field: "FromDate",
        headerName: "From Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 140
    },
    {
        field: "ToDate",
        headerName: "To Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 140
    },
    {
        field: "Reason",
        headerName: "Reason",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 250,
        tooltipField: "FullReason",
        cellRenderer: function (params) {
            var reason = params.value || "";
            return reason.length > 50 ? reason.substring(0, 50) : reason;
        }
    },
    {
        field: "AppliedDate",
        headerName: "Applied Date",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 130
    },
    {
        field: "Actions",
        headerName: "Action",
        sortable: true,
        filter: true,
        floatingFilter: true,
        minWidth: 220,
        cellRenderer: function (params) {
            var leaveId = params.data.LeaveId;
            return '<div class="action-buttons" style="display: flex; gap: 5px;">' +
                '<button onclick="showApproveModal(' + leaveId + ')" class="btn btn-success" style="padding: 5px 12px;">Approve</button>' +
                '<button onclick="showRejectModal(' + leaveId + ')" class="btn btn-danger" style="padding: 5px 12px;">Reject</button>' +
                '<a href="/Leave/Details/' + leaveId + '" class="btn btn-info" style="padding: 5px 12px; text-decoration: none;">View</a>' +
                '</div>';
        }
    }
];

    window.exportApproveLeave = function () {
        if (gridApi) {
            gridApi.ApproveLeave({
                fileName: "LeaveHistory.csv"
            });
        }
        else {
            alert("Grid not initialized.");
        }
    };

function initPendingGrid() {
    var gridDiv = document.getElementById("leaveGrid");
    if (gridDiv && typeof agGrid !== "undefined" && window.leaveData) {
        gridApi = agGrid.createGrid(gridDiv, {
            columnDefs: pendingColumnDefs,
            defaultColDef: { sortable: true, filter: true, floatingFilter: true, resizable: true },
            rowData: window.leaveData,
            domLayout: "normal"
        });
        console.log("Pending Leave Grid initialized with", window.leaveData?.length, "records");
        setTimeout(function () { if (gridApi) gridApi.sizeColumnsToFit(); }, 100);
    }
}

// ========================================
// MODAL FUNCTIONS
// ========================================

window.showApproveModal = function (leaveId) {
    currentLeaveId = leaveId;
    currentAction = "Approved";
    document.getElementById("modalTitle").innerHTML = "Approve Leave Request";
    document.getElementById("modalMessage").innerHTML = "Are you sure you want to approve this leave request?";
    document.getElementById("actionModal").style.display = "flex";
    document.getElementById("remarks").value = "";
};

window.showRejectModal = function (leaveId) {
    currentLeaveId = leaveId;
    currentAction = "Rejected";
    document.getElementById("modalTitle").innerHTML = "Reject Leave Request";
    document.getElementById("modalMessage").innerHTML = "Are you sure you want to reject this leave request?";
    document.getElementById("actionModal").style.display = "flex";
    document.getElementById("remarks").value = "";
};

window.closeModal = function () {
    document.getElementById("actionModal").style.display = "none";
    currentLeaveId = null;
    currentAction = null;
};

// ========================================
// CONFIRM ACTION - FIXED WITH PROPER JSON
// ========================================
window.confirmAction = function () {
    if (!currentLeaveId || !currentAction) {
        showAlertMessage("No action selected", "error");
        return;
    }

    var remarks = document.getElementById("remarks")?.value || "";
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

    fetch('/Leave/ApproveReject', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            leaveId: currentLeaveId,
            status: currentAction,
            remarks: remarks
        })
    })
        .then(function (response) {
            return response.json();
        })
        .then(function (data) {
            if (data.success) {
                if (gridApi) {
                    var rowsToRemove = [];
                    gridApi.forEachNode(function (node) {
                        if (node.data.LeaveId === currentLeaveId) {
                            rowsToRemove.push(node);
                        }
                    });
                    gridApi.applyTransaction({ remove: rowsToRemove.map(function (r) { return r.data; }) });

                    var remainingRows = gridApi.getModel().getRowCount();
                    var pendingCountElem = document.querySelector(".stats-grid .stat-card.pending .stat-number");
                    if (pendingCountElem) pendingCountElem.textContent = remainingRows;
                }

                showAlertMessage("Leave request " + currentAction.toLowerCase() + " successfully!", "success");
                window.closeModal();
            } else {
                showAlertMessage(data.message || "Something went wrong", "error");
            }
        })
        .catch(function (error) {
            console.error("Error:", error);
            showAlertMessage("An error occurred. Please try again.", "error");
        });
};

function showAlertMessage(message, type) {
    var className = type === 'success' ? 'alert-success' : 'alert-error';
    var alertHtml = '<div class="alert ' + className + '" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 250px; padding: 12px;">' + message + '</div>';
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    setTimeout(function () {
        var alert = document.querySelector('.alert');
        if (alert) alert.remove();
    }, 3000);
}

    window.exportLeaveHistory = function () {
        if (gridApi) {
            gridApi.exportDataAsCsv({
                fileName: "LeaveHistory.csv"
            });
        }
        else {
            alert("Grid not initialized.");
        }
    };

// ========================================
// INITIALIZE BASED ON PAGE
// ========================================

document.addEventListener('DOMContentLoaded', function () {
    console.log("Leave Management Page Loaded");

    if (document.getElementById("leaveHistoryGrid")) {
        console.log("History page detected");
        initHistoryGrid();
    }

    if (document.getElementById("leaveGrid")) {
        console.log("Approve Leave page detected");
        initPendingGrid();
    }
});

// Close modal when clicking outside
window.onclick = function (event) {
    var modal = document.getElementById("actionModal");
    if (event.target === modal) {
        window.closeModal();
    }
};

// Handle window resize
window.addEventListener('resize', function () {
    if (gridApi) {
        setTimeout(function () { gridApi.sizeColumnsToFit(); }, 100);
    }
    });
})();


