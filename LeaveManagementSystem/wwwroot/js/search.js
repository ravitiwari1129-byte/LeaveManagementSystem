// ========================================
// SEARCH.JS - Leave Search Functionality
// ========================================

var gridApi = null;

(function ($) {
    'use strict';

    // ========================================
    // COLUMN DEFINITIONS
    // ========================================
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

    // ========================================
    // INITIALIZE GRID
    // ========================================
    function initGrid() {
        console.log("🔍 Initializing search grid...");

        var gridDiv = document.getElementById("searchResultGrid");

        if (!gridDiv) {
            console.error("❌ Grid container not found!");
            return;
        }

        if (typeof agGrid === "undefined") {
            console.error("❌ AG Grid not loaded!");
            return;
        }

        try {
            // ✅ Assign to GLOBAL gridApi
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
                overlayNoRowsTemplate: '<span style="padding: 10px; color: #666;">Enter search criteria and click Search to find results.</span>',
                onGridReady: function (params) {
                    console.log("✅ Grid ready!");
                    setTimeout(function () {
                        params.api.sizeColumnsToFit();
                    }, 100);
                }
            });
            console.log("✅ Search Grid initialized");
            console.log("✅ gridApi is now:", gridApi);
        } catch (error) {
            console.error("❌ Error creating grid:", error);
        }
    }

    // ========================================
    // PERFORM SEARCH
    // ========================================
    window.performSearch = function () {
        console.log("Direct Select2 Value:", $('#searchStatus').val());
        console.log("🔍 Perform Search called...");
        console.log("gridApi status:", gridApi ? "Exists ✅" : "Not exists ❌");

        var selectedEmployees = [];

        var employeeControl = document.getElementById("searchEmployeeName");

        if (employeeControl) {

            // Admin Multi Select
            if (employeeControl.tagName === "SELECT") {
                selectedEmployees = $(employeeControl).val() || [];
            }

            // Employee Textbox
            else {
                var employeeName = employeeControl.value.trim();
                if (employeeName !== "") {
                    selectedEmployees.push(employeeName);
                }
            }
        }

        console.log("Selected Employees:", selectedEmployees);
 

        // Get selected statuses from multi-select
        var selectedStatuses = $('#searchStatus').val() || [];
        console.log("Selected Statuses:", selectedStatuses);

        if (selectedStatuses.includes("All")) {
            selectedStatuses = [];
        }

        console.log("Selected Employees:", selectedEmployees);
        console.log("Selected Statuses:", selectedStatuses);

        var fromDate = document.getElementById("searchFromDate")?.value || "";
        var toDate = document.getElementById("searchToDate")?.value || "";
        console.log("From Date:", fromDate);
        console.log("To Date:", toDate);

        // Show loading
        if (gridApi) {
            gridApi.setGridOption("loading", true);
            gridApi.setGridOption("rowData", []);
        } else {
            console.error("❌ gridApi is null! Cannot update grid.");
            alert("Grid not initialized. Please refresh the page.");
            return;
        }

        var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || "";
        console.log("Token found:", token ? "Yes" : "No");

        // Make AJAX call
        $.ajax({
            url: '/Leave/SearchLeaves',
            type: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            contentType: 'application/json',
            data: JSON.stringify({
                employeeNames: selectedEmployees,
                statuses: selectedStatuses,
                fromDate: fromDate || null,
                toDate: toDate || null
            }),
            success: function (response) {
                console.log("✅ AJAX Success:", response);

                if (gridApi) {
                    gridApi.setGridOption("loading", false);
                }

                if (response.success) {
                    if (response.data && response.data.length > 0) {
                        console.log("✅ Found " + response.data.length + " records");
                        console.log("Sample data:", response.data[0]);
                        if (gridApi) {
                            gridApi.setGridOption("rowData", response.data);
                        }
                    } else {
                        console.log("⚠️ No records found");
                        if (gridApi) {
                            gridApi.setGridOption("rowData", []);
                        }
                        showMessage("⚠️ No records found", "info");
                    }
                } else {
                    showMessage("❌ " + (response.message || "Error searching leaves"), "error");
                }
            },
            error: function (xhr, status, error) {
                console.error("❌ AJAX Error:", error);
                console.error("Status:", status);
                console.error("Response:", xhr.responseText);

                if (gridApi) {
                    gridApi.setGridOption("loading", false);
                    gridApi.setGridOption("rowData", []);
                }
                showMessage("❌ Error: " + error, "error");
            }
        });
    };

    // ========================================
    // RESET SEARCH
    // ========================================
    window.resetSearch = function () {
        console.log("🔄 Reset Search called...");

        var empInput = document.getElementById("searchEmployeeName");

        if (empInput && empInput.tagName === "SELECT") {
            // Admin multi-select reset
            $(empInput).val(null).trigger('change');
        }

        var statusSelect = document.getElementById("searchStatus");
        if (statusSelect) {
            if ($.fn.select2 && $(statusSelect).data('select2')) {
                $(statusSelect).val(null).trigger('change.select2');
            } else {
                for (var i = 0; i < statusSelect.options.length; i++) {
                    statusSelect.options[i].selected = false;
                }
            }
        }

        var fromDate = document.getElementById("searchFromDate");
        if (fromDate) fromDate.value = "";

        var toDate = document.getElementById("searchToDate");
        if (toDate) toDate.value = "";

        if (gridApi) {
            gridApi.setGridOption("rowData", []);
        }

        console.log("✅ Reset complete");
    };

    // ========================================
    // SHOW MESSAGE
    // ========================================
    function showMessage(message, type) {
        var className = type === 'success' ? 'alert-success' :
            type === 'info' ? 'alert-info' : 'alert-error';
        var alertHtml = '<div class="alert ' + className + '" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 250px; padding: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.2);">' + message + '</div>';
        $('body').append(alertHtml);
        setTimeout(function () {
            $('.alert').fadeOut(300, function () {
                $(this).remove();
            });
        }, 3000);
    }

    // ========================================
    // DOCUMENT READY
    // ========================================
    $(document).ready(function () {
        console.log("📄 Search Leaves Page Ready");

        initGrid();

        $('#searchBtn').on('click', function (e) {
            e.preventDefault();
            window.performSearch();
        });

        $('#resetSearchBtn').on('click', function (e) {
            e.preventDefault();
            window.resetSearch();
        });

        $('#searchEmployeeName, #searchStatus, #searchFromDate, #searchToDate').on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                window.performSearch();
            }
        });

        console.log("✅ All events bound!");
    });

    // ========================================
    // WINDOW RESIZE
    // ========================================
    $(window).on('resize', function () {
        if (gridApi) {
            setTimeout(function () {
                gridApi.sizeColumnsToFit();
            }, 100);
        }
    });

})(jQuery);


