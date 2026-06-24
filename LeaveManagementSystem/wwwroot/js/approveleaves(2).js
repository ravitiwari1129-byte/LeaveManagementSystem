// ========================================
// LEAVE MANAGEMENT JS - COMPLETE WORKING
// ========================================

// (function () {
//     'use strict';

//     let gridApi = null;
//     let currentLeaveId = null;
//     let currentAction = null;
//     Helper function to calculate days
//     function calculateDays(fromDateStr, toDateStr) {
//         try {
//             var fromDate = new Date(fromDateStr);
//             var toDate = new Date(toDateStr);
//             return Math.ceil(Math.abs(toDate - fromDate) / (1000 * 60 * 60 * 24)) + 1;
//         } catch (error) {
//             return 1;
//         }
//     }

//     ========================================
//     HISTORY GRID (Leave History Page)
//     ========================================

//     const historyColumnDefs = [
//         {
//             field: "LeaveType",
//             headerName: "Leave Type",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 130
//         },
//         {
//             field: "FromDate",
//             headerName: "From Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110
//         },
//         {
//             field: "ToDate",
//             headerName: "To Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110
//         },
//         {
//             headerName: "Days",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 70,
//             cellStyle: { textAlign: "center", fontWeight: "bold" },
//             valueGetter: function (params) {
//                 return calculateDays(params.data.FromDateRaw, params.data.ToDateRaw);
//             }
//         },
//         {
//             field: "Reason",
//             headerName: "Reason",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 200,
//             tooltipField: "FullReason",
//             cellRenderer: function (params) {
//                 var reason = params.value || "";
//                 return reason.length > 50 ? reason.substring(0, 50) + "..." : reason;
//             }
//         },
//         {
//             field: "Status",
//             headerName: "Status",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 100,
//             cellRenderer: function (params) {
//                 var status = params.value;
//                 var badgeClass = status === "Pending" ? "badge-pending" :
//                     (status === "Approved" ? "badge-approved" : "badge-rejected");
//                 return '<span class="badge ' + badgeClass + '">' + status + "</span>";
//             }
//         },
//         {
//             field: "AppliedDate",
//             headerName: "Applied Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110
//         },
//         {
//             headerName: "Action",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 80,
//             cellRenderer: function (params) {
//                 return '<a href="/Leave/Details/' + params.data.LeaveId + '" class="btn btn-warning" style="padding: 4px 10px; font-size: 11px;">View</a>';
//             }
//         }
//     ];

//     function initHistoryGrid() {
//         var gridDiv = document.getElementById("leaveHistoryGrid");
//         if (gridDiv && typeof agGrid !== "undefined" && window.leaveHistoryData) {
//             gridApi = agGrid.createGrid(gridDiv, {
//                 columnDefs: historyColumnDefs,
//                 defaultColDef: { sortable: true, filter: true, resizable: true },
//                 rowData: window.leaveHistoryData,
//                 domLayout: "normal",
//                 ✅ SIRF YEH onGridReady FUNCTION ADD KARO
//                 onGridReady: function (params) {
//                     var presetStatus = sessionStorage.getItem('presetStatus');
//                     if (presetStatus) {
//                         sessionStorage.removeItem('presetStatus');
//                         setTimeout(function () {
//                             params.api.setFilterModel({
//                                 Status: {
//                                     type: 'equals',
//                                     filter: presetStatus
//                                 }
//                             });
//                             params.api.onFilterChanged();
//                             console.log("Auto-filtered by:", presetStatus);
//                         }, 500);
//                     }
//                 }
//             });
//             console.log("History Grid initialized with", window.leaveHistoryData?.length, "records");
//             updateHistoryStats();
//             setTimeout(function () { if (gridApi) gridApi.sizeColumnsToFit(); }, 100);
//         }
//     }

//     function updateHistoryStats() {
//         if (!window.leaveHistoryData) return;

//         var pendingCount = window.leaveHistoryData.filter(function (item) {
//             return item.Status === "Pending";
//         }).length;

//         var approvedCount = window.leaveHistoryData.filter(function (item) {
//             return item.Status === "Approved";
//         }).length;

//         var rejectedCount = window.leaveHistoryData.filter(function (item) {
//             return item.Status === "Rejected";
//         }).length;

//         var totalDays = 0;
//         for (var i = 0; i < window.leaveHistoryData.length; i++) {
//             if (window.leaveHistoryData[i].Status === "Approved") {
//                 totalDays += calculateDays(
//                     window.leaveHistoryData[i].FromDateRaw,
//                     window.leaveHistoryData[i].ToDateRaw
//                 );
//             }
//         }

