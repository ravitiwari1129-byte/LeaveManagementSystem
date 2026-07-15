// ========================================
// LEAVE REPORT - Complete Working
// ========================================

var gridApi = null;

(function () {
    'use strict';

    // Column Definitions
    var columnDefs = [
        {
            field: "employeeName",
            headerName: "Employee",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 2,
            minWidth: 150
        },
        {
            field: "leaveType",
            headerName: "Leave Type",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 1.5,
            tooltipField: "leaveType",
            minWidth: 130
        },
        {
            field: "fromDate",
            headerName: "From Date",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 1,
            minWidth: 130,
            valueFormatter: function (params) {
                if (!params.value) return "";
                var date = new Date(params.value);
                return date.toLocaleDateString("en-GB", {
                    day: "2-digit",
                    month: "short",
                    year: "numeric"
                });
            }
        },
        {
            field: "toDate",
            headerName: "To Date",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 1,
            minWidth: 130,
            valueFormatter: function (params) {
                if (!params.value) return "";
                var date = new Date(params.value);
                return date.toLocaleDateString("en-GB", {
                    day: "2-digit",
                    month: "short",
                    year: "numeric"
                });
            }
        },
        {
            headerName: "Days",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 0.5,
            minWidth: 100,
            cellStyle: { textAlign: "center", fontWeight: "bold" },
            valueGetter: function (params) {
                if (params.data.fromDate && params.data.toDate) {
                    var fromDate = new Date(params.data.fromDate);
                    var toDate = new Date(params.data.toDate);
                    var diffTime = Math.abs(toDate - fromDate);
                    return Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
                }
                return 1;
            }
        },
        {
            field: "reason",
            headerName: "Reason",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 2.5,
            minWidth: 200,
            tooltipField: "reason",
            comparator: function (valueA, valueB) {
                valueA = valueA || "";
                valueB = valueB || "";
                return valueA.localeCompare(valueB);
            },
            cellRenderer: function (params) {
                var reason = params.value || "";
                return reason.length > 60 ? reason.substring(0, 60) : reason;
            }
        },
        {
            field: "status",
            headerName: "Status",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 1,
            minWidth: 130,
            cellRenderer: function (params) {
                var status = params.value;
                var badgeClass = "";
                if (status === "Pending") badgeClass = "badge-pending";
                else if (status === "Approved") badgeClass = "badge-approved";
                else if (status === "Rejected") badgeClass = "badge-rejected";
                return '<span class="badge ' + badgeClass + '">' + (status || "") + "</span>";
            }
        },
        {
            field: "appliedDate",
            headerName: "Applied Date",
            sortable: true,
            filter: true,
            floatingFilter: true,
            flex: 1,
            minWidth: 130,
            valueFormatter: function (params) {
                if (!params.value) return "";
                var date = new Date(params.value);
                return date.toLocaleDateString("en-GB", {
                    day: "2-digit",
                    month: "short",
                    year: "numeric"
                });
            }
        }
    ];


    // Initialize Grid
    function initGrid() {
        var gridDiv = document.getElementById("leaveReportGrid");
        if (gridDiv && typeof agGrid !== "undefined") {
            gridApi = agGrid.createGrid(gridDiv, {
                columnDefs: columnDefs,
                defaultColDef: {
                    sortable: true,
                    filter: true,
                    resizable: true
                },
                rowData: [],
                domLayout: "normal",
                animateRows: true,
                overlayNoRowsTemplate: '<span style="padding: 10px;">Click Generate Report to view data.</span>',
                onGridReady: function (params) {
                    setTimeout(function () {
                        params.api.sizeColumnsToFit();
                    }, 100);
                }
            });
            console.log("✅ Leave Report Grid initialized");
        }
    }

    function generateReport() {
        console.log("🔍 Generate Report clicked");

        var employeeId = [];

        if ($("#reportEmployeeId").length > 0 && $("#reportEmployeeId").is("select")) {
            employeeId = $("#reportEmployeeId option:selected").map(function () {
                return $(this).val();
            }).get();
        } else {
            var val = $("#empid").val();
            if (val) {
                employeeId.push(val);
            }
        }

        var status = $("#reportStatus option:selected").map(function () {
            return $(this).val();
        }).get();

        if (status.includes("All")) {
            status = [];
        }

        var fromDate = $("#reportFromDate").val();
        var toDate = $("#reportToDate").val();

        console.log("Employee IDs:", employeeId);
        console.log("Statuses:", status);

        if (gridApi) {
            gridApi.setGridOption("loading", true);
        }

        $.ajax({
            url: "/Report/GetLeaveReport",
            type: "POST",
            data: {
                employeeId: employeeId,
                status: status,
                fromDate: fromDate,
                toDate: toDate
            },
            success: function (response) {
                console.log("Response received:", response);

                if (response.success && gridApi) {
                    gridApi.setGridOption("rowData", response.data || []);

                    if (!response.data || response.data.length === 0) {
                        showNotification("No records found");
                    }

                    gridApi.setGridOption("loading", false);

                    setTimeout(function () {
                        gridApi.sizeColumnsToFit();
                    }, 100);
                }
            },
            error: function (xhr, status, error) {
                console.error("❌ AJAX Error:", error);

                if (gridApi) {
                    gridApi.setGridOption("rowData", []);
                    gridApi.setGridOption("loading", false);
                }

                showError("Error loading report. Please try again.");
            }
        });
    }

    // Reset Filters
    function resetFilters() {
        console.log("🔄 Reset button clicked");
        $("#reportEmployeeId").val(null).trigger('change.select2');
        $("#reportStatus").val(null).trigger('change.select2');
        $("#reportFromDate").val("");
        $("#reportToDate").val("");
        if (gridApi) {
            gridApi.setGridOption("rowData", []);
            gridApi.setFilterModel(null);
            gridApi.setGridOption("loading", false);
        }
        console.log("✅ All filters reset");
    }

    // Show Notification
    function showNotification(message) {
        var notification = $('<div class="chart-notification" style="background: #2c3e50; color: white; padding: 10px 20px; border-radius: 6px; margin-bottom: 15px; font-size: 14px;">📋 ' + message + '</div>');
        $(".card:has(#leaveReportGrid)").prepend(notification);
        setTimeout(function () {
            notification.fadeOut(300, function () { $(this).remove(); });
        }, 3000);
    }

    function showError(message) {
        var alertHtml = '<div class="alert alert-error" style="margin-top: 1rem;">❌ ' + message + "</div>";
        $(".filters").after(alertHtml);
        setTimeout(function () {
            $(".alert-error").fadeOut("slow", function () {
                $(this).remove();
            });
        }, 3000);
    }

    // Document Ready
    $(document).ready(function () {
        console.log("=== Leave Report Page Ready ===");

        initGrid();

        $('#reportEmployeeId').select2({
            placeholder: "Select Employee",
            width: '100%',
            allowClear: true
        });

        $('#reportEmployeeId').on('change', function () {
            let selectedValues = $(this).val() || [];
            if (selectedValues.includes("All")) {
                let allValues = [];
                $('#reportEmployeeId option').each(function () {
                    if ($(this).val() !== "All") {
                        allValues.push($(this).val());
                    }
                });
                $(this).val(allValues).trigger('change.select2');
            }
        });

        $('#reportStatus').select2({
            placeholder: "Select Status",
            width: '100%',
            allowClear: true
        });

        $('#reportStatus').on('change', function () {
            let selectedValues = $(this).val() || [];

            if (selectedValues.includes("All")) {

                let allValues = [];

                $('#reportStatus option').each(function () {
                    if ($(this).val() !== "All") {
                        allValues.push($(this).val());
                    }
                });

                $(this).val(allValues).trigger('change.select2');
            }
        });
        
        $("#generateReportBtn").on("click", function () {
            generateReport();
        });

        
        $("#resetReportBtn").on("click", function () {
            resetFilters();
        });

        
        $("#reportEmployeeId, #reportStatus, #reportFromDate, #reportToDate").on("keypress", function (e) {
            if (e.which === 13) {
                generateReport();
            }
        });

        console.log("✅ Events bound successfully");
    });

    // Window Resize
    $(window).on("resize", function () {
        if (gridApi) {
            setTimeout(function () {
                gridApi.sizeColumnsToFit();
            }, 100);
        }
    });

})();


window.exportLeaveReport = function () {
    if (gridApi) {
        gridApi.exportDataAsCsv({
            fileName: "LeaveReport.csv"
        });
    }
    else {
        alert("Grid not initialized.");
    }
};