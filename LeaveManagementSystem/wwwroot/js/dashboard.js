// ========================================
// DASHBOARD CHART - FIXED REDIRECTS
// ========================================

(function ($) {
    'use strict';

    let chartInstance = null;

    function createChart() {
        console.log("🔍 createChart() called...");

        const container = document.getElementById('myChart');
        if (!container) {
            console.error("❌ Container #myChart not found!");
            return;
        }

        if (!window.dashboardData) {
            console.error("❌ dashboardData not found!");
            return;
        }

        if (typeof agCharts === 'undefined') {
            console.error("❌ AG Charts not loaded!");
            return;
        }

        const chartData = [
            {
                status: 'Pending',
                count: window.dashboardData.pendingCount || 0
            },
            {
                status: 'Approved',
                count: window.dashboardData.approvedCount || 0
            },
            {
                status: 'Rejected',
                count: window.dashboardData.rejectedCount || 0
            }
        ];

        console.log("📊 Chart Data:", chartData);

        const options = {
            container: container,
            data: chartData,
            title: {
                text: "Leave Requests Overview",
                fontSize: 16,
                fontWeight: "bold"
            },
            subtitle: {
                text: "Click on any bar to view filtered report",
                fontSize: 12
            },
            series: [{
                type: 'bar',
                xKey: 'status',
                yKey: 'count',
                fill: '#3498db',
                strokeWidth: 0,
                cornerRadius: 6,
                label: {
                    enabled: true,
                    fontWeight: 'bold',
                    fontSize: 14,
                    color: '#fff'
                }
            }]
        };

        try {
            if (chartInstance) {
                chartInstance.destroy();
                console.log("🔄 Previous chart destroyed");
            }

            chartInstance = agCharts.AgCharts.create(options);
            console.log("✅ Chart created successfully!");

            // ✅ Click handler - Redirect to Leave Report with status
            container.style.cursor = 'pointer';
            container.onclick = function (e) {
                const rect = container.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const width = rect.width;

                const barWidth = width / 3;
                const barIndex = Math.floor(x / barWidth);
                const statuses = ['Pending', 'Approved', 'Rejected'];

                if (barIndex >= 0 && barIndex < 3) {
                    const clickedStatus = statuses[barIndex];
                    const count = window.dashboardData[clickedStatus.toLowerCase() + 'Count'] || 0;

                    console.log(`📊 Clicked: ${clickedStatus} (${count} requests)`);

                    if (count > 0) {
                        // ✅ Store status in sessionStorage
                        sessionStorage.setItem('presetStatus', clickedStatus);
                        console.log(`✅ Status stored: ${clickedStatus}`);

                        // ✅ Redirect to Leave Report
                        if (window.userRole === 'Admin') {
                            window.location.href = '/Report/Index';
                        } else {
                            window.location.href = '/Report/Dashboard';
                        }
                    }
                }
            };

        } catch (error) {
            console.error("❌ Error creating chart:", error);
        }
    }

    $(document).ready(function () {
        console.log("📄 Document ready, initializing chart...");
        setTimeout(createChart, 300);
    });

})(jQuery);