//         var pendingElem = document.getElementById("pendingCount");
//         var approvedElem = document.getElementById("approvedCount");
//         var rejectedElem = document.getElementById("rejectedCount");
//         var totalElem = document.getElementById("totalDays");

//         if (pendingElem) pendingElem.textContent = pendingCount;
//         if (approvedElem) approvedElem.textContent = approvedCount;
//         if (rejectedElem) rejectedElem.textContent = rejectedCount;
//         if (totalElem) totalElem.textContent = totalDays;
//     }

//     ========================================
//     APPROVE LEAVE GRID (Pending Leaves)
//     ========================================

//     const pendingColumnDefs = [
//         {
//             field: "EmployeeName",
//             headerName: "Employee",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 160
//         },
//         {
//             field: "LeaveType",
//             headerName: "Leave Type",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 140
//         },
//         {
//             field: "FromDate",
//             headerName: "From Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 130
//         },
//         {
//             field: "ToDate",
//             headerName: "To Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 130
//         },
//         {
//             field: "Reason",
//             headerName: "Reason",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 250,
//             tooltipField: "FullReason",
//             cellRenderer: function (params) {
//                 var reason = params.value || "";
//                 return reason.length > 50 ? reason.substring(0, 50) + "..." : reason;
//             }
//         },
//         {
//             field: "AppliedDate",
//             headerName: "Applied Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 140
//         },
//         {
//             field: "Actions",
//             headerName: "Action",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 220,
//             cellRenderer: function (params) {
//                 var leaveId = params.data.LeaveId;
//                 return '<div class="action-buttons" style="display: flex; gap: 5px;">' +
//                     '<button onclick="showApproveModal(' + leaveId + ')" class="btn btn-success" style="padding: 5px 12px;">Approve</button>' +
//                     '<button onclick="showRejectModal(' + leaveId + ')" class="btn btn-danger" style="padding: 5px 12px;">Reject</button>' +
//                     '<a href="/Leave/Details/' + leaveId + '" class="btn btn-info" style="padding: 5px 12px; text-decoration: none;">View</a>' +
//                     '</div>';
//             }
//         }
//     ];

//     function initPendingGrid() {
//         var gridDiv = document.getElementById("leaveGrid");
//         if (gridDiv && typeof agGrid !== "undefined" && window.leaveData) {
//             gridApi = agGrid.createGrid(gridDiv, {
//                 columnDefs: pendingColumnDefs,
//                 defaultColDef: { sortable: true, filter: true, resizable: true },
//                 rowData: window.leaveData,
//                 domLayout: "normal"
//             });
//             console.log("Pending Leave Grid initialized with", window.leaveData?.length, "records");
//             setTimeout(function () { if (gridApi) gridApi.sizeColumnsToFit(); }, 100);
//         }
//     }

//     ========================================
//     MODAL FUNCTIONS
//     ========================================

//     window.showApproveModal = function (leaveId) {
//         currentLeaveId = leaveId;
//         currentAction = "Approved";
//         document.getElementById("modalTitle").innerHTML = "Approve Leave Request";
//         document.getElementById("modalMessage").innerHTML = "Are you sure you want to approve this leave request?";
//         document.getElementById("actionModal").style.display = "flex";
//         document.getElementById("remarks").value = "";
//     };

//     window.showRejectModal = function (leaveId) {
//         currentLeaveId = leaveId;
//         currentAction = "Rejected";
//         document.getElementById("modalTitle").innerHTML = "Reject Leave Request";
//         document.getElementById("modalMessage").innerHTML = "Are you sure you want to reject this leave request?";
//         document.getElementById("actionModal").style.display = "flex";
//         document.getElementById("remarks").value = "";
//     };

//     window.closeModal = function () {
//         document.getElementById("actionModal").style.display = "none";
//         currentLeaveId = null;
//         currentAction = null;
//     };

//     ========================================
//     CONFIRM ACTION - FIXED WITH PROPER JSON
//     ========================================
//     window.confirmAction = function () {
//         if (!currentLeaveId || !currentAction) {
//             showAlertMessage("No action selected", "error");
//             return;
//         }

//         var remarks = document.getElementById("remarks")?.value || "";
//         var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

//         fetch('/Leave/ApproveReject', {
//             method: 'POST',
//             headers: {
//                 'Content-Type': 'application/json',
//                 'RequestVerificationToken': token
//             },
//             body: JSON.stringify({
//                 leaveId: currentLeaveId,
//                 status: currentAction,
//                 remarks: remarks
//             })
//         })
//             .then(function (response) {
//                 return response.json();
//             })
//             .then(function (data) {
//                 if (data.success) {
//                     if (gridApi) {
//                         var rowsToRemove = [];
//                         gridApi.forEachNode(function (node) {
//                             if (node.data.LeaveId === currentLeaveId) {
//                                 rowsToRemove.push(node);
//                             }
//                         });
//                         gridApi.applyTransaction({ remove: rowsToRemove.map(function (r) { return r.data; }) });

//                         var remainingRows = gridApi.getModel().getRowCount();
//                         var pendingCountElem = document.querySelector(".stats-grid .stat-card.pending .stat-number");
//                         if (pendingCountElem) pendingCountElem.textContent = remainingRows;
//                     }

//                     showAlertMessage("Leave request " + currentAction.toLowerCase() + " successfully!", "success");
//                     window.closeModal();
//                 } else {
//                     showAlertMessage(data.message || "Something went wrong", "error");
//                 }
//             })
//             .catch(function (error) {
//                 console.error("Error:", error);
//                 showAlertMessage("An error occurred. Please try again.", "error");
//             });
//     };

//     function showAlertMessage(message, type) {
//         var className = type === 'success' ? 'alert-success' : 'alert-error';
//         var alertHtml = '<div class="alert ' + className + '" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 250px; padding: 12px;">' + message + '</div>';
//         document.body.insertAdjacentHTML('beforeend', alertHtml);
//         setTimeout(function () {
//             var alert = document.querySelector('.alert');
//             if (alert) alert.remove();
//         }, 3000);
//     }

//     ========================================
//     INITIALIZE BASED ON PAGE
//     ========================================

//     document.addEventListener('DOMContentLoaded', function () {
//         console.log("Leave Management Page Loaded");

//         if (document.getElementById("leaveHistoryGrid")) {
//             console.log("History page detected");
//             initHistoryGrid();
//         }

//         if (document.getElementById("leaveGrid")) {
//             console.log("Approve Leave page detected");
//             initPendingGrid();
//         }
//     });

//     Close modal when clicking outside
//     window.onclick = function (event) {
//         var modal = document.getElementById("actionModal");
//         if (event.target === modal) {
//             window.closeModal();
//         }
//     };

//     Handle window resize
//     window.addEventListener('resize', function () {
//         if (gridApi) {
//             setTimeout(function () { gridApi.sizeColumnsToFit(); }, 100);
//         }
//     });
// })();


// ========================================
// LEAVE REPORT - AG Grid v35+ Compatible
// WITH CHART FILTER INTEGRATION (Dropdown unchanged)
// ========================================

// (function () {
//     'use strict';

//     let gridApi = null;
//     let allValues = [];
//     Column Definitions (UNCHANGED)
//     const columnDefs = [
//         {
//             field: "employeeName",
//             headerName: "Employee",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 150,
//             minWidth: 130
//         },
//         {
//             field: "leaveType",
//             headerName: "Leave Type",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 130,
//             minWidth: 120
//         },
//         {
//             field: "fromDate",
//             headerName: "From Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110,
//             minWidth: 105,
//             valueFormatter: function (params) {
//                 if (!params.value) return "";
//                 var date = new Date(params.value);
//                 return date.toLocaleDateString("en-GB", {
//                     day: "2-digit",
//                     month: "short",
//                     year: "numeric"
//                 });
//             }
//         },
//         {
//             field: "toDate",
//             headerName: "To Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110,
//             minWidth: 105,
//             valueFormatter: function (params) {
//                 if (!params.value) return "";
//                 var date = new Date(params.value);
//                 return date.toLocaleDateString("en-GB", {
//                     day: "2-digit",
//                     month: "short",
//                     year: "numeric"
//                 });
//             }
//         },
//         {
//             headerName: "Days",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 80,
//             minWidth: 70,
//             cellStyle: { textAlign: "center", fontWeight: "bold" },
//             valueGetter: function (params) {
//                 if (params.data.fromDate && params.data.toDate) {
//                     var fromDate = new Date(params.data.fromDate);
//                     var toDate = new Date(params.data.toDate);
//                     var diffTime = Math.abs(toDate - fromDate);
//                     return Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
//                 }
//                 return 1;
//             }
//         },
//         {
//             field: "reason",
//             headerName: "Reason",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 200,
//             minWidth: 180,
//             tooltipField: "reason",
//             cellRenderer: function (params) {
//                 var reason = params.value || "";
//                 return reason.length > 60 ? reason.substring(0, 60) + "..." : reason;
//             }
//         },
//         {
//             field: "status",
//             headerName: "Status",
//             sortable: true,
//             filter: 'agSetColumnFilter',
//             filter: true,
//             floatingFilter: true,
//             width: 100,
//             minWidth: 100,
//             cellRenderer: function (params) {
//                 var status = params.value;
//                 var badgeClass = "";
//                 if (status === "Pending") badgeClass = "badge-pending";
//                 else if (status === "Approved") badgeClass = "badge-approved";
//                 else if (status === "Rejected") badgeClass = "badge-rejected";
//                 return '<span class="badge ' + badgeClass + '">' + (status || "") + "</span>";
//             }
//         },
//         {
//             field: "appliedDate",
//             headerName: "Applied Date",
//             sortable: true,
//             filter: true,
//             floatingFilter: true,
//             width: 110,
//             minWidth: 105,
//             valueFormatter: function (params) {
//                 if (!params.value) return "";
//                 var date = new Date(params.value);
//                 return date.toLocaleDateString("en-GB", {
//                     day: "2-digit",
//                     month: "short",
//                     year: "numeric"
//                 });
//             }
//         }
//     ];

//     Default Column Properties
//     const defaultColDef = {
//         sortable: true,
//         filter: true,
//         resizable: true,
//         editable: false
//     };

//     Grid Options
//     const gridOptions = {
//         columnDefs: columnDefs,
//         defaultColDef: defaultColDef,
//         rowData: [],
//         domLayout: "normal",
//         animateRows: true,
//         pagination: false,
//         enableCellTextSelection: true,
//         overlayNoRowsTemplate: '<span style="padding: 10px;">Click Generate Report to view data.</span>'
//     };

//     Initialize AG Grid
//     function initGrid() {
//         var gridDiv = document.getElementById("leaveReportGrid");
//         if (gridDiv && typeof agGrid !== "undefined") {
//             gridApi = agGrid.createGrid(gridDiv, gridOptions);
//             window.leaveReportGridApi = gridApi;
//             console.log("✅ Leave Report Grid initialized");
//         } else {
//             console.log("❌ Grid initialization failed");
//         }
//     }

//     Generate Report
//     function generateReport(presetStatus) {
//         console.log("🔍 Generate Report clicked");

//         var employeeId = $("#reportEmployeeId").val();
//         var status = presetStatus || $("#reportStatus").val();

//         var fromDate = $("#reportFromDate").val();
//         var toDate = $("#reportToDate").val();

//         console.log("Search params:", { employeeId, status, fromDate, toDate });

//         if (gridApi) {
//             gridApi.setGridOption("loading", true);
//         }

//         $.ajax({
//             url: "/Dashboard/GetLeaveReport",
//             type: "POST",
//             data: {
//                 employeeId: employeeId,
//                 status: status,
//                 fromDate: fromDate,
//                 toDate: toDate
//             },
//             success: function (response) {
//                 console.log("Response received:", response);

//                 if (response.success && gridApi) {
//                     if (response.data && response.data.length > 0) {
//                         console.log("✅ Found " + response.data.length + " records");
//                         gridApi.setGridOption("rowData", response.data);
//                         gridApi.setFilterModel(null);
//                         ✅ Apply filter ONLY if presetStatus exists (from chart)
//                         if (presetStatus && presetStatus !== "") {
//                         if (status.length > 0) {

//                             gridApi.setFilterModel(null);

//                             if (status.length === 1) {

//                                 gridApi.setFilterModel({
//                                     status: {
//                                         filterType: 'text',
//                                         type: 'equals',
//                                         filter: status[0]
//                                     }
//                                 });

//                             } else {

//                                 gridApi.setFilterModel({
//                                     status: {
//                                         filterType: 'text',
//                                         operator: 'OR',
//                                         conditions: status.map(s => ({
//                                             type: 'equals',
//                                             filter: s
//                                         }))
//                                     }
//                                 });

//                             }

//                             gridApi.onFilterChanged();
//                         }
//                         setTimeout(function () {
//                             gridApi.setFilterModel({
//                                 status: {
//                                     type: 'equals',
//                                     filter: presetStatus
//                                 }
//                             });
//                             gridApi.onFilterChanged();
//                             console.log("✅ Auto-filtered by status:", presetStatus);
//                             showNotification("Showing: " + presetStatus + " leaves");
//                         }, 200);
//                         }
//                     } else {
//                         console.log("⚠️ No records found");
//                         gridApi.setGridOption("rowData", []);
//                         if (presetStatus) {
//                             showNotification("No " + presetStatus + " records found");
//                         }
//                     }
//                     gridApi.setGridOption("loading", false);
//                     setTimeout(function () {
//                         gridApi.sizeColumnsToFit();
//                     }, 100);
//                 } else if (gridApi) {
//                     gridApi.setGridOption("rowData", []);
//                     gridApi.setGridOption("loading", false);
//                     showError(response.message || "Error generating report");
//                 }
//             },
//             error: function (xhr, status, error) {
//                 console.error("❌ AJAX Error:", error);
//                 if (gridApi) {
//                     gridApi.setGridOption("rowData", []);
//                     gridApi.setGridOption("loading", false);
//                 }
//                 showError("Error loading report. Please try again.");
//             }
//         });
//     }

//     Show notification
//     function showNotification(message) {
//         var notification = $('<div class="chart-notification">🎯 ' + message + '</div>');
//         $(".card:has(#leaveReportGrid)").prepend(notification);
//         setTimeout(function () {
//             notification.fadeOut(300, function () { $(this).remove(); });
//         }, 3000);
//     }

//     Reset Filters
//     function resetFilters() {
//         console.log("🔄 Reset button clicked");
//         $("#reportEmployeeId").val("");
//         $("#reportStatus").val("");
//         $("#reportFromDate").val("");
//         $("#reportToDate").val("");
//         if (gridApi) {
//             gridApi.setGridOption("rowData", []);
//             gridApi.setFilterModel(null);
//             gridApi.onFilterChanged();
//             gridApi.setGridOption("loading", false);
//         }
//         console.log("✅ All filters reset to default");
//     }

//     function showError(message) {
//         var alertHtml = '<div class="alert alert-error" style="margin-top: 1rem;">' + message + "</div>";
//         $(".filters").after(alertHtml);
//         setTimeout(function () {
//             $(".alert-error").fadeOut("slow", function () {
//                 $(this).remove();
//             });
//         }, 3000);
//     }

//     function showSuccess(message) {
//         var alertHtml = '<div class="alert alert-success" style="margin-top: 1rem;">' + message + "</div>";
//         $(".filters").after(alertHtml);
//         setTimeout(function () {
//             $(".alert-success").fadeOut("slow", function () {
//                 $(this).remove();
//             });
//         }, 3000);
//     }

//     Document Ready
//     $(document).ready(function () {
//         console.log("=== Leave Report Page Ready ===");
//         initGrid();
//         $('#reportStatus').select2({
//             placeholder: "Select Status",
//             width: '100%'
//         });

//         $('#reportStatus').on('change', function () {

//             let selectedValues = $(this).val() || [];

//             if (selectedValues.includes("All")) {

//                 let allValues = [];

//                 $('#reportStatus option').each(function () {

//                     let value = $(this).val();

//                     if (value && value !== "All") {
//                         allValues.push(value);
//                     }
//                 });

//                 Sirf selection update karo
//                 $(this).val(allValues).trigger('change.select2');
//             }

//             console.log("Selected Status:", $(this).val());

//             generateReport() yahan nahi call karna
//         });

//         $("#generateReportBtn").on("click", function () {

//             let selectedStatuses = $("#reportStatus").val() || [];

//             generateReport(selectedStatuses);
//         });

//         $("#resetReportBtn").on("click", function () {
//             resetFilters();
//             showSuccess("All filters have been reset");
//         });

//         $("#reportEmployeeId, #reportStatus, #reportFromDate, #reportToDate").on("keypress", function (e) {
//             if (e.which === 13) {
//                 generateReport(status);
//             }
//         });

//         ✅ Check for preset status from chart
//         var presetStatus = sessionStorage.getItem('presetStatus');
//         if (presetStatus) {
//             console.log("🎯 Found preset status from chart:", presetStatus);
//             sessionStorage.removeItem('presetStatus');

//             ✅ Generate report with filter (dropdown WILL NOT change)
//             setTimeout(function () {
//                 generateReport(presetStatus);
//             }, 500);
//         }

//         console.log("✅ Events bound successfully");
//     });

//     Window Resize
//     $(window).on("resize", function () {
//         if (gridApi) {
//             setTimeout(function () {
//                 gridApi.sizeColumnsToFit();
//             }, 100);
//         }
//     });
// })();


// const select = document.getElementById("reportStatus");

// const values = Array.from(select.selectedOptions)
//     .map(option => option.value);

// console.log(values);